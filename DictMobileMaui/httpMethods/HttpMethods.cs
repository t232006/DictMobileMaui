using System.Net.Http.Json;
using DictMobile.models;

namespace DictMobile.httpMethods
{
    public class HttpMethods
    {
        const string POST_DICT_URL = "https://syncserver-739819639259.europe-west1.run.app/api/sync/push/dict";
        const string POST_TOPIC_URL = "https://syncserver-739819639259.europe-west1.run.app/api/sync/push/topic";
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
        public async Task<object?> PostTopicAsync(IEnumerable<topic> Topic)
        {
            List<topic> LTopic = [.. Topic];
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
