using GoogleDriveManipulator;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IndDictionary.addition
{
	class SyncCloud
	{
		public static async Task SaveToCloud(string secret, string WhereFrom, string filename)
		{
			var gu = GoogleUploader.Upload(secret, WhereFrom, filename);
			await gu;
		}
		public static void LoadFromCloud (string secret, string id, string WhereTo)
		{
			var gu = GoogleDownloader.DownloadList(secret).Result;
			gu.DownloadFile(id, WhereTo);
		}
	}
}
