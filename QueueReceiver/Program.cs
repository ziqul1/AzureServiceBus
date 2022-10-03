using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueueReceiver;
using QueueSenderAPI.Models;
using System.Text.Json;

string _connectionString = "";
string _queueName = "";

ServiceBusClient client = new ServiceBusClient(_connectionString);
ServiceBusProcessor processor = client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

static async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");
    var deserializedUser = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(body);
    await args.CompleteMessageAsync(args.Message);

    var _userContext = new UserContext();
    var user = await _userContext.Users.Where(x => x.Id == deserializedUser.Id).FirstOrDefaultAsync();
    var validator = new UserValidator();
    var result = validator.Validate(deserializedUser);

    if (result.IsValid)
        user.IsFlagActive = true;

    _userContext.SaveChangesAsync();
    Console.WriteLine("Flag change complete. Press any key to continue...");
    Console.ReadKey();
}

static Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

try
{
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    await processor.StartProcessingAsync();
    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();

    Console.WriteLine("\nStopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
    Console.ReadKey();
}
finally
{
    await processor.DisposeAsync();
    await client.DisposeAsync();

}