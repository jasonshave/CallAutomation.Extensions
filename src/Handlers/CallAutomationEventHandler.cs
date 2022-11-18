// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;
using Microsoft.Extensions.Logging;

namespace CallAutomation.Extensions.Handlers;

internal sealed class CallAutomationEventHandler : ICallAutomationEventHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICallAutomationEventDispatcher _dispatcher;
    private readonly CallAutomationClient _client;
    private readonly ILogger<CallAutomationEventHandler> _logger;

    public CallAutomationEventHandler(
        IServiceProvider serviceProvider,
        ICallAutomationEventDispatcher dispatcher,
        CallAutomationClient client,
        ILogger<CallAutomationEventHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _dispatcher = dispatcher;
        _client = client;
        _logger = logger;
    }

    public async ValueTask Handle(CallAutomationEventBase eventBase, string? requestId)
    {
        var clientElements = new CallAutomationClientElements(_client, eventBase.CallConnectionId);

        // use the event type to retrieve the correct callback
        var callAutomationHelperCallback = CallbackRegistry.GetHelperCallback(requestId, eventBase.GetType(), true);

        if (callAutomationHelperCallback is null)
        {
            _logger.LogDebug("No callbacks found for request {requestId}", requestId);
            return;
        }

        // dispatch delegate callbacks
        var delegates = callAutomationHelperCallback.GetDelegateCallbacks(eventBase.GetType());
        foreach (var @delegate in delegates)
        {
            _logger.LogInformation("Found callback delegate for request {requestId} and event {event}", requestId, eventBase.GetType());
            await _dispatcher.DispatchAsync(eventBase, @delegate, clientElements);
        }

        // dispatch handler callbacks
        var handlerTuples = callAutomationHelperCallback.GetHandlers(eventBase.GetType());
        foreach (var handlerTuple in handlerTuples)
        {
            var handler = _serviceProvider.GetService(handlerTuple.Item2);

            if (handler is null) return;

            _logger.LogInformation("Found callback handler for request {requestId} and event {event}", requestId, eventBase.GetType());
            await _dispatcher.DispatchAsync(eventBase, handlerTuple.Item1, handler, clientElements);
        }
    }
}