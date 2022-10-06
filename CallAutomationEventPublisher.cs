// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Azure.Communication.CallAutomation;
using Azure.Messaging;
using JasonShave.Azure.Communication.Service.CallAutomation.Extensions.Interfaces;

namespace JasonShave.Azure.Communication.Service.CallAutomation.Extensions;

internal sealed class CallAutomationEventPublisher : ICallAutomationEventPublisher
{
    private readonly CallAutomationClient _client;
    private readonly IServiceProvider _serviceProvider;

    public CallAutomationEventPublisher(CallAutomationClient client, IServiceProvider serviceProvider)
    {
        _client = client;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask PublishAsync(CloudEvent[] cloudEvents, string? requestId = default)
    {
        foreach (var cloudEvent in cloudEvents)
        {
            CallAutomationEventBase callAutomationEventBase = CallAutomationEventParser.Parse(cloudEvent);

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