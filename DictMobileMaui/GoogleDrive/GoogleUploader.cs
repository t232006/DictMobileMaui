using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleDriveManipulation
{
    public class GoogleUploader : GoogleHelper
    {
        public string output;
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
                    request.Upload();
                }
                var file = request.ResponseBody;
                instance.output = file.Id;
            }
            catch (Exception e)
            {
                if (e is AggregateException)
                {
                    instance.output = "Credential not found";
                }
                if (e is FileNotFoundException)
                {
                    instance.output = "File not found";
                }
            }
            return instance;
        }
    }
}
