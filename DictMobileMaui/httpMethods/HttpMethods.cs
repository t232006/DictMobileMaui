using System.Diagnostics;
using System.Net.Http.Json;
using DictMobile.models;
using IndDictionary;

namespace DictMobile.httpMethods
{
    public class HttpMethods
    {
        const string POST_DICT_URL = "https://syncserver-739819639259.europe-west1.run.app/api/sync/push/dict";
        const string POST_TOPIC_URL = "https://syncserver-739819639259.europe-west1.run.app/api/sync/push/topic";
        const string GET_TOPIC_URL = "https://syncserver-739819639259.europe-west1.run.app/api/sync/pull/topic";
        const string GET_DICT_URL = "https://syncserver-739819639259.europe-west1.run.app/api/sync/pull/dict";
        
        private async Task<string> PostListAsync<T>(T DictOrTopic, string URL)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(URL, DictOrTopic); 
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                if (response.IsSuccessStatusCode)    
                    return $"Success! Applied {result.applied}";
                else
                    return result.ToString();
            }
        }
        public async Task<List<T>?> GetListAsync<T>(uint? DBID, string since)
        {
            using (var client = new HttpClient())
            {
                var url = typeof(T).Equals(typeof(DictVM)) ? $"{GET_DICT_URL}?DBID={DBID}&since={since}" :
                                            $"{GET_TOPIC_URL}?DBID={DBID}&since={since}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<T>>();
                    return result;
                }
                return null;
            }
        }
        public async Task<List<dict>> GetDictAsync(uint? DBID, string since)
        {
            List<DictVM>? ReceivedList = await GetListAsync<DictVM>(DBID, since);
            List<dict> Result = new();
            if (ReceivedList == null) return Result;
            foreach( var s in ReceivedList)
            {
                int? TopicId = App.Database.showTableTopic().FirstOrDefault(n => n.Name == s.TopicName)?.id;
                /*if (TopicId == null)
                {
                    App.Database.saveRecT(new topic { Name = s.TopicName });
                    Debug.WriteLine("11");
                    TopicId = App.Database.showTableTopic().FirstOrDefault(n => n.Name == s.TopicName)?.id;
                }*/
                if (App.Database.showTableTopic().FirstOrDefault(n => n.Name == s.TopicName).IsDeleted == true)
                    App.Database.showTableTopic().FirstOrDefault(n => n.Name == s.TopicName).IsDeleted = false;
                Result.Add(new dict
                {
                    DBID = DBID,
                    Word = s.Word,
                    Translation = s.Translation,
                    Topic = TopicId,
                    DateRec = s.DateRec,
                    Grade = s.Score,
                    Usersel = s.Usersel,
                    Phrase = s.Phrase,
                    Relevation = s.Relevation,
                    IsDeleted = s.IsDeleted,
                    Modification_Time = s.Modification_Time
                });        
            }
            return Result;
        }

        public async Task<string> PostTopicAsync(IEnumerable<topic> Topic)
        {
            List<topic> LTopic = [.. Topic]; //from IEnumerable to List
            return await PostListAsync<List<topic>>(LTopic, POST_TOPIC_URL);
        }
        public async Task<string> PostDictAsync(IEnumerable<DictVM> Dict)
        {
            List<DictVM> LDict = [.. Dict];
            return await PostListAsync<List<DictVM>>(LDict, POST_DICT_URL);
        }
       
        
    }
}
