using Chari80.BLL;
using Chari80.DAL;
using Chari80.Filters;
using Chari80.Libs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Chari80.Controllers.Account
{
    [AppFilter]
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
        [Route("Image/Upload")]
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
                dict.Add(new APIResult<tbl_images>(ResultType.fail, null, res));
                return new APIResult<List<APIResult<tbl_images>>>(ResultType.fail, res, dict);
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
                    return new APIResult<tbl_images>(ResultType.success, img, "API_SUCCESS");
                }
            }
            return new APIResult<tbl_images>(ResultType.fail, null, "Bad Request!");
        }

        public APIResult<List<tbl_images>> Get(string model, int model_id, string model_tag = "main")
        {

            //var accs = new BL.Accounts.Accounts();

            using (MainEntities ctx = new MainEntities())
            {
                var imgs = ctx.tbl_images.Where(a => a.model_id == model_id && a.model_name == model && a.model_tag == model_tag).ToList();
                if (imgs != null)
                {
                    return new APIResult<List<tbl_images>>(ResultType.success, imgs, "API_SUCCESS");
                }

            }

            return new APIResult<List<tbl_images>>(ResultType.fail, null, "API_ERROR_BAD");
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
                  

                    return new APIResult<bool>(ResultType.success, true, "API_SUCCESS");
                }

            }

            return new APIResult<bool>(ResultType.fail, false, "API_ERROR_BAD");
        }


    }
    }

