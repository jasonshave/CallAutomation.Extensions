// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallConnectionHandler
{
    ICallConnectionHandler OnCallConnected<THandler>()
        where THandler : CallAutomationHandler;

    ICallConnectionHandler OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler;

    ICallConnectionHandler OnCallConnected(
        Func<ValueTask> callbackFunction);

    ICallConnectionHandler OnCallConnected(
        Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    ICallConnectionHandler OnCallDisconnected(
        Func<ValueTask> callbackFunction);

    ICallConnectionHandler OnCallDisconnected(
        Func<CallDisconnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    ValueTask ExecuteAsync();
}