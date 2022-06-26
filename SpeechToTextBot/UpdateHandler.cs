using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace SpeechToTextBot;

public class UpdateHandler : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Check that Message and Voice
        Message? message = update?.Message;
        if (message == null)
        {
            return;
        }

        Voice? voice = message?.Voice;
        if (voice == null)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Send voice message please ...",
                cancellationToken: cancellationToken
            );

            return;
        }

        // Save received file
        Directory.CreateDirectory("./voices");
        var fileName = $"./voices/voice-{voice.FileId}.mp3";
        Telegram.Bot.Types.File fileInfo = await botClient.GetFileAsync(voice.FileId, cancellationToken: cancellationToken);
        await using (var fileStream = System.IO.File.Create(fileName))
        {
            await botClient.DownloadFileAsync(
                filePath: fileInfo.FilePath,
                destination: fileStream,
                cancellationToken: cancellationToken
            );
        }

        // Send audio to Yandex API ...        ??? GRPC ???
        var textOfAudio = await YandexAPI.GetTextOfAudio(fileName);

        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: $"Voice succesfully sent and saved. You said: {textOfAudio}",
            cancellationToken: cancellationToken
        );
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
