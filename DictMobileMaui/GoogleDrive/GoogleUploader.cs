using Google.Apis.Drive.v3;
using Google.Apis.Upload;
using Google.Apis.Requests;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GoogleDriveManipulation
{
    public class GoogleUploader : GoogleHelper
    {
        public string output { get; private set; }

        GoogleUploader(string _token, string _filePath, string _fileName) : base(_token)
        {
        }
        public static async Task<GoogleUploader> Upload(string token, string filePath, string fileName)
        {
            var instance = new GoogleUploader(token, filePath, fileName);
            instance.output = "";
            try
            {
                await instance.Start();

                if (instance.driveService == null)
                    throw new InvalidOperationException("DriveService is not initialized.");

                var fullPath = Path.Combine(filePath, fileName);

                string mimeType = GetMimeTypeByExtension(Path.GetExtension(fileName));
                Debug.WriteLine($"GoogleUploader: mimeType = {mimeType}");

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    MimeType = mimeType
                };

                FilesResource.CreateMediaUpload request;
                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    request = instance.driveService.Files.Create(fileMetadata, stream, mimeType);
                    request.Fields = "id,name,mimeType";
                    // опционально: изменить chunk size для Android при необходимости
                    // request.ChunkSize = ResumableUpload.MinimumChunkSize * 2;

               
                    var progress = await request.UploadAsync();

                    if (progress.Status != UploadStatus.Completed)
                    {
                        throw new Exception($"Upload failed. Status: {progress.Status}, Exception: {progress.Exception?.Message}");
                    }
                }

                var file = request.ResponseBody;
                instance.output = file?.Name ?? "no filename";
            }
            catch (Exception e)
            {
                Debug.WriteLine($"GoogleUploader error: {e}");
                instance.output = e switch
                {
                    AggregateException _ => "Credential not found",
                    FileNotFoundException _ => "File not found",
                    Google.GoogleApiException gae => $"Google API error: {gae.Message}",
                    _ => $"Error: {e.Message}"
                };
            }
            return instance;
        }

        static string GetMimeTypeByExtension(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return "application/octet-stream";
            switch (ext.ToLowerInvariant())
            {
                case ".csv": return "text/csv";
                case ".txt": return "text/plain";
                case ".pdf": return "application/pdf";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".zip": return "application/zip";
                default: return "application/octet-stream";
            }
        }
    }
}