// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;
using System.Text.Json;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationStartRecordingHelper : HelperCallbackBase, IStartRecordingCallbackkWithHandler
{
    private readonly CallRecording _callRecording;
    private readonly StartRecordingOptions _startRecordingOptions;

    internal CallAutomationStartRecordingHelper(CallRecording callRecording, StartRecordingOptions startRecordingOptions, string requestId)
        : base(requestId)
    {
        _callRecording = callRecording;
        _startRecordingOptions = startRecordingOptions;
    }

    public IStartRecordingCallback WithCallbackHandler(ICallbacksHandler handler)
    {
        CallbackHandler = handler;
        return this;
    }

    public IStartRecordingCallback OnRecordingStateChanged<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, RecordingStateChanged>(RequestId, $"On{nameof(RecordingStateChanged)}");
        return this;
    }

    public IStartRecordingCallback OnRecordingStateChanged(Func<ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<RecordingStateChanged>(RequestId, callbackFunction);
        return this;
    }

    public IStartRecordingCallback OnRecordingStateChanged(Func<RecordingStateChanged, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<RecordingStateChanged>(RequestId, callbackFunction);
        return this;
    }

    public async ValueTask<RecordingStateResult> ExecuteAsync()
    {
        return await _callRecording
            .StartRecordingAsync(_startRecordingOptions);
    }
}