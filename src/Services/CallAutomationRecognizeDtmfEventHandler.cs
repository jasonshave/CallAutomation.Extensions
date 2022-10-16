// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using Microsoft.Extensions.Logging;

namespace CallAutomation.Extensions.Services;

public class CallAutomationRecognizeDtmfEventHandler : ICallAutomationRecognizeDtmfHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICallAutomationRecognizeEventDispatcher _dispatcher;
    private readonly CallAutomationClient _client;
    private readonly ILogger<CallAutomationRecognizeDtmfEventHandler> _logger;

    public CallAutomationRecognizeDtmfEventHandler(
        IServiceProvider serviceProvider,
        ICallAutomationRecognizeEventDispatcher dispatcher,
        CallAutomationClient client,
        ILogger<CallAutomationRecognizeDtmfEventHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _dispatcher = dispatcher;
        _client = client;
        _logger = logger;
    }

    public async ValueTask Handle(CallAutomationEventBase eventBase, string? requestId)
    {
        var clientElements = new CallAutomationClientElements(_client, eventBase.CallConnectionId);

        if (eventBase is RecognizeCompleted recognizeCompleted)
        {
            var callbackDelegate = CallbackRegistry.GetDtmfCallback<DtmfTone>(requestId, eventBase.GetType(), true);
            if (callbackDelegate is not null)
            {
                _logger.LogInformation("Found callback delegate for request {requestId}, with {numTones} DTMF tone(s), and event {event}", requestId, recognizeCompleted.CollectTonesResult.Tones.Count, eventBase.GetType());

                // need to determine if one or more tones are collected as we need to get a single
                // delegate for a single tone or get a single delegate for multiple tones
                if (recognizeCompleted.CollectTonesResult.Tones.Count is 1)
                {
                    var tone = recognizeCompleted.CollectTonesResult.Tones.FirstOrDefault();
                    var multicastDelegates = callbackDelegate.GetCallbacks(tone);
                    foreach (var multicastDelegate in multicastDelegates)
                    {
                        await _dispatcher.DispatchAsync(eventBase, multicastDelegate, clientElements, recognizeCompleted.CollectTonesResult.Tones);
                    }
                }
            }
        }

        if (eventBase is RecognizeFailed recognizeFailed)
        {
            var callbackDelegate = CallbackRegistry.GetDtmfCallback<Type>(requestId, eventBase.GetType(), true);
            if (callbackDelegate is not null)
            {
                _logger.LogInformation("Found callback delegate for request {requestId}, and event {event}", requestId, eventBase.GetType());

                var multicastDelegates = callbackDelegate.GetCallbacks(typeof(RecognizeFailed));
                foreach (var multicastDelegate in multicastDelegates)
                {
                    await _dispatcher.DispatchAsync(eventBase, multicastDelegate, clientElements);
                }
            }
        }
    }
}