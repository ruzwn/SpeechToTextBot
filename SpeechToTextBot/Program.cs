using Telegram.Bot;
using Telegram.Bot.Polling;

namespace SpeechToTextBot;

internal static class Program
{
	public const string BotToken = "Token";

	private static void Main()
	{
		var botClient = new TelegramBotClient(BotToken);
		var handler = new UpdateHandler();
		var receiverOptions = new ReceiverOptions();
		var cts = new CancellationTokenSource();
		
		botClient.StartReceiving(handler, receiverOptions, cancellationToken: cts.Token);

		Console.ReadLine();
		cts.Cancel();
	}
}
