using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class QnAMakerService
{
    private string qnaServiceHostName;
    private string knowledgeBaseId;
    private string endpointKey;

    public QnAMakerService(string hostName, string kbId, string endpointkey)
    {
        qnaServiceHostName = hostName;
        knowledgeBaseId = kbId;
        endpointKey = endpointkey;

    }
    async Task<string> Post(string uri, string body)
    {
        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage())
        {
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(uri);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", "EndpointKey " + endpointKey);

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
    public async Task<string> GetAnswer(string question)
    {
        string uri = qnaServiceHostName + "/qnamaker/knowledgebases/" + knowledgeBaseId + "/generateAnswer";
        string questionJSON = "{\"question\": \"" + question.Replace("\"", "'") + "\"}";

        var response = await Post(uri, questionJSON);

        var answers = JsonConvert.DeserializeObject<QnAAnswer>(response);
        if (answers.answers.Count > 0)
        {

            return answers.answers[0].answer;
        }
        else
        {
            return "No good match found.";
        }
    }

        public class Metadata
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Answer
    {
        public IList<string> Questions { get; set; }
        public string answer { get; set; }
        public double Score { get; set; }
        public int Id { get; set; }
        public string Source { get; set; }
        public IList<object> Keywords { get; set; }
        public IList<Metadata> Metadata { get; set; }
    }

    public class QnAAnswer
    {
        public IList<Answer> answers { get; set; }
    }
}
