// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface ICreateCallHandling
{
    ICreateCallHandling OnCallConnected<THandler>()
        where THandler : CallAutomationHandler;

    ICreateCallHandling OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler;

    ICreateCallHandling OnCallConnected(
        Func<ValueTask> callbackFunction);

    ICreateCallHandling OnCallConnected(
        Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    ICreateCallHandling OnCallDisconnected(
        Func<ValueTask> callbackFunction);

    ICreateCallHandling OnCallDisconnected(
        Func<CallDisconnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    ValueTask<CreateCallResult> ExecuteAsync();
}