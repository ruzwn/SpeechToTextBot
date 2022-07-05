using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace SpeechToTextBot;

public class UpdateHandler : IUpdateHandler
{
    private readonly HttpClient _httpClient = new();

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        if (message == null)
        {
            return;
        }

        var voice = message.Voice;
        if (voice == null)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Send voice message please ...",
                cancellationToken: cancellationToken
            );

            return;
        }

        var fileInfo = await botClient.GetFileAsync(voice.FileId, cancellationToken);
        var requestUri = "https://api.telegram.org/file/bot" + Program.BotToken + $"/{fileInfo.FilePath}";
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        var fileBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        var text = await YandexAPI.GetTextOfAudio(fileBytes);

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: text,
            cancellationToken: cancellationToken);
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);

        return Task.CompletedTask;
    }
}
