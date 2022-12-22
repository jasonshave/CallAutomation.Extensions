﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure;
using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface ICreateCallHandling
{
    ICreateCallHandling WithInboundMediaStreaming(string streamingUri);

    /// <summary>
    /// Specifies the handler to invoke when a call is connected.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    ICreateCallHandling OnCallConnected<THandler>()
        where THandler : CallAutomationHandler;

    /// <summary>
    /// Specifies the handler to invoke when a call is disconnected.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    ICreateCallHandling OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler;

    /// <summary>
    /// Specifies the callback delegate when a call is connected.
    /// The event and base objects are not returned in this method.
    /// </summary>
    /// <param name="callbackFunction"></param>
    ICreateCallHandling OnCallConnected(
        Func<ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when a call is connected.
    /// The <see cref="CallConnected"/> event along with <see cref="CallConnection"/>, <see cref="CallMedia"/>, and <see cref="CallRecording"/> are provided.
    /// </summary>
    /// <param name="callbackFunction"></param>
    ICreateCallHandling OnCallConnected(
        Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when a call is disconnected.
    /// The event and base objects are not returned in this method.
    /// </summary>
    /// <param name="callbackFunction"></param>
    ICreateCallHandling OnCallDisconnected(
        Func<ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when a call is disconnected.
    /// The <see cref="CallConnected"/> event along with <see cref="CallConnection"/>, <see cref="CallMedia"/>, and <see cref="CallRecording"/> are provided.
    /// </summary>
    /// <param name="callbackFunction"></param>
    ICreateCallHandling OnCallDisconnected(
        Func<CallDisconnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    /// <summary>
    /// Executes the create call process.
    /// </summary>
    /// <returns><see cref="CreateCallResult"/></returns>
    ValueTask<Response<CreateCallResult>> ExecuteAsync();
}