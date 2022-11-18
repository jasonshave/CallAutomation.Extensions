// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Interfaces;

public interface IAnswerCallHandling
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
    /// Specifies the callback delegate when a call is connected. The event, <see cref="CallConnection"/>, <see cref="CallMedia"/>, <see cref="CallRecording"/>, and <see cref="OperationContext"/>
    /// are provided to the delegate when invoked.
    /// </summary>
    /// <param name="callbackFunction"></param>
    IAnswerCallHandling OnCallConnected(
        Func<CallConnected, CallConnection, CallMedia, CallRecording, OperationContext, ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when a call is disconnected. The event and base objects are not returned in this method.
    /// </summary>
    /// <param name="callbackFunction"></param>
    IAnswerCallHandling OnCallDisconnected(
        Func<ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when a call is disconnected. The event, <see cref="CallConnection"/>, <see cref="CallMedia"/>, <see cref="CallRecording"/>
    /// </summary>
    /// <param name="callbackFunction"></param>
    IAnswerCallHandling OnCallDisconnected(
        Func<CallDisconnected, CallConnection, CallMedia, CallRecording, OperationContext, ValueTask> callbackFunction);

    /// <summary>
    /// Executes the create call process.
    /// </summary>
    /// <returns><see cref="AnswerCallResult"/></returns>
    ValueTask<AnswerCallResult> ExecuteAsync();
}