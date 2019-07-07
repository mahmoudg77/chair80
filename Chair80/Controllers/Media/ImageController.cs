using Chair80.BLL;
using Chair80.DAL;
using Chair80.Filters;
using Chair80.Libs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Chair80.Controllers.Account
{
    [AppFilter]
    [RoutePrefix("{lang}/Image")]
    public class ImageController : ApiController
    {
        /// <summary>
        /// Upload Images
        /// </summary>
        /// <param name="lang">UI Language</param>
        /// <param name="model">Model Name</param>
        /// <param name="model_id">Model ID</param>
        /// <param name="model_tag">Image name </param>
        /// <returns></returns>
        [HttpPost]
        [Route("Upload")]
        [AuthFilter("Upload Image")]
        public APIResult<List<APIResult<tbl_images>>> Upload(string model, int model_id, string model_tag = "main")
        {

            List<APIResult<tbl_images>> dict = new List<APIResult<tbl_images>>();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                return Images.SaveImagesFromRequest(httpRequest, "en", model, model_id, model_tag);

            }
            catch (Exception ex)
            {
                string res = string.Format(ex.Message);
                dict.Add(APIResult<tbl_images>.Error(ResponseCode.BackendDatabase, res));
                return APIResult<List<APIResult<tbl_images>>>.Error(ResponseCode.BackendDatabase, res, dict);
            }

        }


        /// <summary>
        /// Get one image by ID
        /// </summary>
        /// <param name="id">Image ID</param>
        /// <returns>List Of tbl_mages</returns>
        public APIResult<tbl_images> Get(int id)
        {
            using (MainEntities ctx = new MainEntities())
            {
                tbl_images img = ctx.tbl_images.Where(a => a.id == id).FirstOrDefault();
                if (img != null)
                {
                    return APIResult<tbl_images>.Success( img, "API_SUCCESS");
                }
            }
            return APIResult<tbl_images>.Error(ResponseCode.BackendInternalServer, "Bad Request!");
        }

        public APIResult<List<tbl_images>> Get(string model, int model_id, string model_tag = "main")
        {

            //var accs = new BL.Accounts.Accounts();

            using (MainEntities ctx = new MainEntities())
            {
                var imgs = ctx.tbl_images.Where(a => a.model_id == model_id && a.model_name == model && a.model_tag == model_tag).ToList();
                if (imgs != null)
                {
                    return APIResult<List<tbl_images>>.Success( imgs, "API_SUCCESS");
                }

            }

            return APIResult<List<tbl_images>>.Error(ResponseCode.BackendInternalServer, "API_ERROR_BAD");
        }


        /// <summary>
        /// Delete Image By ID (AuthFilter)
        /// </summary>
        /// <param name="id">Image ID</param>
        /// <returns>True if succes, False if not</returns>
        [AuthFilter("Delete Image")]
        public APIResult<bool> Delete(int id)
        {
            
            //var accs = new BL.Accounts.Accounts();

            using (MainEntities ctx = new MainEntities())
            {
                tbl_images img = ctx.tbl_images.Where(a => a.id == id).FirstOrDefault();

                if (img != null)
                {
                    try
                    {
                        ctx.Entry(img).State = System.Data.Entity.EntityState.Deleted;
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["mediaServer_Path"] + img.large));
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["mediaServer_Path"] + img.thumb));
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["mediaServer_Path"] + img.meduim));
                        System.IO.File.Delete(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["mediaServer_Path"] + img.original));

                    }
                    catch (Exception)
                    {

                       
                    }
                  

                    return APIResult<bool>.Success( true, "API_SUCCESS");
                }

            }

            return APIResult<bool>.Error(ResponseCode.BackendInternalServer, "API_ERROR_BAD");
        }


    }
    }

