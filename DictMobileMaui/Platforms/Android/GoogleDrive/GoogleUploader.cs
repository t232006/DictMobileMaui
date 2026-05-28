using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.Text;

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
            GoogleUploader instance = new GoogleUploader(token, filePath, fileName);
            instance.output = "";
            try
            {
                await instance.Start();
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    MimeType = "application / octet - stream"
                };
                FilesResource.CreateMediaUpload request;
                using (var stream = new FileStream(Path.Combine(filePath, fileName), FileMode.Open))
                {
                    request = instance.driveService.Files.Create(fileMetadata, stream, "text/csv");
                    request.Fields = "id";
                    await request.UploadAsync();
                }
                var file = request.ResponseBody;
                instance.output = file?.OriginalFilename ?? "no file Id";
            }
            catch (Exception e)
            {
                instance.output = e switch
                {
                    AggregateException => "Credential not found",
                    FileNotFoundException => "File not found",
                    _ => $"Error: {e.Message}"
                };
                
            }
            return instance;
        }
    }
}
