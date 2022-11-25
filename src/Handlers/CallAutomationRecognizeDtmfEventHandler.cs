// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Extensions;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;
using Microsoft.Extensions.Logging;

namespace CallAutomation.Extensions.Handlers;

public class CallAutomationRecognizeDtmfEventHandler : BaseEventHandler, ICallAutomationRecognizeDtmfHandler
{
    private readonly ICallAutomationRecognizeEventDispatcher _dispatcher;
    private readonly ILogger<CallAutomationRecognizeDtmfEventHandler> _logger;

    public CallAutomationRecognizeDtmfEventHandler(
        IServiceProvider serviceProvider,
        ICallAutomationRecognizeEventDispatcher dispatcher,
        ICallbacksHandler callbackHandler,
        CallAutomationClient client,
        ILogger<CallAutomationRecognizeDtmfEventHandler> logger)
        : base(serviceProvider, callbackHandler, client)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async ValueTask Handle(CallAutomationEventBase eventBase, string? requestId)
    {
        var clientElements = new CallAutomationClientElements(_client, eventBase.CallConnectionId);

        if (eventBase is RecognizeCompleted recognizeCompleted)
        {
            var tone = recognizeCompleted.CollectTonesResult.Tones.FirstOrDefault();

            // need to determine if one or more tones are collected as we need to get a single
            // delegate for a single tone or get a single delegate for multiple tones
            if (recognizeCompleted.CollectTonesResult.Tones.Count is 1)
            {
                // dispatch delegate callbacks
                var delegates = _callbackHandler.GetDelegateCallbacks(requestId, tone.Convert().GetType());
                foreach (var @delegate in delegates)
                {
                    _logger.LogInformation("Found callback delegate for request {requestId}, with {numTones} DTMF tone(s), and event {event}", requestId, recognizeCompleted.CollectTonesResult.Tones.Count, eventBase.GetType().Name);
                    await _dispatcher.DispatchAsync(recognizeCompleted, @delegate, clientElements, recognizeCompleted.CollectTonesResult.Tones);
                }

                var handlerTuples = _callbackHandler.GetHandlers(requestId, tone.Convert().GetType());
                foreach (var handlerTuple in handlerTuples)
                {
                    var handler = GetHandler(handlerTuple.HandlerName);
                    if (handler is null) return;

                    _logger.LogInformation("Found callback handler for request {requestId} and event {event}", requestId, eventBase.GetType());
                    await _dispatcher.DispatchAsync(recognizeCompleted, handler, handlerTuple.MethodName, clientElements, recognizeCompleted.CollectTonesResult.Tones);
                }
            }
        }

        if (eventBase is RecognizeFailed recognizeFailed)
        {
            // dispatch delegate callbacks
            var delegates = _callbackHandler.GetDelegateCallbacks(requestId, recognizeFailed.ReasonCode.Convert().GetType());
            foreach (var @delegate in delegates)
            {
                _logger.LogInformation("Found callback delegate for request {requestId}, and event {event}", requestId, eventBase.GetType().Name);
                await _dispatcher.DispatchAsync(recognizeFailed, @delegate, clientElements);
            }

            var handlerTuples = _callbackHandler.GetHandlers(requestId, recognizeFailed.ReasonCode.Convert().GetType());
            foreach (var handlerTuple in handlerTuples)
            {
                var handler = GetHandler(handlerTuple.HandlerName);
                if (handler is null) return;

                _logger.LogInformation("Found callback handler for request {requestId} and event {event}", requestId, eventBase.GetType());
                await _dispatcher.DispatchAsync(recognizeFailed, handler, handlerTuple.MethodName, clientElements);
            }
        }
    }
}