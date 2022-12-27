// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

internal interface ICallAutomationEventDispatcher
{
    ValueTask DispatchDelegateAsync(CallAutomationEventBase @event, Delegate callbackFunction, CallAutomationClientElements clientElements);

    ValueTask DispatchHandlerAsync(CallAutomationEventBase @event, object handlerInstance, string methodName, CallAutomationClientElements clientElements);
}