namespace SpeechToTextBot;

public static class YandexAPI
{
    private static readonly HttpClient client = new()
    {
        BaseAddress = new Uri("https://stt.api.cloud.yandex.net/speech/v1/stt:recognize?topic=general&lang=ru-RU&folderId=b1gn2tudjjpvhi9a7ncm")
    };

    public static async Task<string> GetTextOfAudio(string fileName)
    {
        // To do automatic getting access token?
        const string accessToken = "t1.9euelZqWyseazo_OyY6OyM6LkpLMle3rnpWal86Ym8vGi82czI2QyMfMlcjl8_drdSJq-e8oUmoJ_t3z9yskIGr57yhSagn-.YnfYZyrewhXrPfBdYVQbI1Jjtq2z21N74RaIOdTM34TiTDaO2PYBARjmUlRHt-t_AioNn91ETD-3X8087_ZkCQ";

        using var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.Headers.Add("Authorization", "Bearer " + accessToken);

        await using var fileStream = File.OpenRead(fileName);
        using var content = new MultipartFormDataContent
        {
            {new StreamContent(fileStream)}
        };

        request.Content = content;

        var response = await client.SendAsync(request);

        return await response.Content.ReadAsStringAsync();
    }
}
