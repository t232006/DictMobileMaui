using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Maui.Authentication;

namespace GoogleDriveManipulationAndroid
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTask, Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.mycompany.dictmobile",   
        DataPathPrefix = "/oauth2redirect"
    )]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.mycompany.dictmobile",
        DataHost = "oauth2redirect"
    )]
    public class MyWebAuthenticatorCallbackActivity : WebAuthenticatorCallbackActivity
    {
        // Можно оставить пустым
    }
}