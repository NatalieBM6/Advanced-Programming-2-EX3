using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class PhotoModel
    {

        private string m_outputDir;

        private List<Photo> photosList = new List<Photo>();

        [Display(Name = "Photos")]
        public List<Photo> Photos { get => photosList; set => photosList = value; }

        [DataType(DataType.Text)]
        [Display(Name = "Thumbnails")]
        public ObservableCollection<Photo> Thumbnails { get; set; }

        /// <summary>
        /// Fills the list with photos from the directory.
        /// </summary>
        /// <param name="path">Path of parent directory.</param>
        /// <param name="size">size of thumbnails.</param>
        public void GetPhotos(string path, int size)
        {
            photosList.Clear();
            Thumbnails = new ObservableCollection<Photo>();

            string thumbPath = path + "\\thumbnails";
            int id = 0;
            if (!Directory.Exists(path) || !Directory.Exists(thumbPath)) return;
            DirectoryInfo directory = new DirectoryInfo(path);

            List<DirectoryInfo> yearDirs = directory.GetDirectories().ToList();
            foreach (DirectoryInfo year in yearDirs)
            {
                List<DirectoryInfo> monthDirs = year.GetDirectories().ToList();
                foreach (DirectoryInfo month in monthDirs)
                {
                    List<FileInfo> photos = month.GetFiles().ToList();
                    foreach (FileInfo photo in photos)
                    {
                        string yearString = year.Name;
                        string monthString = month.Name;
                        string relativePath = year.Parent.Name +
                            "\\" + yearString + "\\" + monthString + "\\" + photo.Name;
                        string thumbRelPath = year.Parent.Name +
                            "\\thumbnails\\" + yearString + "\\" + monthString + "\\" + photo.Name;
                        string fullPath = photo.FullName;
                        string fullThumbPath = fullPath.Replace(relativePath, thumbRelPath);

                        photosList.Add(new Photo(photo.Name, relativePath, thumbRelPath, fullPath, fullThumbPath,
                            yearString, monthString, size, id++));

                        Thumbnails.Add(new Photo(photo.Name, relativePath, thumbRelPath, fullPath, fullThumbPath,
                            yearString, monthString, size, id++));

                    }
                }

            }
        }
    }
}