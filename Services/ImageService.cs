using ContactBook.Services.Interfaces;

namespace ContactBook.Services
{
    public class ImageService : IImageService
    {
        private readonly string[] suffixes = {"Bytes", "KB", "MB", "GB", "TB", "PB"};
        private readonly string defaultImage = "/images/profile-pic-default.svg";
        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            if (fileData is null) return defaultImage;

            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData);

                return string.Format($"data:{extension};base64,{imageBase64Data}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}