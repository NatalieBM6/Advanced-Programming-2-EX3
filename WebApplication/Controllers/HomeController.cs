using Infrastructure.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

using Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using WebApplication.ServerCommunication;

namespace WebApplication.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class HomeController : Controller
    {

        private ImageWeb ImageWebModel;
        public string[] arrayOfNames;
        private bool wasInitiallized = false;

        /// <summary>
        /// Initializes the class.
        /// </summary>
        public void initClass()
        {
            if (wasInitiallized)
            {
                return;
            }
            arrayOfNames = new String[4];
            //setting default values to the array


            wasInitiallized = true;
            this.ImageWebModel = new ImageWeb();
            ImageWebModel.changeInModel += this.StudentIdsEvent;

            bool result = ImageWebModel.CheckConnection();

            ViewBag.ServerStatus = "True";
            if (!result)
            {
                ViewBag.ServerStatus = "False";
            }

        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            initClass();
           

            int amountImages = ImageWebModel.CountImagesInOutputDir();
            ViewBag.amountImages = amountImages;

            System.Threading.Thread.Sleep(500);



            return View();
        }

        /// <summary>
        /// Students the ids event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="IdEventArgs"/> instance containing the event data.</param>
        private void StudentIdsEvent(object sender, IdEventArgs e)
        {

            ViewBag.FirstName = e.FirstName;
            ViewBag.SecondName = e.SecondName;
            ViewBag.FirstId = e.FirstId;
            ViewBag.SecondId = e.SecondId;

         
        }
    }

}
 