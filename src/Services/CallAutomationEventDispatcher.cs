// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using System.Reflection;

namespace CallAutomation.Extensions.Services;

public class CallAutomationEventDispatcher : ICallAutomationEventDispatcher
{
    public async ValueTask DispatchAsync(CallAutomationEventBase @event, Delegate callbackFunction, CallAutomationClientElements clientElements)
    {
        if (!callbackFunction.Method.GetParameters().Any())
        {
            await ((ValueTask)callbackFunction.DynamicInvoke()).ConfigureAwait(false);
            return;
        }

        await ((ValueTask)callbackFunction.DynamicInvoke(@event, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording)).ConfigureAwait(false);
    }

    public async ValueTask DispatchAsync(CallAutomationEventBase @event, MethodInfo methodInfo, object handlerInstance, CallAutomationClientElements clientElements)
    {
        var task = methodInfo.Invoke(handlerInstance, new object[] { @event, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording }) as Task;
        if (task is null) return;

        await task.ConfigureAwait(false);
    }

    public void Dispatch(CallAutomationEventBase @event, Action<CallAutomationEventBase, CallConnection, CallMedia, CallRecording> callbackAction, CallAutomationClientElements clientElements)
    {
        callbackAction(@event, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording);
    }
}