// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using System.Reflection;

namespace CallAutomation.Extensions.Dispatchers;

internal sealed class CallAutomationEventDispatcher : ICallAutomationEventDispatcher
{
    public async ValueTask DispatchAsync(CallAutomationEventBase @event, Delegate @delegate, CallAutomationClientElements clientElements)
    {
        if (!@delegate.Method.GetParameters().Any())
        {
            await ((ValueTask)@delegate.DynamicInvoke()).ConfigureAwait(false);
            return;
        }

        await ((ValueTask)@delegate.DynamicInvoke(@event, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording)).ConfigureAwait(false);
    }

    public async ValueTask DispatchAsync<T>(CallAutomationEventBase @event, T operationContext, CallAutomationHandler handlerInstance, string methodName, CallAutomationClientElements clientElements)
        where T : OperationContext
    {
        //switch (@event)
        //{
        //    case CallConnected cc:
        //        await handlerInstance.OnCallConnected(cc, operationContext, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording);
        //        return;
        //    default:
        //        break;
        //}
        
        var methodInfo = handlerInstance.GetType().GetMethod(methodName);
        var task = (ValueTask)methodInfo.Invoke(handlerInstance, new object[] { @event, operationContext, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording });
        await task.ConfigureAwait(false);
    }

    public void Dispatch<T>(CallAutomationEventBase @event, T operationContext, Action<CallAutomationEventBase, T, CallConnection, CallMedia, CallRecording> callbackAction, CallAutomationClientElements clientElements)
        where T : OperationContext
    {
        callbackAction(@event, operationContext, clientElements.CallConnection, clientElements.CallMedia, clientElements.CallRecording);
    }
}