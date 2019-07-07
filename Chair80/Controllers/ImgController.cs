using Chair80.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chair80.Controllers
{
    public class ImgController : Controller
    {



        public ActionResult Index(int width,int height)
        {
            //Response.Clear();
            string filename = @"E:\Works\Chair80\backend\Chair80\Storage\Original\20190220231807-268275logo.png";
            var source= Image.FromFile(filename);
            

            var img = BLL.Images.ResizeImageKeepAspectRatio(source, width, height);
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                return base.File(ms.ToArray(), "image/jpeg");
            }
         }
        // GET: Img
        [HttpGet]
        public ActionResult Item(string model, int model_id, string model_tag = "main", string size = "original", string index = "0", string mode="scale")
        {


            string thumb = "120x120";
            string medium = "400x400";
            string large = "800x800";

            int width = 0, height = 0;

            switch (size.ToLower())
            {
                case "original":
                    width = 0;
                    height = 0;
                    break;
                case "large":
                    width = 800;
                    height = 800;
                    break;
                case "meduim":
                    width = 400;
                    height = 400;
                    break;
                case "thumb":
                    width = 120;
                    height = 120;
                    break;
                default:
                    if (!size.Contains("x"))
                        return null;
                    string[] sizeParams = size.Split('x');
                    if (sizeParams.Length < 2)
                        return null;
                    if (!int.TryParse(sizeParams[0],out width)) return null;
                    if (!int.TryParse(sizeParams[1],out height)) return null;
                        
                    break;
            }

            using (MainEntities ctx = new MainEntities())
            {
                var imgs = ctx.tbl_images.Where(a => a.model_id == model_id && a.model_name == model && a.model_tag == model_tag).OrderBy(a => a.id).ToList();


                if (imgs==null || imgs.Count==0) goto noImageHandler;
                int vIndex = 0;
                if (int.TryParse(index,out vIndex))
                {

                }else if (index == "last")
                {
                    vIndex = imgs.Count - 1;
                }
                else
                {
                    vIndex = 0;
                }

                if (vIndex >= imgs.Count) goto noImageHandler;
                if (imgs.Count == 0) goto noImageHandler;
                var img = imgs.ToArray()[vIndex];
                  
                string filePath = img.original;
                    
                if (filePath == "") goto noImageHandler;

                filePath = ConfigurationManager.AppSettings["mediaServer_Path"] + filePath.Replace("/", "\\");

                if (!System.IO.File.Exists(filePath)) goto noImageHandler;
                
                imageHandler:

                var source=Image.FromFile(filePath);

                if (width * height == 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        source.Save(ms, ImageFormat.Png);
                        return base.File(ms.ToArray(), "image/jpeg");
                    }
                }
                Image target;
                if(mode == "crope")
                {

                    target=BLL.Images.ResizeImageKeepAspectRatio(source, width, height);
                    using (var ms = new MemoryStream())
                    {
                        target.Save(ms, ImageFormat.Png);
                        return base.File(ms.ToArray(), "image/jpeg");
                    }
                }
                else if(mode == "scale")
                {
                    target=BLL.Images.resize(source, width, height);
                    using (var ms = new MemoryStream())
                    {
                        target.Save(ms, ImageFormat.Png);
                        return base.File(ms.ToArray(), "image/jpeg");
                    }
                }

                using (var ms = new MemoryStream())
                {
                    source.Save(ms, ImageFormat.Png);
                    return base.File(ms.ToArray(), "image/jpeg");
                }
               
                     

            noImageHandler:

                if (System.IO.File.Exists(Server.MapPath("~/Content/imgs/no-image/" + model + "-"+model_tag+".png"))) {
                    filePath = Server.MapPath("~/Content/imgs/no-image/" + model + "-" + model_tag + ".png");
                    goto imageHandler;
                }
                if (System.IO.File.Exists(Server.MapPath("~/Content/imgs/no-image/any.png"))) {
                    filePath = Server.MapPath("~/Content/imgs/no-image/any.png");
                    goto imageHandler;
                }



            }

            return null;
        }
    }
}