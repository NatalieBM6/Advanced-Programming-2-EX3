using Infrastructure.Enums;
using Infrastructure.Event;
using Logging.Modal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class LogsController : Controller
    {
        private LogsModel LogsModel = new LogsModel();


        // GET: Logs
        /// <summary>
        /// Logses this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Logs()
        {
            ViewBag.Message = "Log page.";

            System.Threading.Thread.Sleep(1000);

            return View(LogsModel.LogEntries);
        }


        /// <summary>
        /// Logses the specified form.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Logs(FormCollection form)
        {
            string type = form["typeFilter"].ToString();
            if (type == "")
            {
                return View(LogsModel.LogEntries);
            }
            List<Log> filteredLogsList = new List<Log>();
            foreach (Log log in LogsModel.LogEntries)
            {
                if (log.Status == type)
                {
                    filteredLogsList.Add(log);
                }
            }
            return View(filteredLogsList);

        }

    }
}