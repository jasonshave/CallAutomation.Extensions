// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CallAutomation.Extensions.Services;

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
        var callbackDelegate = CallbackRegistry.GetCallback(requestId, eventBase.GetType(), true);

        if (callbackDelegate is not null)
        {
            _logger.LogInformation("Found callback delegate for request {requestId} and event {event}", requestId, eventBase.GetType());
            await _dispatcher.DispatchAsync(eventBase, callbackDelegate, clientElements);
            return;
        }

        (Type? handlerType, MethodInfo? methodInfo) = CallbackRegistry.GetCallbackHandler(requestId, eventBase.GetType(), true);
        if (handlerType is null || methodInfo is null) return;

        var handler = _serviceProvider.GetService(handlerType);
        if (handler is null) return;

        await _dispatcher.DispatchAsync(eventBase, methodInfo, handler, clientElements);
    }
}