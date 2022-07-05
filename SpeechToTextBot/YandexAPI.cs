using Newtonsoft.Json;

namespace SpeechToTextBot;

public static class YandexAPI
{
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri("https://stt.api.cloud.yandex.net/speech/v1/stt:recognize?lang=ru-RU&topic=general&format=oggopus&folderId=b1gn2tudjjpvhi9a7ncm")
    };

    public static async Task<string> GetTextOfAudio(byte[] fileBytes)
    {
        // To do automatic getting access token?
        const string accessToken = "t1.9euelZrInJGLjZWKyJyRipyTjJzNyO3rnpWal86Ym8vGi82czI2QyMfMlcjl8_ducm9p-e9sOml7_N3z9y4hbWn572w6aXv8.zSLsDuNUU5c19kDtTmL8O_Jlu0NKZsiAbaK9G-8QLtXwJL3DF_x4gmzjRsbs3ml9kSN3RpQ6oQoK4zqVvRmKCg";
        
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        request.Content = new ByteArrayContent(fileBytes);

        var response = await Client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseBodyAsDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);

        
        if (responseBodyAsDict != null && responseBodyAsDict.ContainsKey("result"))
        {
            return responseBodyAsDict["result"];
        }

        return "An error has occured";
    }
}
