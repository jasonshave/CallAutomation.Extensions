// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure;
using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface IPlayMediaCallback : IExecuteAsync<Response>
{
    /// <summary>
    /// Targets a specific participant on the call to hear the audio file.
    /// </summary>
    /// <param name="rawId"></param>
    /// <returns></returns>
    IPlayMediaCallback ToParticipant(string rawId);

    /// <summary>
    /// Specifies the handler to invoke when the audio file has stopped playing.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <returns></returns>
    IPlayMediaCallback OnPlayCompleted<THandler>()
        where THandler : CallAutomationHandler;

    /// <summary>
    /// Specifies the callback delegate when the audio file stops playing.
    /// The <see cref="CallConnected"/> event along with <see cref="CallConnection"/>, <see cref="CallMedia"/>, <see cref="CallRecording"/>, and <see cref="IOperationContext"/> are provided.
    /// </summary>
    /// <param name="callbackFunction"></param>
    /// <returns></returns>
    IPlayMediaCallback OnPlayCompleted(
        Func<PlayCompleted, CallConnection, CallMedia, CallRecording, IOperationContext, ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when the audio file stops playing.
    /// The event and base objects are not returned in this method.
    /// </summary>
    /// <param name="callbackFunction"></param>
    /// <returns></returns>
    IPlayMediaCallback OnPlayCompleted(Func<ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the handler to invoke when the audio file cannot be played.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <returns></returns>
    IPlayMediaCallback OnPlayFailed<THandler>()
        where THandler : CallAutomationHandler;

    /// <summary>
    /// Specifies the callback delegate to invoke when the audio file cannot be played.
    /// </summary>
    /// <param name="callbackFunction"></param>
    /// <returns></returns>
    IPlayMediaCallback OnPlayFailed(
        Func<PlayFailed, CallConnection, CallMedia, CallRecording, IOperationContext, ValueTask> callbackFunction);

    /// <summary>
    /// Sets the custom context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    IPlayMediaCallback WithContext(IOperationContext context);
}