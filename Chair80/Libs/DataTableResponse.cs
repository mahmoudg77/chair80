using Chair80.DAL;
using Chair80.Requests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Chair80.Libs
{
    public class DataTableResponse<T> where T :class
    {
        public DataTableResponse()
        {
        }

        public DataTableResponse(int draw, int recordsTotal, int recordsFiltered, IEnumerable<T> data)
        {
            this.draw = draw;
            this.recordsTotal = recordsTotal;
            this.recordsFiltered = recordsFiltered;
            this.data = data;
        }

        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public IEnumerable<T> data { get; set; }
        
        public static DataTableResponse<T> getDataTable(DataTableResponse<Dictionary<string,object>> dt)
        {
            DataTableResponse<T> returned = new DataTableResponse<T>();
            returned.draw = dt.draw;
            returned.recordsFiltered = dt.recordsFiltered;
            returned.recordsTotal = dt.recordsTotal;
            returned.data = JsonConvert.DeserializeObject<IEnumerable<T>>(JsonConvert.SerializeObject(dt.data));
            return returned;
        }

        public static DataTableResponse<T> getDataTable(IEnumerable<T> list, IEnumerable<KeyValuePair<string, string>> querystring, Requests.DataTableRequest request)
        {
            DataTableResponse<Dictionary<string, object>> dataTableResponse = new DataTableResponse<Dictionary<string, object>>();
            DataTable dt = new DataTable();

            dt = list.ToDataTable();

            dataTableResponse.recordsTotal = list.Count();

            string whr = "";
            //foreach (var i in querystring)
            //{
            //    whr += (whr == "" ? "" : " and ") + i.Key + " = '" + i.Value + "'";
            //}

            if (request != null && request.search != null)
            {

                string subWhr = "";
                foreach (var item in request.columns.Where(a => a.searchable).ToList())
                {
                    subWhr += (subWhr == "" ? "" : " or ") + item.data + " like '%" + request.search.value + "%' ";
                }
                //subWhr += ")";
                whr += (whr == "" ? (subWhr == "" ? "" : subWhr) : (subWhr == "" ? "" : " and " + subWhr)) ;

            }

            string orders = "";
            if (request != null && request.order != null && request.order.Count > 0)
            {
                foreach (var ord in request.order)
                {
                    if (request.columns[ord.column].orderable)
                    {
                        orders += (orders == "" ? "" : " , ") + request.columns[ord.column].data + " " + ord.dir;
                    }
                }
                //orders = (orders == "" ? "" : " order by ") + orders;
            }
            int length = 1000;
            int start = 0;
            if (request != null)
            {
                length = (request.length==-1? length: request.length);
                start = request.start;
            }

            List<DataRow> rows = dt.Select(whr, orders).ToList();
            dataTableResponse.recordsFiltered = rows.Count();

            rows = rows.Skip(start).Take(length).ToList();
            DataTable newdt = new DataTable();
            foreach (DataColumn c in dt.Columns) newdt.Columns.Add(c.ColumnName, c.DataType); 
            foreach (var row in rows) newdt.LoadDataRow(row.ItemArray,true);
            dataTableResponse.data = General.DataTableToDictionary(newdt);


                Libs.DataTableResponse<T> d = getDataTable(dataTableResponse);


                return d;

             
        }
        
        //public static IEnumerable<T> Create<T>(IQueryable<T> results, Requests.DataTableRequest request) 
        //{
        //    var skip = request.start;
        //    var pageSize = request.length;
        //    var orderedResults = OrderResults(results, request);
        //    return pageSize > 0 ? orderedResults.Skip(skip).Take(pageSize).ToList() :
        //          orderedResults.ToList();
        //}

        public static IEnumerable<T> OrderResults<T>(IQueryable<T> results, Requests.DataTableRequest request)
        {
            if (request.order == null) return results;
            foreach(var ord in request.order)
            {
                var columnName = request.columns[ord.column].data;
                if (ord.dir == "asc")  results = results.OrderBy(typeof(T).GetProperty(columnName).Name);
                if (ord.dir == "desc") results = results.OrderByDescending(typeof(T).GetProperty(columnName).Name);
            }
            return results;
        }


        //private static DataTableResponse<DAL.Item.vwItem> WrapSearch
        //   (ICollection<DAL.Item.vwItem> details, DataTableRequest request)
        //{
        //    var results = DataTableFilters.FilterItems(details, request.search.value).ToList();
        //    var response = new DataTableResponse<DAL.Item.vwItem>()
        //    {
        //        data = Create(results, request),
        //        draw = request.draw,
        //        recordsFiltered = results.Count,
        //        recordsTotal = details.Count
        //    };
        //    return response;
        //}


        public static IQueryable<T> Filter
                  (IQueryable<T> details, string searchText, Requests.DataTableRequest request)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return details;
            }

            var ds = searchText.Split(' ');

            var results = details;
            foreach (var i in ds)
            {
                results = results.Where(x =>
                         x.ConcatColumns(request.columns.Where(a => a.searchable).Select(a => a.data).ToList()).ContainsIgnoringCase(i.Trim())
                );
            }

            return results;
        }


    }
}
