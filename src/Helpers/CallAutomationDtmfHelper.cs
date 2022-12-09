// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Extensions;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationDtmfHelper : HelperCallbackWithContext,
    IRecognizeDtmf,
    IHandleDtmfTimeout,
    ICanRecognizeDtmfOptions,
    ICanChooseRecognizeOptions,
    IHandleDtmfResponseWithHandler
{
    private readonly CallMedia _callMedia;

    private Uri _fileUri;
    private CommunicationIdentifier _recognizeInputFromParticipant;
    private RecognizeOptions _recognizeOptions;

    internal CallAutomationDtmfHelper(CallMedia callMedia, string requestId)
        : base(requestId)
    {
        _callMedia = callMedia;
    }

    public ICanRecognizeDtmfOptions WithPrompt(string fileUri)
    {
        _fileUri = new Uri(fileUri);
        return this;
    }

    public ICanChooseRecognizeOptions FromParticipant(string id)
    {
        _recognizeInputFromParticipant = id.ConvertToCommunicationIdentifier();
        return this;
    }

    public IHandleDtmfResponseWithHandler WithOptions(Action<RecognizeOptions> options)
    {
        var recognizeOptions = new RecognizeOptions();
        options(recognizeOptions);
        _recognizeOptions = recognizeOptions;
        return this;
    }

    public IHandleDtmfResponse WithCallbackHandler(ICallbacksHandler handler)
    {
        CallbackHandler = handler;
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone>(Func<RecognizeCompleted, CallConnection, CallMedia, CallRecording, IReadOnlyList<DtmfTone>, ValueTask> callback)
        where TTone : IDtmfTone
    {
        CallbackHandler.AddDelegateCallback<TTone>(RequestId, callback);
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone, THandler>()
        where TTone : IDtmfTone
        where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, TTone>(RequestId, $"On{nameof(RecognizeCompleted)}");
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone>(Func<ValueTask> callback)
        where TTone : IDtmfTone
    {
        CallbackHandler.AddDelegateCallback<TTone>(RequestId, callback);
        return this;
    }

    public IHandleDtmfResponse OnRecognizeCompleted<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, RecognizeCompleted>(RequestId, $"On{nameof(RecognizeCompleted)}");
        return this;
    }

    public IHandleDtmfResponse OnRecognizeCompleted(Func<ValueTask> callback)
    {
        CallbackHandler.AddDelegateCallback<RecognizeCompleted>(RequestId, callback);
        return this;
    }

    public IHandleDtmfTimeout OnFail<TRecognizeFail>(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback)
        where TRecognizeFail : IRecognizeDtmfFailed
    {
        CallbackHandler.AddDelegateCallback<TRecognizeFail>(RequestId, callback);
        return this;
    }

    public IHandleDtmfTimeout OnFail<TRecognizeFail, THandler>()
        where TRecognizeFail : IRecognizeDtmfFailed
        where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, TRecognizeFail>(RequestId, $"On{typeof(TRecognizeFail).Name}");
        return this;
    }

    public IHandleDtmfTimeout OnRecognizeFailed(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback)
    {
        CallbackHandler.AddDelegateCallback<RecognizeFailed>(RequestId, callback);
        return this;
    }

    public IHandleDtmfTimeout OnRecognizeFailed<THandler>()
            where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, RecognizeFailed>(RequestId, $"On{nameof(RecognizeFailed)}");
        return this;
    }

    public IExecuteAsync WithContext(OperationContext context)
    {
        SetContext(context);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        // invoke recognize API
        var recognizeOptions = new CallMediaRecognizeDtmfOptions(
            _recognizeInputFromParticipant,
            _recognizeOptions.MaxToneCount)
        {
            OperationContext = JSONContext,
            Prompt = new FileSource(_fileUri),
            InterruptCallMediaOperation = _recognizeOptions.AllowInterruptExistingMediaOperation,
            InterruptPrompt = _recognizeOptions.AllowInterruptPrompt,
            InterToneTimeout = TimeSpan.FromSeconds(_recognizeOptions.WaitBetweenTonesInSeconds),
            InitialSilenceTimeout = TimeSpan.FromSeconds(_recognizeOptions.WaitForResponseInSeconds),
        };

        await _callMedia.StartRecognizingAsync(recognizeOptions);
    }
}