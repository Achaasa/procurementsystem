using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using procurementsystem.IService;

public class CloudinaryService : ICloudinaryService
{
    private Cloudinary _cloudinary;

    public CloudinaryService()
    {
        var account = new Account(
           Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
    Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
    Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
        );

        _cloudinary = new Cloudinary(account);
    }

    // Method to upload image to Cloudinary (accepts IFormFile)
    public (string imageUrl, string imageKey) UploadImage(IFormFile file, string category)
    {
        try
        {
            // Check if environment variables are set
            var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
            var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary environment variables are not configured. Please check CLOUDINARY_CLOUD_NAME, CLOUDINARY_API_KEY, and CLOUDINARY_API_SECRET.");
            }

            var fileStream = file.OpenReadStream();

            // Build folder path dynamically: e.g., "procurement/users"
            string rootFolder = "procurement";
            string folder = $"{rootFolder}/{category}";

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, fileStream),
                PublicId = Guid.NewGuid().ToString(),
                Folder = folder
            };

            var uploadResult = _cloudinary.Upload(uploadParams);

            if (uploadResult.StatusCode == HttpStatusCode.OK)
            {
                return (uploadResult.SecureUrl.AbsoluteUri, uploadResult.PublicId);
            }
            else
            {
                throw new InvalidOperationException($"Cloudinary upload failed with status: {uploadResult.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            Console.WriteLine($"Cloudinary upload error: {ex.Message}");
            throw;
        }
    }

    // Method to delete image from Cloudinary using public ID
    public bool DeleteImage(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var deletionResult = _cloudinary.Destroy(deleteParams);

        return deletionResult.StatusCode == HttpStatusCode.OK;
    }
}
