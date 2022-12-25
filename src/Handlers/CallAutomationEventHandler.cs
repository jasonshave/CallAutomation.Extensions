// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;
using Microsoft.Extensions.Logging;

namespace CallAutomation.Extensions.Handlers;

internal sealed class CallAutomationEventHandler : BaseEventHandler, ICallAutomationEventHandler
{
    private readonly ICallAutomationEventDispatcher _dispatcher;
    private readonly ILogger<CallAutomationEventHandler> _logger;

    public CallAutomationEventHandler(
        IServiceProvider serviceProvider,
        ICallAutomationEventDispatcher dispatcher,
        CallAutomationClient client,
        ILogger<CallAutomationEventHandler> logger)
        : base(serviceProvider, client)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async ValueTask Handle(CallAutomationEventBase eventBase, string? requestId)
    {
        if (string.IsNullOrEmpty(requestId)) return;
        var clientElements = new CallAutomationClientElements(_client, eventBase.CallConnectionId);

        var callAutomationHelperCallback = CallbackRegistry.GetHelperCallback(requestId, eventBase.GetType(), true);

        if (callAutomationHelperCallback is null)
        {
            _logger.LogDebug("No callbacks found for request {requestId}", requestId);
            return;
        }

        var delegates = callAutomationHelperCallback.HelperCallbacks.GetDelegateCallbacks(requestId, callAutomationHelperCallback.GetType());
        foreach (var @delegate in delegates)
        {
            _logger.LogInformation("Found callback delegate for request {requestId} and event {event}", requestId, eventBase.GetType());
            await _dispatcher.DispatchDelegateAsync(eventBase, @delegate, clientElements);
        }

        // dispatch handler callbacks
        var handlerTuples = callAutomationHelperCallback.HelperCallbacks.GetHandlers(requestId, eventBase.GetType());
        foreach (var handlerTuple in handlerTuples)
        {
            var handler = GetHandler(handlerTuple.HandlerName);
            if (handler is null) return;

            _logger.LogInformation("Found callback handler for request {requestId} and event {event}", requestId, eventBase.GetType());

            await _dispatcher.DispatchHandlerAsync(eventBase, handler, handlerTuple.MethodName, clientElements);
        }
    }
}