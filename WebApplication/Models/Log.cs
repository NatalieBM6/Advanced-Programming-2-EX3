using Logging.Modal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Log
    {
        //members
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Enrty Type")]
        public MessageTypeEnum EntryType { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Enrty Type")]
        public String Status { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public enum EntryType
    {
        INFO,
        FAIL,
        WARNING
    }

}