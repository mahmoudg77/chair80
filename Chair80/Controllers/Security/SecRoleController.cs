﻿
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Http;
using Chair80.DAL;
using Chair80.Filters;
using Chair80.Libs;
using Chair80.Requests;

namespace Chair80.Controllers
{
    /// <summary>
    /// Roles and permissions control
    /// </summary>
    [AppFilter]
    [RoutePrefix("{lang}/SecRole")]
    public class SecRoleController : ApiController
    {
        /// <summary>
        /// Gat All Roles (Datatable - AuthFilter)
        /// </summary>
        /// <param name="request">Datatable Request</param>
        /// <returns>Datatable structure</returns>
        [HttpPost]
        [Route("All")]
        [AuthFilter("View All Roles")]
        public APIResult<Libs.DataTableResponse<sec_roles>> All(Requests.DataTableRequest request)
        {
            Libs.DataTableResponse<sec_roles> response = Libs.DataTableResponse<sec_roles>.getDataTable(Libs.General.getDataTabe(request, "sec_roles"));
            if (response.data == null)
                return APIResult<Libs.DataTableResponse<sec_roles>>.Error(ResponseCode.BackendInternalServer, "API_ERROR_BAD");

            return APIResult<Libs.DataTableResponse<sec_roles>>.Success( response, "API_SUCCESS");
        }

        /// <summary>
        /// Add New Role (AuthFilter)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthFilter("Add New Role")]
        public APIResult<sec_roles> Post(sec_roles request)
        {

            using(MainEntities ctx=new MainEntities())
            {
                ctx.sec_roles.Add(request);
                
                if (ctx.SaveChanges()>0)
                {

                    return APIResult<sec_roles>.Success( request, "API_SUCCESS");
                }

            }
           

            return APIResult<sec_roles>.Error(ResponseCode.BackendInternalServer, "API_ERROR_BAD");
        }
        /// <summary>
        /// Edit Specific Role by ID (AuthFilter)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthFilter("Edit Role")]
        public APIResult<sec_roles> Put(int id,sec_roles request)
        {
            using (MainEntities ctx = new MainEntities())
            {
                var sec_role = ctx.sec_roles.Find(id);
                sec_role.name = request.name;
                sec_role.role_key = request.role_key;
                sec_role.description = request.description;

                ctx.Entry(sec_role).State=System.Data.Entity.EntityState.Modified;

                if (ctx.SaveChanges() > 0)
                {

                    return APIResult<sec_roles>.Success( request, "API_SUCCESS");
                }

            }

            return APIResult<sec_roles>.Error(ResponseCode.BackendDatabase, "API_ERROR_BAD");
        }
        /// <summary>
        /// Delete Specific Role By ID (AuthFilter)
        /// </summary>
        /// <param name="id"> Role ID</param>
        /// <returns></returns>
        [AuthFilter("Delete Role")]
        public APIResult<bool> Delete(int id)
        {
            using (MainEntities ctx = new MainEntities())
            {
                var sec_role = ctx.sec_roles.Find(id);
                ctx.Entry(sec_role).State = System.Data.Entity.EntityState.Deleted;

                if (ctx.SaveChanges() > 0)
                {

                    return APIResult<bool>.Success( true, "API_SUCCESS");
                }

            }

            return APIResult<bool>.Error(ResponseCode.BackendDatabase, "API_ERROR_BAD",false);
        }

        /// <summary>
        /// Get Specific Role By ID (AuthFilter)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthFilter("View Role Details")]
        public APIResult<sec_roles> Get(int id)
        {
            using(MainEntities ctx=new MainEntities())
            {
                var sec_roles = ctx.sec_roles.Find(id);
                if (sec_roles != null)
                {
                    return APIResult<sec_roles>.Success(sec_roles, "API_SUCCESS");
                }
            }
           
            return APIResult<sec_roles>.Error(ResponseCode.BackendDatabase, "API_ERROR_BAD");
        }
        /// <summary>
        /// Get All System Screen (AuthFilter)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Screen")]
        [AuthFilter("View All Screens")]
        public APIResult<List<string>> Screens()
        {
            return APIResult<List<string>>.Success( Functions.NameSpaceClasses(), "API_SUCCESS");
        }
        /// <summary>
        /// Get All Methods on specific screen (AuthFilter)
        /// </summary>
        /// <param name="screen"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Methods")]
        [AuthFilter("View All Permissions")]
        public APIResult<List<KeyValuePair<string, string>>> Methods(string screen)
        {
            var lst = Functions.ClassMethods(screen);
            return APIResult<List<KeyValuePair<string, string>>>.Success(lst, "API_SUCCESS");
        }
        /// <summary>
        /// Get Methods Name in Specific screen witch Allowed for Specific Role (AuthFilter) 
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RoleMethods")]
        [AuthFilter("View Role Permissions")]
        public APIResult<List<string>> RoleMethods(string screen,int role)
        {
            MainEntities ctx = new MainEntities();

            var lst = ctx.sec_access_right.Where(a=>a.model_name==screen && a.role_id==role).Select(b=>b.method_name).ToList();
            return APIResult<List<string>>.Success( lst, "API_SUCCESS");
        }

        /// <summary>
        /// Save permissions
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SavePermissions")]
        [AuthFilter("Edit Permisions")]
        public APIResult<bool> SavePermissions(SavePermissionRequest request)
        {

            using (MainEntities ctx = new MainEntities())
            {
                var deleted=ctx.sec_access_right.Where(a => a.role_id == request.role_id && a.model_name == request.screen);
               
                foreach(sec_access_right itm in deleted)
                ctx.Entry(itm).State=System.Data.Entity.EntityState.Deleted;

                if (deleted.Count() > 0) ctx.SaveChanges();


                    foreach (var m in request.methods)
                    {
                        sec_access_right access = new sec_access_right();
                        access.method_name = m;
                        access.model_name = request.screen;
                        access.role_id = request.role_id;
                        access.force_filter = "";

                        ctx.sec_access_right.Add(access);
                    }
                    if (ctx.SaveChanges() > 0)
                    {
                        return APIResult<bool>.Success( true, "API_SUCCESS");
                    }
                 

            }


            return APIResult<bool>.Error(ResponseCode.BackendDatabase, "API_ERROR_BAD",false);
        }
    }
}
