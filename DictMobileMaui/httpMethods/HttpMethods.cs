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
        private async Task<object?> PostTopicList(List<topic> Topic)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(POST_TOPIC_URL, Topic);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return result; 
            }  
        }
        private async Task<object?> PostDictList(List<DictVM> Dict)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(POST_DICT_URL, Dict);
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                return result;
            }
        }

        public async Task<List<topic>?> GetTopicAsync(int DBID, DateTime since)
        {
            using (var client = new HttpClient())
            {
                var url = $"{GET_TOPIC_URL}?DBID={DBID}&since={since:O}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<topic>>();
                    return result;
                }
                return null;
            }
        }

        private async Task<List<DictVM>?> GetFullDictAsync(uint DBID, DateTime since)
        {
            using (var client = new HttpClient())
            {
                var url = $"{GET_DICT_URL}?DBID={DBID}&since={since:O}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<DictVM>>();
                    return result;
                }
                return null;
            }
        }
        public async Task<List<dict>> GetDictAsync(uint DBID, DateTime since)
        {
            List<DictVM>? ReceivedList = await GetFullDictAsync(DBID, since);
            List<dict> Result = new();
            if (ReceivedList == null) return Result;
            foreach( var s in ReceivedList)
            {
                int? TopicId = App.Database.showTableTopic().FirstOrDefault(n => n.Name == s.TopicName).id;
                if (TopicId == null)
                {
                    App.Database.saveRecT(new topic { Name = s.TopicName });
                    TopicId = App.Database.showTableTopic().FirstOrDefault(n => n.Name == s.TopicName).id;
                }
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

        public async Task<object?> PostTopicAsync(IEnumerable<topic> Topic)
        {
            List<topic> LTopic = [.. Topic]; //from IEnumerable to List
            return PostTopicList(LTopic);
        }
        public async Task<object?> PostDictAsync(IEnumerable<DictVM> Dict)
        {
            List<DictVM> LDict = [.. Dict];
            return PostDictList(LDict);
        }
        public object? PostTopic(IEnumerable<topic> Topic)
        {
            List<topic> LTopic = [.. Topic];
            return PostTopicList(LTopic);
        }
        public object? PostDict(IEnumerable<DictVM> Dict)
        {
            List<DictVM> LDict = [.. Dict];
            return PostDictList(LDict);
        }
    }
}
