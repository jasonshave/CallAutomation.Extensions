// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface IAnswerCallHandling : IExecuteAsync<AnswerCallResult>
{
    /// <summary>
    /// Specifies the handler to invoke when a call is connected.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    IAnswerCallHandling OnCallConnected<THandler>()
        where THandler : CallAutomationHandler;

    /// <summary>
    /// Specifies the handler to invoke when a call is disconnected.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    IAnswerCallHandling OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler;

    /// <summary>
    /// Specifies the callback delegate when a call is connected. The event and base objects are not returned in this method.
    /// </summary>
    /// <param name="callbackFunction"></param>
    IAnswerCallHandling OnCallConnected(
        Func<ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when a call is connected. The event, <see cref="CallConnection"/>, <see cref="CallMedia"/>, <see cref="CallRecording"/>, and <see cref="IOperationContext"/>
    /// are returned.
    /// </summary>
    /// <param name="callbackFunction"></param>
    IAnswerCallHandling OnCallConnected(
        Func<CallConnected, CallConnection, CallMedia, CallRecording, IOperationContext, ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when a call is disconnected. The event and base objects are not returned in this method.
    /// </summary>
    /// <param name="callbackFunction"></param>
    IAnswerCallHandling OnCallDisconnected(
        Func<ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when a call is disconnected. The event, <see cref="CallConnection"/>, <see cref="CallMedia"/>, <see cref="CallRecording"/>, and <see cref="IOperationContext"/> are returned.
    /// </summary>
    /// <param name="callbackFunction"></param>
    IAnswerCallHandling OnCallDisconnected(
        Func<CallDisconnected, CallConnection, CallMedia, CallRecording, IOperationContext, ValueTask> callbackFunction);

    /// <summary>
    /// Sets the operation context which is returned on the next delegate.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    IAnswerCallHandling WithContext(IOperationContext context);
}