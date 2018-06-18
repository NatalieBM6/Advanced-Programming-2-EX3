using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    public interface IImageServiceModal
    {
        /************************************************************************
        *The Input: The path to add.
        *The Output: Success or error message.
        *The Function operation: The function returns the new path of the the file
        * if succeded.
        *************************************************************************/
        string AddFile(string path, out bool result);
    }
}

