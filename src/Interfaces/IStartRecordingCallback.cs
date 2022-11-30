// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;
public interface IStartRecordingCallbackkWithHandler : IWithCallbackHandler<IStartRecordingCallback>, IStartRecordingCallback
{
}

public interface IStartRecordingCallback : IExecuteAsync<RecordingStateResult>
{
    /// <summary>
    /// Specifies the handler to invoke when the recording state changed
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <returns></returns>
    IStartRecordingCallback OnRecordingStateChanged<THandler>()
        where THandler : CallAutomationHandler;

    /// <summary>
    /// Specifies the callback delegate when the recording state changed.
    /// The <see cref="RecordingStateChanged"/> event along with <see cref="RecordingStateChanged"/>, <see cref="CallMedia"/>, and <see cref="CallRecording"/> are provided.
    /// </summary>
    /// <param name="callbackFunction"></param>
    /// <returns></returns>
    IStartRecordingCallback OnRecordingStateChanged(
        Func<RecordingStateChanged, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction);

    /// <summary>
    /// Specifies the callback delegate when the recording state changed.
    /// The event and base objects are not returned in this method.
    /// </summary>
    /// <param name="callbackFunction"></param>
    /// <returns></returns>
    IStartRecordingCallback OnRecordingStateChanged(Func<ValueTask> callbackFunction);
}