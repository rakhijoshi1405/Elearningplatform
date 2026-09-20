using System;
using System.IO;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Elearningplatform.Utilities
{
    public static class VideoUploader
    {
        private static readonly string[] AllowedVideoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm" };
        private static readonly long MaxFileSize = 500 * 1024 * 1024; // 500MB

        public static VideoUploadResult UploadVideo(HttpPostedFileBase videoFile, string uploadPath)
        {
            var result = new VideoUploadResult();

            try
            {
                if (videoFile == null || videoFile.ContentLength == 0)
                {
                    result.ErrorMessage = "No file uploaded.";
                    return result;
                }

                if (videoFile.ContentLength > MaxFileSize)
                {
                    result.ErrorMessage = "File size exceeds the maximum limit of 500MB.";
                    return result;
                }

                var fileExtension = Path.GetExtension(videoFile.FileName);
                if (!string.IsNullOrEmpty(fileExtension))
                {
                    fileExtension = fileExtension.ToLower();
                }
                if (string.IsNullOrEmpty(fileExtension) || !AllowedVideoExtensions.Contains(fileExtension))
                {
                    result.ErrorMessage = "Invalid file type.";
                    return result;
                }

                var videosPath = Path.Combine(uploadPath, "Videos");
                var thumbnailsPath = Path.Combine(uploadPath, "Thumbnails");

                Directory.CreateDirectory(videosPath);
                Directory.CreateDirectory(thumbnailsPath);

                var fileName = Path.GetFileNameWithoutExtension(videoFile.FileName);
                var uniqueFileName = string.Format("{0}_{1}{2}", fileName, Guid.NewGuid(), fileExtension);
                var videoFilePath = Path.Combine(videosPath, uniqueFileName);

                videoFile.SaveAs(videoFilePath);

                var thumbnailFileName = Path.ChangeExtension(uniqueFileName, ".jpg");
                var thumbnailPath = Path.Combine(thumbnailsPath, thumbnailFileName);

                GeneratePlaceholderThumbnail(thumbnailPath);

                result.IsSuccess = true;
                result.VideoFileName = uniqueFileName;
                result.VideoFilePath = string.Format("/Content/Uploads/Videos/{0}", uniqueFileName);
                result.ThumbnailPath = string.Format("/Content/Uploads/Thumbnails/{0}", thumbnailFileName);
                result.FileSize = videoFile.ContentLength;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = string.Format("Upload failed: {0}", ex.Message);
            }

            return result;
        }

        private static void GeneratePlaceholderThumbnail(string thumbnailPath)
        {
            try
            {
                using (var bitmap = new Bitmap(320, 180))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.LightGray);
                    graphics.DrawRectangle(Pens.DarkGray, 0, 0, 319, 179);
                    var font = new Font("Arial", 12);
                    var brush = Brushes.DarkGray;
                    graphics.DrawString("Video Thumbnail", font, brush, 80, 80);
                    bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
                }
            }
            catch
            {
                // Continue without thumbnail if generation fails
            }
        }
    }

    public class VideoUploadResult
    {
        public bool IsSuccess { get; set; }
        public string VideoFileName { get; set; }
        public string VideoFilePath { get; set; }
        public string ThumbnailPath { get; set; }
        public long FileSize { get; set; }
        public int Duration { get; set; }
        public string ErrorMessage { get; set; }
    }
}