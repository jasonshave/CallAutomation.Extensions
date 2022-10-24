# CallAutomation.Extensions

[![.NET](https://github.com/jasonshave/CallAutomation.Extensions/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jasonshave/CallAutomation.Extensions/actions/workflows/dotnet.yml) [![Nuget](https://img.shields.io/nuget/v/CallAutomation.Extensions.svg?style=flat)](https://www.nuget.org/packages/CallAutomation.Extensions/)

An extension library using the 'fluent API' style for the Azure Communication Services Call Automation platform making it easier to invoke actions and correlate callbacks to functions at runtime.

## Setup and configuration

In your `Startup.cs` or `Program.cs` (for .NET 6 or greater), simply add the following line of code.

```csharp
// .NET 6 or higher
builder.Services
    .AddCallAutomationClient(builder.Configuration["ConnectionString"])
    .AddCallAutomationEventHandling();
```

This will register the `CallAutomationClient` as a singleton in .NET's dependency injection container along with the necessary back-end services and a publisher `ICallAutomationEventPublisher` you can use to publish the Webhook callbacks.

## Handling the Webhook callbacks

The Azure Communication Services Call Automation platform uses Webhook callbacks to an HTTPS endpoint you specify when creating a call or answering a call. The format for these callbacks is a `CloudEvent` collection and this library includes a simple publisher used to locate the correct callback handler to invoke to correlate the previous action.

```csharp
// .NET 6 or higher minimal API example
app.MapPost("/api/callbacks", async (CloudEvent[] events, ICallAutomationEventPublisher publisher) => 
    await publisher.PublishAsync(events));
```

## Outbound calling

The outbound calling experience is greatly simplified into a series of chained methods which lead the developer to the correct conclusion all without having to 'new up' classes or figure out what requirements you need to meet.

### Calling an Azure Communication Services user

```csharp

// inject the CallAutomationClient
await client
        .Call("[ACS_USER_ID]")
        .From("[YOUR_APP_ID]")
        .WithCallbackUri("https://yourwebserver.com/api/callbacks")
        .OnCallConnected<ConnectionHandler>()
        .ExecuteAsync();
```

### Calling a PSTN number

Since calling to the PSTN requires you to specify a source caller ID for your application, the API permits setting this as an optional parameter.

```csharp

// inject the CallAutomationClient
await client
        .Call("+14255551212")
        .From("+18005551212", x =>
        {
            x.CallerDisplayName = "Contoso Airlines";
            x.ApplicationId = "[YOUR_APP_ID]";
        })
        .WithCallbackUri("https://yourwebserver.com/api/callbacks")
        .OnCallConnected<ConnectionHandler>()
        .ExecuteAsync();
```

## Playing audio

The Call Automation platform currently supports playing audio file from a URI. The file must be recorded in 16Khz, mono, and a WAV file format. In the following example, the `CallMedia` object can be obtained from any of the `On[EventName]` methods as shown in the detailed example below. Alternatively you can get the `CallMedia` object by using the `CallAutomationClient` and invoking `.GetCallConnection(connectionId).GetCallMedia()`.

```csharp
var callMedia = client.GetCallConnection(connectionId).GetCallMedia();

await callMedia
    .Play(x =>
    {
        x.FileUrl = holdMusic;
        x.Loop = true;
    })
    .ExecuteAsync();
```

## Outbound call then play audio

The following API sample assumes you've created a model for `CallRequest` which has a `string` for both the `Source` and `Destination` properties.

```csharp
app.MapPost("/api/call", async (CallRequest callRequest, CallAutomationClient client) =>
{
    await client
        .Call(callRequest.Destination)
        .From(callRequest.Source, x =>
        {
            x.CallerDisplayName = "Contoso Airlines";
            x.ApplicationId = applicationId;
        })
        .WithCallbackUri("https://yourwebserver.com/api/callbacks")
        .OnCallConnected(async (_, _, callMedia, _) =>
        {
            await callMedia
                .Play(x => x.FileUrl = holdMusic)
                .ExecuteAsync();
        })
        .ExecuteAsync();
});
```

## Collecting single-digit DTMF input and IVR menus

The library currently supports collecting single digit DTMF input only. Future support for this library to handle multi-tone DTMF input is coming soon. The base Call Automation SDK can be used to perform this type of recognition behavior.

You can use the `OnCallConnected` method to capture the delegate you wish to invoke when this behavior occurs. Additionally, you can do the same for things like `OnPlayCompleted`, and most importantly, when a person presses a key to emit a DTMF tone on the phone.

The following is a simple scenario involving the following:

1. Answer an incoming call
2. Play a greeting
3. Invoke the recognize API to collect a single digit while playing a prompt (Press 1 for English and 2 for Spanish)
4. Invoke the language-specific main menu prompt with DTMF collection (not shown here)

```csharp
await client.Answer(incomingCall)
    .WithCallbackUri("https://yourwebserver.com/api/callbacks")
    .OnCallConnected(async (callConnected, callConnection, callMedia, callRecording) =>
    {
        // play welcome prompt
        await callMedia.Play(x => x.FileUrl = _welcomePrompt)
            .OnPlayCompleted(async () =>
            {
                // language menu
                await callMedia
                    .ReceiveDtmfTone()
                    .FromParticipant(incomingCall.From.RawId)
                    .WithPrompt(_languageSelection)
                    .WithOptions(x =>
                    {
                        x.AllowInterruptExistingMediaOperation = true;
                        x.AllowInterruptPrompt = true;
                        x.WaitForResponseInSeconds = 5;
                    })
                    .OnPress<One, MainMenuEnglish>()
                    .OnPress<Two, MainMenuSpanish>()
                    .OnPress<Zero>(async () =>
                    {
                        await callConnection
                            .AddParticipant<CommunicationUserIdentifier>(["ACS_USER_ID"])
                            .ExecuteAsync();
                    })
                    .ExecuteAsync();
            })
            .ExecuteAsync();
    })
    .OnCallDisconnected(() =>
    {
        logger.LogInformation($"Call disconnected. CorrelationId: {incomingCall.CorrelationId}");
        return ValueTask.CompletedTask;
    })
    .ExecuteAsync();
```

## Custom event handlers

Instead of writing async or non-async method delegates to respones, you can handle the callbacks with your own event handlers by inheriting from `CallAutomationHandler` as follows:

```csharp
await client.Answer(incomingCall)
            .WithCallbackUri("https://yourwebserver.com/api/callbacks")
            .OnCallConnected<CustomHandler>() //<--custom handler
            .ExecuteAsync();
```

Use your custom handler is implemented and will receive the event, the `CallConnection`, `CallMedia`, and `CallRecording` objects automatically.

> NOTE: You will need to register your custom handler in .NET's dependency injection manually however a work item is in the backlog to perform this automatically for you.

```csharp
// remember to register your handler in Program.cs or Startup.cs
builder.Services.AddSingleton<CustomHandler>();
```

```csharp
public class CustomHandler : CallAutomationHandler
{
    public override ValueTask OnCallConnected(CallConnected @event, CallConnection callConnection, CallMedia callMedia, CallRecording callRecording)
    {
        // do work...
    }
}
```
