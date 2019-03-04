using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chair80.Requests
{
    public class DataTableRequest
    {
        public int draw { get; set; }
        public int length { get; set; }
        public int start { get; set; }
        public List<dtColumn> columns { get; set; }
        public List<dtOrder> order { get; set; }
        public dtColumnSearch search { get; set; }
    }

    public class dtColumn
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool orderable { get; set; }
        public bool searchable { get; set; }
        public dtColumnSearch search { get; set; }
    }
    public class dtColumnSearch
    {
        public bool regex { get; set; }
        public string value { get; set; }
    }
    public class dtOrder
    {
        public int column { get; set; }
        public string dir { get; set; }

    }
}