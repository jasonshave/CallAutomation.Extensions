﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using System.Reflection;

namespace CallAutomation.Extensions.Dispatchers;

internal sealed class CallAutomationEventDispatcher : ICallAutomationEventDispatcher
{
    public async ValueTask DispatchAsync(CallAutomationEventBase @event, Delegate @delegate, CallAutomationClientElements clientElements, OperationContext context)
    {
        if (!@delegate.Method.GetParameters().Any())
        {
            await ((ValueTask)@delegate.DynamicInvoke()).ConfigureAwait(false);
            return;
        }

        await ((ValueTask)@delegate.DynamicInvoke(@event, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording, context)).ConfigureAwait(false);
    }

    public async ValueTask DispatchAsync(CallAutomationEventBase @event, MethodInfo methodInfo, object handlerInstance, CallAutomationClientElements clientElements, OperationContext context)
    {
        var task = methodInfo.Invoke(handlerInstance, new object[] { @event, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording }) as Task;
        if (task is null) return;

        await task.ConfigureAwait(false);
    }

    public void Dispatch(CallAutomationEventBase @event, Action<CallAutomationEventBase, CallConnection, CallMedia, CallRecording, OperationContext> callbackAction, CallAutomationClientElements clientElements, OperationContext context)
    {
        callbackAction(@event, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording, context);
    }
}