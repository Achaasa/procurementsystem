using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace procurementsystem.IService
{
    public interface ICloudinaryService
    {

        (string imageUrl, string imageKey) UploadImage(IFormFile file, string category);
        public bool DeleteImage(string publicId);
    }
}