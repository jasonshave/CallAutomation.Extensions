// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallAutomationEventDispatcher
{
    ValueTask DispatchAsync(CallAutomationEventBase @event, Delegate callbackFunction, CallAutomationClientElements clientElements);

    ValueTask DispatchAsync(CallAutomationEventBase @event, MethodInfo methodInfo, object handlerInstance, CallAutomationClientElements clientElements);

    void Dispatch(CallAutomationEventBase @event, Action<CallAutomationEventBase, CallConnection, CallMedia, CallRecording> callbackAction, CallAutomationClientElements clientElements);
}