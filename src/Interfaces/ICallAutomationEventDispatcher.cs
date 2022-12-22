// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

internal interface ICallAutomationEventDispatcher
{
    ValueTask DispatchAsync(CallAutomationEventBase @event, Delegate @delegate, CallAutomationClientElements clientElements);

    ValueTask DispatchAsync(CallAutomationEventBase @event, IOperationContext? operationContext, CallAutomationHandler handlerInstance, string methodName, CallAutomationClientElements clientElements);
}