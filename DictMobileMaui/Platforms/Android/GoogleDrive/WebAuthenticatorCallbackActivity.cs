using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Maui.Authentication;

namespace DictMobile.GoogleDrive.forAndroid
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTask, Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.mycompany.dictMobile",   // ← ваш Package Name
        DataPath = "/oauth2redirect"
    )]
    public class MyWebAuthenticatorCallbackActivity : WebAuthenticatorCallbackActivity
    {
        // Можно оставить пустым
    }
}