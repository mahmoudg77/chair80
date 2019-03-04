using Chair80Admin.DAL;
using Chair80Admin.Libs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Chair80Admin.BLL
{
    public class Images
    {

        public static string resize(string img, float BOXHEIGHT, float BOXWIDTH,string path)
        {

            Image image = Image.FromFile(img);
            
            float scaleHeight = (float)BOXHEIGHT / (float)image.Height;
            float scaleWidth = (float)BOXWIDTH / (float)image.Width;

            float scale = Math.Min(scaleHeight, scaleWidth);

            Image thumb = image.GetThumbnailImage((int)(image.Width * scale), (int)(image.Height * scale), () => false, IntPtr.Zero);

            System.IO.FileInfo f = new System.IO.FileInfo(img);

       
             

            thumb.Save(f.Directory.Parent.FullName + @"\" + path + @"\" + f.Name, image.RawFormat);

            thumb.Dispose();

            return f.Directory.Parent.FullName + @"\" + path + @"\" + f.Name;
        }

        public static APIResult<List<APIResult<tbl_images>>> SaveImagesFromRequest(HttpRequest httpRequest, string lang, string model, int model_id, string model_tag = "main")
        {
            List<APIResult<tbl_images>> dict = new List<APIResult<tbl_images>>();


            foreach (string file in httpRequest.Files)
            {
                var postedFile = httpRequest.Files[file];

                dict.Add(SaveImageFromFile(postedFile, model, model_id, model_tag));
                //var message1 = string.Format("Image Updated Successfully.");
                //return new APIResult<Dictionary<string, object>>(ResultType.success, message1, dict);
            }
          // var res = string.Format("Please Upload a image.");
            //dict.Add(new APIResult<tbl_images>(ResultType.fail, null, res));
            return new APIResult<List<APIResult<tbl_images>>>(ResultType.success, dict, "API_SUCCESS");
        }


        public static APIResult<tbl_images> SaveImageFromFile(HttpPostedFile postedFile,string model,int model_id,string model_tag = "main")
        {
            if (postedFile != null && postedFile.ContentLength > 0)
            {

                int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                List<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                var extension = ext.ToLower();
                if (!AllowedFileExtensions.Contains(extension))
                {

                    var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

                     return new APIResult<tbl_images>(ResultType.fail, null, message);
                }
                else if (postedFile.ContentLength > MaxContentLength)
                {

                    var message = string.Format("Please Upload a file upto 1 mb.");

                    return new APIResult<tbl_images>(ResultType.fail, null, message);
                }
                else
                {
                    Random random = new Random();
                    var code = random.Next(100000, 999999);

                    string fname = DateTime.Now.ToString("yyyyMMddHHmmss-")+ code  + postedFile.FileName;
                    string online_original = "/Storage/Original/" + fname;
                    string online_thumb = "/Storage/Thumb/" + fname;
                    string online_medium = "/Storage/Medium/" + fname;
                    string online_large = "/Storage/Large/" + fname;

                    string original =ConfigurationManager.AppSettings["mediaServer_Path"] + online_original.Replace("/","\\");// HttpContext.Current.Server.MapPath("~" + online_original);


                    postedFile.SaveAs(original);
                    postedFile = null;
                    string thumb = BLL.Images.resize(original, 120, 120, "Thumb");
                    string medium = BLL.Images.resize(original, 400, 400, "Medium");
                    string large = BLL.Images.resize(original, 800, 800, "Large");


                    using (MainEntities ctx = new MainEntities())
                    {
                        tbl_images img = new tbl_images();
                        img.large = online_large;
                        img.meduim = online_medium;
                        img.original = online_original;
                        img.thumb = online_thumb;
                        img.model_name = model;
                        img.model_id = model_id;
                        img.model_tag = model_tag;

                        ctx.tbl_images.Add(img);
                        ctx.SaveChanges();

                        return new APIResult<tbl_images>(ResultType.success, img, "API_SUCCESS");
                    }


                }
            }
            return new APIResult<tbl_images>(ResultType.fail, null, "API_ERROR_UPPLOAD");
        }
    }
}