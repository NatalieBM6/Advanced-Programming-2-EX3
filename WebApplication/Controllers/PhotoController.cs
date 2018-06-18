using Infrastructure.Event;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class PhotoController : Controller
    {

        public static PhotoModel photos = new PhotoModel();
        private ConfigModel ConfigModel = new ConfigModel();

        private static string m_OutPutDir;
        private static string m_ThumbnailSize;

        private Photo m_currentPhoto;


        // GET: Photo
        /// <summary>
        /// Photoses this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Photos()
        {
            //default settings;
            string outputDir; 
            outputDir = AppDomain.CurrentDomain.BaseDirectory;
            outputDir += "OutPutImages";
            m_OutPutDir = outputDir;

            ConfigModel.changeInModel += this.updateSettings;

            ViewBag.Message = "Your Photos page.";


            photos.Photos.Clear();
            System.Threading.Thread.Sleep(1500);
            photos.GetPhotos(m_OutPutDir, Int32.Parse(m_ThumbnailSize));

            return View(photos);
        }


        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SettingsEventArgs"/> instance containing the event data.</param>
        private void updateSettings(object sender, SettingsEventArgs e)
        {
            m_OutPutDir = e.OutputDir;
            m_ThumbnailSize = e.ThumbnailSize;
        }

        /// <summary>
        /// Deletes the photo.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="thumbnail">The thumbnail.</param>
        /// <returns></returns>
        public ActionResult DeletePhoto(string image, string thumbnail)
        {

            System.IO.File.Delete(image);
            System.IO.File.Delete(thumbnail);

            photos.Thumbnails.Remove(m_currentPhoto);


            return RedirectToAction("Photos");
        }


        /// <summary>
        /// Fulls the screen image view.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <returns></returns>
        public ActionResult FullScreenImageView(string picture)
        {
            foreach (Photo item in photos.Thumbnails)
            {
                bool result = (item.RelativePath).Equals(picture);
                if (result == true)
                {
                
                    m_currentPhoto = item;
                }
            }

            return View(m_currentPhoto);
        }

        /// <summary>
        /// Deletes the image view.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <returns></returns>
        public ActionResult DeleteImageView(string picture)
        {
            foreach (Photo item in photos.Thumbnails)
            {
                bool result = (item.RelativePath).Equals(picture);
                if (result == true)
                {
                    m_currentPhoto = item;
                }
            }

            return View(m_currentPhoto);
        }



    }
}