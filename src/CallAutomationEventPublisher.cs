// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Azure.Communication.CallAutomation;
using Azure.Messaging;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Interfaces;
using Microsoft.Extensions.Logging;

namespace JasonShave.Azure.Communication.CallAutomation.Extensions;

internal sealed class CallAutomationEventPublisher : ICallAutomationEventPublisher
{
    private readonly CallAutomationClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CallAutomationEventPublisher> _logger;

    public CallAutomationEventPublisher(
        CallAutomationClient client, 
        IServiceProvider serviceProvider, 
        ILogger<CallAutomationEventPublisher> logger)
    {
        _client = client;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async ValueTask PublishAsync(CloudEvent[] cloudEvents, string? requestId = default)
    {
        foreach (var cloudEvent in cloudEvents)
        {
            CallAutomationEventBase callAutomationEventBase = CallAutomationEventParser.Parse(cloudEvent);
            _logger.LogInformation($"Handling request: {requestId} for event: {callAutomationEventBase.GetType()}");

            CallConnection callConnection = _client.GetCallConnection(callAutomationEventBase.CallConnectionId);
            CallMedia callMedia = callConnection.GetCallMedia();
            CallRecording callRecording = _client.GetCallRecording();

            Delegate? callbackDelegate;
            if (callAutomationEventBase is CallConnected or CallDisconnected)
            {
                // for outbound calls the "CustomerContext" property should be used
                // otherwise for inbound calls the "CorrelationId" will work.
                callbackDelegate = CallbackRegistry.GetCallback(callAutomationEventBase.CorrelationId, callAutomationEventBase.GetType(), true);
            }
            else
            {
                callbackDelegate = CallbackRegistry.GetCallback(callAutomationEventBase.OperationContext, callAutomationEventBase.GetType(), true);
            }

            if (callbackDelegate is not null)
            {
                if (!callbackDelegate.Method.GetParameters().Any())
                {
                    await ((ValueTask)callbackDelegate.DynamicInvoke()).ConfigureAwait(false);
                    return;
                }

                await ((ValueTask)callbackDelegate.DynamicInvoke(callAutomationEventBase, callConnection, callMedia, callRecording)).ConfigureAwait(false); 
                return;
            }

            (Type? handlerType, MethodInfo? methodInfo) = CallbackRegistry.GetCallbackHandlerMethod(callAutomationEventBase.OperationContext, callAutomationEventBase.GetType(), true);
            if (handlerType is null || methodInfo is null) return;

            var handler = _serviceProvider.GetService(handlerType);
            if (handler is null) return;

            var task = methodInfo.Invoke(handler, new object[] { callAutomationEventBase, callConnection, callMedia, callRecording }) as Task;
            if (task is null) return;

            await task.ConfigureAwait(false);
        }
    }
}