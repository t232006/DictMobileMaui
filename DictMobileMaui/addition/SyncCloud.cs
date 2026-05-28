//using Android.App;
using GoogleDriveManipulation;
namespace IndDictionary.addition
{
	class SyncCloud
	{
		public static async Task<string> SaveToCloud(string secret, string WhereFrom, string filename)
		{
			var gu = await GoogleUploader.Upload(secret, WhereFrom, filename);
			return gu.output;
		}
		public static async Task LoadFromCloud (string secret, string id, string WhereTo)
		{
            var gu = GoogleDownloader.DownloadFile(secret, id, WhereTo);
            await gu;
        }
	}
}
