using Telegram.Bot;
using Telegram.Bot.Polling;

namespace SpeechToTextBot;

internal static class Program
{
	private static async Task Main(string[] args)
	{
		var botClient = new TelegramBotClient("TOKEN");

		var me = await botClient.GetMeAsync();
		Console.WriteLine($"Id: {me.Id}; Name: {me.Username}");

		var handler = new UpdateHandler();
		var receiverOptions = new ReceiverOptions();
		var cts = new CancellationTokenSource();
		botClient.StartReceiving(handler, receiverOptions, cancellationToken: cts.Token);

		Console.ReadLine();

		cts.Cancel();
	}
}
