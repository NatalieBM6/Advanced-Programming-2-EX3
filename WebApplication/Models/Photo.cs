using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Photo
    {
        /// <summary>
        /// Constructor for photo.
        /// </summary>
        /// <param name="name">Name of the photo.</param>
        /// <param name="relPath">Relative path of the photo.</param>
        /// <param name="relThumbPath">Relative of the thumbnail photo.</param>
        /// <param name="fullPath">Full path of the photo.</param>
        /// <param name="fullThumb">Full path of the thumbnail photo.</param>
        /// <param name="year">Year the photo was taken.</param>
        /// <param name="month">Month the photo was taken.</param>
        /// <param name="size">Size of the photo.</param>
        /// <param name="id">ID of the photo.</param>
        public Photo(string name, string relPath, string relThumbPath, string fullPath,
            string fullThumb, string year, string month, int size, int id)
        {
            this.name = name;
            this.Path = relPath;
            this.ThumbPath = relThumbPath;
            this.FullPath = fullPath;
            this.FullThumbPath = fullThumb;
            this.year = year;
            this.month = month;
            this.size = size;
            this.id = id;

           

        }

        private string name;
        private string path;
        private string thumbPath;
        private string fullPath;
        private string fullThumbPath;
        private string year;
        private string month;
        private int size;
        private int id;

        private string relativeThumbPath;
        private string relativePath;


        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get => name; set => name = value; }

        public int Size { get => size; set => size = value; }

        [Required]
        [Display(Name = "ID")]
        public int ID { get => this.id; set => this.id = value; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Month")]
        public string Month { get => month; set => month = value; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Year")]
        public string Year { get => year; set => year = value; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ThumbPath")]
        public string ThumbPath { get => thumbPath; set => thumbPath = value; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Path")]
        public string Path { get => path; set => path = value; }

        public string FullPath { get => fullPath; set => fullPath = value; }
        public string FullThumbPath { get => fullThumbPath; set => fullThumbPath = value; }
        public string RelativeThumbPath { get => relativeThumbPath; set => relativeThumbPath = value; }
        public string RelativePath { get => relativePath; set => relativePath = value; }

        /// <summary>
        /// Absolutes to relative path.
        /// </summary>
        /// <param name="pathToFile">The path to file.</param>
        /// <param name="referencePath">The reference path.</param>
        /// <returns></returns>
        public static string AbsoluteToRelativePath(string pathToFile, string referencePath)
        {
            Uri fileUri = new Uri(pathToFile);
            if (fileUri == null) {
                return "";
            }
            Uri referenceUri = new Uri(referencePath);

            if (referenceUri == null)
            {
                return "";
            }

            return referenceUri.MakeRelativeUri(fileUri).ToString();
          
        }

        /// <summary>
        /// Gets the creation date.
        /// </summary>
        /// <param name="aPhoto">a photo.</param>
        /// <returns></returns>
        public string GetCreationDate(Photo aPhoto)
        {

            string Date = "";
            Date += aPhoto.Month + "-";
            Date += aPhoto.Year;
            return Date;
        }
    }
}