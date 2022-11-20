// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface ICreateCallHandling : IExecuteAsync<CreateCallResult>
{
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
    /// The <see cref="CallConnected"/> event along with <see cref="CallConnection"/>, <see cref="CallMedia"/>, <see cref="CallRecording"/>, and <see cref="IOperationContext"/> are provided.
    /// </summary>
    /// <param name="callbackFunction"></param>
    ICreateCallHandling OnCallConnected(
        Func<CallConnected, CallConnection, CallMedia, CallRecording, IOperationContext, ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when a call is disconnected.
    /// The event and base objects are not returned in this method.
    /// </summary>
    /// <param name="callbackFunction"></param>
    ICreateCallHandling OnCallDisconnected(
        Func<ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when a call is disconnected.
    /// The <see cref="CallConnected"/> event along with <see cref="CallConnection"/>, <see cref="CallMedia"/>, <see cref="CallRecording"/>, and <see cref="IOperationContext"/> are provided.
    /// </summary>
    /// <param name="callbackFunction"></param>
    ICreateCallHandling OnCallDisconnected(
        Func<CallDisconnected, CallConnection, CallMedia, CallRecording, IOperationContext, ValueTask> callbackFunction);

    /// <summary>
    /// Adds custom context to be serialized and returned.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    ICreateCallHandling WithContext(IOperationContext context);
}