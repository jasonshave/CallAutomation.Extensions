// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Models;
using System.Reflection;

namespace CallAutomation.Extensions.Interfaces;

internal interface ICallAutomationEventDispatcher
{
    ValueTask DispatchAsync(CallAutomationEventBase @event, Delegate @delegate, CallAutomationClientElements clientElements);

    ValueTask DispatchAsync(CallAutomationEventBase @event, MethodInfo methodInfo, object handlerInstance, CallAutomationClientElements clientElements);
}