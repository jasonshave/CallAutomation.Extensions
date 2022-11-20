// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Extensions;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;
using Microsoft.Extensions.Logging;

namespace CallAutomation.Extensions.Handlers;

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

    public async ValueTask Handle(CallAutomationEventBase eventBase, IOperationContext context)
    {
        var clientElements = new CallAutomationClientElements(_client, eventBase.CallConnectionId);

        if (eventBase is RecognizeCompleted recognizeCompleted)
        {
            var tone = recognizeCompleted.CollectTonesResult.Tones.FirstOrDefault();

            var callAutomationHelperCallback = CallbackRegistry.GetHelperCallback(context.RequestId, typeof(RecognizeCompleted), true);
            if (callAutomationHelperCallback is not null)
            {
                // need to determine if one or more tones are collected as we need to get a single
                // delegate for a single tone or get a single delegate for multiple tones
                if (recognizeCompleted.CollectTonesResult.Tones.Count is 1)
                {
                    // dispatch delegate callbacks
                    var delegates = callAutomationHelperCallback.GetDelegateCallbacks(tone.Convert().GetType());
                    foreach (var @delegate in delegates)
                    {
                        _logger.LogInformation("Found callback delegate for request {requestId}, with {numTones} DTMF tone(s), and event {event}", context.RequestId, recognizeCompleted.CollectTonesResult.Tones.Count, eventBase.GetType().Name);
                        await _dispatcher.DispatchAsync(recognizeCompleted, @delegate, clientElements, recognizeCompleted.CollectTonesResult.Tones);
                    }

                    var handlerTuples = callAutomationHelperCallback.GetHandlers(tone.Convert().GetType());
                    foreach (var handlerTuple in handlerTuples)
                    {
                        var handler = _serviceProvider.GetService(handlerTuple.Item2);

                        if (handler is null) return;

                        _logger.LogInformation("Found callback handler for request {requestId} and event {event}", context.RequestId, eventBase.GetType());
                        await _dispatcher.DispatchAsync(recognizeCompleted, handlerTuple.Item1, handler, clientElements, recognizeCompleted.CollectTonesResult.Tones);
                    }
                }
            }
        }

        if (eventBase is RecognizeFailed recognizeFailed)
        {
            var callAutomationHelperCallback = CallbackRegistry.GetHelperCallback(context.RequestId, recognizeFailed.ReasonCode.Convert().GetType(), true);
            if (callAutomationHelperCallback is not null)
            {
                // dispatch delegate callbacks
                var delegates = callAutomationHelperCallback.GetDelegateCallbacks(recognizeFailed.ReasonCode.Convert().GetType());
                foreach (var @delegate in delegates)
                {
                    _logger.LogInformation("Found callback delegate for request {requestId}, and event {event}", context.RequestId, eventBase.GetType().Name);
                    await _dispatcher.DispatchAsync(recognizeFailed, @delegate, clientElements);
                }

                var handlerTuples = callAutomationHelperCallback.GetHandlers(recognizeFailed.ReasonCode.Convert().GetType());
                foreach (var handlerTuple in handlerTuples)
                {
                    var handler = _serviceProvider.GetService(handlerTuple.Item2);

                    if (handler is null) return;

                    _logger.LogInformation("Found callback handler for request {requestId} and event {event}", context.RequestId, eventBase.GetType());
                    await _dispatcher.DispatchAsync(recognizeFailed, handlerTuple.Item1, handler, clientElements);
                }
            }
        }
    }
}