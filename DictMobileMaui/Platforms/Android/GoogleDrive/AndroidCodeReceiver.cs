using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Maui.Authentication;
using System.Diagnostics;

namespace GoogleDriveManipulationAndroid;

public class AndroidCodeReceiver : ICodeReceiver
{
    // Должен полностью совпадать с тем, что указали в Google Cloud Console
    public string RedirectUri { get; } = "com.mycompany.dictmobile:/oauth2redirect";

    public async Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync(
        AuthorizationCodeRequestUrl url,
        CancellationToken cancellationToken)
    {
        try
        {
            var authUrl = url.Build().ToString();

            Debug.WriteLine($"Auth URL: {authUrl}"); // ← проверьте redirect_uri в строке запроса
            Debug.WriteLine($"Expected redirect URI: {RedirectUri}");

            // Открываем браузер и ждём callback
            var result = await WebAuthenticator.AuthenticateAsync(
                new Uri(authUrl),
                new Uri(RedirectUri));

            // Преобразуем результат в нужный формат для Google
            return new AuthorizationCodeResponseUrl(result.Properties);
        }
        catch (Exception ex)
        {
            throw new Exception($"WebAuthenticator error: {ex.Message}", ex);
        }
    }
}