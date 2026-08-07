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
        private static readonly HttpClient client = new() { Timeout = TimeSpan.FromSeconds(15)};
        private class SyncResponse
        {
            public int applied { get; set; }
        }
        private async Task<httpResponce> PostListAsync<T>(T DictOrTopic, string URL)
        {
            httpResponce result = new();
            var json = System.Text.Json.JsonSerializer.Serialize(DictOrTopic);
            Debug.WriteLine($"REQUEST JSON:\n{json}");
            var response = await client.PostAsJsonAsync(URL, DictOrTopic); 
            var res = await response.Content.ReadFromJsonAsync<SyncResponse>();
            if (response.IsSuccessStatusCode)
            {
                result.success = true;
                if (typeof(T).Equals(typeof(List<DictVM>))) 
                    result.dictCount = res.applied;
                else 
                    result.topicCount = res.applied;
                return result;
            }
            else
            {
                result.message=String.Concat(response.StatusCode.ToString()," ",response.ReasonPhrase.ToString());
                return result;
            }
            
        }
        public async Task<List<T>?> GetListAsync<T>(uint? DBID, string since)
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
        public async Task<List<dict>> GetDictAsync(uint? DBID, string since)
        {
            List<DictVM>? ReceivedList = await GetListAsync<DictVM>(DBID, since);
            List<dict> Result = new();
            if (ReceivedList == null) return Result;
            foreach( var s in ReceivedList)
            {
                int? TopicId = App.Database.showTableTopic().FirstOrDefault(n => n.Name == s.TopicName)?.id;
                if (TopicId == null)
                {
                    App.Database.saveRecT(new topic { Name = s.TopicName });
                    Debug.WriteLine("===GetDictAsync===> no topic name. Creating one");
                    TopicId = App.Database.showTableTopic().First(n => n.Name == s.TopicName).id;
                }
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

        public async Task<httpResponce> PostTopicAsync(IEnumerable<topic> Topic)
        {
            List<topic> LTopic = [.. Topic]; //from IEnumerable to List
            httpResponce result = new();
            if (LTopic.Count > 0)
                result = await PostListAsync<List<topic>>(LTopic, POST_TOPIC_URL);
            else
            {
                result.topicCount = 0;
                result.success = true;
            }
            return result;
        }
        public async Task<httpResponce> PostDictAsync(IEnumerable<DictVM> Dict)
        {
            List<DictVM> LDict = [.. Dict];
            httpResponce result = new();
            if (LDict.Count > 0)
                result = await PostListAsync<List<DictVM>>(LDict, POST_DICT_URL);
            else
            {
                result.dictCount = 0;
                result.success = true;
            }
                
            return result;
        }
       
        
    }
}
