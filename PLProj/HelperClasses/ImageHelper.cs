using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace PLProj.HelperClasses
{
    //  refactoring
    //   Duplicated code in unrelated classes
    //  May need to Extract Class or otherwise eliminate one of the versions
    public static class ImageHelper
    {
        public static string SaveImage(IFormFile imageFile, IWebHostEnvironment hostEnvironment, string FolderName)
        {
            var webRootPath = hostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadPath = Path.Combine(webRootPath, "Images", FolderName, "uploads");
            Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            return $"Images/{FolderName}/uploads/{fileName}";
        }

        public static void DeleteImage(string imageUrl, IWebHostEnvironment hostEnvironment, string FolderName)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var webRootPath = hostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var filePath = Path.Combine(webRootPath, "Images",FolderName, "uploads", Path.GetFileName(imageUrl));

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
