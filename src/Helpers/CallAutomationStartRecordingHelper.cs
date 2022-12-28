// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationStartRecordingHelper : HelperCallbackBase, IStartRecording
{
    private readonly CallRecording _callRecording;
    private readonly StartRecordingOptions _startRecordingOptions;

    internal CallAutomationStartRecordingHelper(CallRecording callRecording, StartRecordingOptions startRecordingOptions, string requestId)
        : base(requestId)
    {
        _callRecording = callRecording;
        _startRecordingOptions = startRecordingOptions;
    }

    public IStartRecording OnRecordingStateChanged<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, RecordingStateChanged>(RequestId, $"On{nameof(RecordingStateChanged)}");
        return this;
    }

    public IStartRecording OnRecordingStateChanged(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<RecordingStateChanged>(RequestId, callbackFunction);
        return this;
    }

    public IStartRecording OnRecordingStateChanged(Func<RecordingStateChanged, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<RecordingStateChanged>(RequestId, callbackFunction);
        return this;
    }

    public async ValueTask<RecordingStateResult> ExecuteAsync()
    {
        return await _callRecording
            .StartRecordingAsync(_startRecordingOptions);
    }
}