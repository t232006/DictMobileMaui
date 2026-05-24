using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleDriveManipulation
{
    public class GoogleHelper
    {
        private readonly string token;
        internal DriveService driveService;
        private UserCredential credentials;

        public GoogleHelper(string _token)
        {
            this.token = _token;
        }

        public string ApplicationName { get; private set; } = "DictionaryDB";
        public string[] Scopes { get; private set; } = new string[] { DriveService.Scope.Drive };

        internal async Task Start()
        {
            string credentialPath = Path.Combine(Environment.CurrentDirectory, ".credentials", ApplicationName);
            using (var stream = new FileStream(token, FileMode.Open, FileAccess.Read))
            {
                this.credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    //new[] {DriveService.ScopeConstants.DriveReadonly},
                    Scopes,
                    user: "user",
                    taskCancellationToken: CancellationToken.None,
                    new FileDataStore(credentialPath, true));
            }
            this.driveService = new DriveService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = this.credentials,
                ApplicationName = ApplicationName
            });
        }
    }
}
