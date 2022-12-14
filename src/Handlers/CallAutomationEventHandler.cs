// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using Microsoft.Extensions.Logging;

namespace CallAutomation.Extensions.Handlers;

internal sealed class CallAutomationEventHandler : BaseEventHandler, ICallAutomationEventHandler
{
    private readonly ICallAutomationEventDispatcher _dispatcher;
    private readonly ILogger<CallAutomationEventHandler> _logger;

    public CallAutomationEventHandler(
        IServiceProvider serviceProvider,
        ICallAutomationEventDispatcher dispatcher,
        ICallbacksHandler callbackHandler,
        CallAutomationClient client,
        ILogger<CallAutomationEventHandler> logger)
        : base(serviceProvider, callbackHandler, client)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async ValueTask Handle(CallAutomationEventBase eventBase, IOperationContext? operationContext, string? id)
    {
        if (string.IsNullOrEmpty(id)) return;
        var clientElements = new CallAutomationClientElements(_client, eventBase.CallConnectionId);

        var delegates = _callbackHandler.GetDelegateCallbacks(id, eventBase.GetType());
        foreach (var @delegate in delegates)
        {
            _logger.LogInformation("Found callback delegate for request {requestId} and event {event}", id, eventBase.GetType());
            await _dispatcher.DispatchAsync(eventBase, @delegate, clientElements);
        }

        // dispatch handler callbacks
        var handlerTuples = _callbackHandler.GetHandlers(id, eventBase.GetType());
        foreach (var handlerTuple in handlerTuples)
        {
            var handler = GetHandler(handlerTuple.HandlerName);
            if (handler is null) return;

            _logger.LogInformation("Found callback handler for request {requestId} and event {event}", id, eventBase.GetType());

            await _dispatcher.DispatchAsync(eventBase, operationContext, handler, handlerTuple.MethodName, clientElements);
        }
    }
}