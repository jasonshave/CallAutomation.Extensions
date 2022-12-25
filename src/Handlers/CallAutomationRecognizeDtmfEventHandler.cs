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
        CallAutomationClient client,
        ILogger<CallAutomationRecognizeDtmfEventHandler> logger)
        : base(serviceProvider, client)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async ValueTask Handle(CallAutomationEventBase eventBase, string? requestId)
    {
        var clientElements = new CallAutomationClientElements(_client, eventBase.CallConnectionId);
        var isHandled = false;

        var callAutomationHelperCallback = CallbackRegistry.GetHelperCallback(requestId, typeof(RecognizeCompleted), true);

        if (eventBase is RecognizeCompleted recognizeCompleted)
        {
            var tone = recognizeCompleted.CollectTonesResult.Tones.FirstOrDefault();

            var callBasedOn = async (Type type) =>
            {
                // dispatch delegate callbacks
                var delegates = callAutomationHelperCallback.HelperCallbacks.GetDelegateCallbacks(requestId, type);
                foreach (var @delegate in delegates)
                {
                    isHandled = true;
                    _logger.LogInformation("Found callback delegate for request {requestId}, with {numTones} DTMF tone(s), and event {event}", requestId, recognizeCompleted.CollectTonesResult.Tones.Count, eventBase.GetType().Name);
                    await _dispatcher.DispatchDelegateAsync(recognizeCompleted, @delegate, clientElements, recognizeCompleted.CollectTonesResult.Tones);
                }

                var handlerTuples = callAutomationHelperCallback.HelperCallbacks.GetHandlers(requestId, type);
                foreach (var handlerTuple in handlerTuples)
                {
                    var handler = GetHandler(handlerTuple.HandlerName);
                    if (handler is null) return;

                    isHandled = true;
                    _logger.LogInformation("Found callback handler for request {requestId} and event {event}", requestId, eventBase.GetType().Name);
                    await _dispatcher.DispatchHandlerAsync(recognizeCompleted, handler, handlerTuple.MethodName, clientElements, recognizeCompleted.CollectTonesResult.Tones);
                }
            };

            // need to determine if one or more tones are collected as we need to get a single
            // delegate for a single tone or get a single delegate for multiple tones
            if (recognizeCompleted.CollectTonesResult.Tones.Count is 1)
                await callBasedOn(tone.Convert().GetType());

            if (!isHandled)
                await callBasedOn(eventBase.GetType());
        }
        else if (eventBase is RecognizeFailed recognizeFailed)
        {
            var callBasedOn = async (Type type) =>
            {
                // dispatch delegate callbacks
                var delegates = callAutomationHelperCallback.HelperCallbacks.GetDelegateCallbacks(requestId, type);
                foreach (var @delegate in delegates)
                {
                    isHandled = true;
                    _logger.LogInformation("Found callback delegate for request {requestId}, and event {event}", requestId, eventBase.GetType().Name);
                    await _dispatcher.DispatchDelegateAsync(recognizeFailed, @delegate, clientElements);
                }

                var handlerTuples = callAutomationHelperCallback.HelperCallbacks.GetHandlers(requestId, type);
                foreach (var handlerTuple in handlerTuples)
                {
                    var handler = GetHandler(handlerTuple.HandlerName);
                    if (handler is null) return;

                    isHandled = true;
                    _logger.LogInformation("Found callback handler for request {requestId} and event {event}", requestId, eventBase.GetType().Name);
                    await _dispatcher.DispatchHandlerAsync(recognizeFailed, handler, handlerTuple.MethodName, clientElements);
                }
            };

            await callBasedOn(recognizeFailed.ReasonCode.Convert().GetType());
            if (!isHandled)
                await callBasedOn(eventBase.GetType());
        }
    }
}