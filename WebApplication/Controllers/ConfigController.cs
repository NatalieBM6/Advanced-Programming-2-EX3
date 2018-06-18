using Infrastructure.Event;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class ConfigController : Controller
    {
        private ConfigModel ConfigModel = new ConfigModel();
        string[] handlers;
        private string m_toBeDeletedHandler;

        // GET: Config
        public ActionResult Config()
        {
            ConfigModel.changeInModel += this.updateSettings;

            ViewBag.Message = "Your Log page.";

            System.Threading.Thread.Sleep(1000);
            return View(ConfigModel);
        }


        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SettingsEventArgs"/> instance containing the event data.</param>
        private void updateSettings(object sender, SettingsEventArgs e)
        {

            ViewBag.OutPutDir = e.OutputDir;
            ViewBag.SourceName = e.SourceName;
            ViewBag.LogName = e.LogName;
            ViewBag.ThumbnailSize = e.ThumbnailSize;
            handlers = e.Handlers;

        }


        /// <summary>
        /// Deletes the Handler.
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteOK()
        {
            //delete the handler

            ConfigModel.RemoveHandler(m_toBeDeletedHandler);
            Thread.Sleep(500);
            return RedirectToAction("Config");

        }

        /// <summary>
        /// Deletes the handler.
        /// </summary>
        /// <param name="toBeDeletedHandler">To be deleted handler.</param>
        /// <returns></returns>
        public ActionResult DeleteHandler(string toBeDeletedHandler)
        {
            Thread.Sleep(500);

            foreach (string item in ConfigModel.Handlers)
            {
                bool result = item.Equals(toBeDeletedHandler);
                if (result == true)
                {
                    m_toBeDeletedHandler = item;
                    this.ConfigModel.ToBeDeletedHandler = m_toBeDeletedHandler;
                }
            }

            return View(ConfigModel);
        }

    }

}