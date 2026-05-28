using DictMobile.GoogleDrive.forAndroid;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;

namespace GoogleDriveManipulation
{
    public class GoogleHelper
    {
        private readonly string token;
        internal DriveService driveService;
        private UserCredential credentials;
        private bool forMobile;

        public GoogleHelper(string _token)
        {
            this.token = _token;
            forMobile = token == "client_secret_mobile.json";
        }
        public string ApplicationName { get => forMobile ? "MobileDictionary" : "DictionaryDB"; }
        public string[] Scopes { get; private set; } = new string[] { DriveService.Scope.Drive };

        internal async Task Start()
        {
            string credentialPath = Path.Combine(FileSystem.AppDataDirectory, ".credentials", ApplicationName);
            Directory.CreateDirectory(credentialPath);

            using (var stream = await FileSystem.OpenAppPackageFileAsync(token))
            {
                ClientSecrets cs = GoogleClientSecrets.FromStream(stream).Secrets;
                this.credentials = forMobile ? await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    cs,
                    Scopes,
                    user: "user",
                    taskCancellationToken: CancellationToken.None,
                    new FileDataStore(credentialPath, true),
                    codeReceiver: new AndroidCodeReceiver()
                    )
                    :
                    await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    //new[] {DriveService.ScopeConstants.DriveReadonly},
                    Scopes,
                    user: "user",
                    taskCancellationToken: CancellationToken.None,
                    new FileDataStore(credentialPath, true)
                    );

            }
            this.driveService = new DriveService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = this.credentials,
                ApplicationName = ApplicationName
            });
        }
    }
}
