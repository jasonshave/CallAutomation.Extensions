// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Extensions;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationDtmfHelper : HelperCallbackBase,
    IRecognizeDtmf,
    ICanRecognizeDtmfOptions,
    ICanChooseRecognizeOptions,
    IHandleDtmfResponse,
    IHandleDtmfTimeout
{
    private readonly CallMedia _callMedia;

    private Uri? _fileUri;
    private CommunicationIdentifier _recognizeInputFromParticipant;
    private RecognizeOptions _recognizeOptions;
    private string? _textToSpeak;

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

    public ICanRecognizeDtmfOptions WithSpeech(string textToSpeak)
    {
        _textToSpeak = textToSpeak;
        return this;
    }

    public ICanChooseRecognizeOptions FromParticipant(string id)
    {
        _recognizeInputFromParticipant = id.ConvertToCommunicationIdentifier();
        return this;
    }

    public IHandleDtmfResponse WithOptions(Action<RecognizeOptions> options)
    {
        var recognizeOptions = new RecognizeOptions();
        options(recognizeOptions);
        _recognizeOptions = recognizeOptions;
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone>(Func<RecognizeCompleted, CallConnection, CallMedia, CallRecording, IReadOnlyList<DtmfTone>, ValueTask> callback)
        where TTone : IDtmfTone
    {
        HelperCallbacks.AddDelegateCallback<TTone>(RequestId, callback);
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone, THandler>()
        where TTone : IDtmfTone
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, TTone>(RequestId, $"On{nameof(RecognizeCompleted)}");
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone>(Func<ValueTask> callback)
        where TTone : IDtmfTone
    {
        HelperCallbacks.AddDelegateCallback<TTone>(RequestId, callback);
        return this;
    }

    public IHandleDtmfResponse OnRecognizeCompleted<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, RecognizeCompleted>(RequestId, $"On{nameof(RecognizeCompleted)}");
        return this;
    }

    public IHandleDtmfResponse OnRecognizeCompleted(Func<ValueTask> callback)
    {
        HelperCallbacks.AddDelegateCallback<RecognizeCompleted>(RequestId, callback);
        return this;
    }

    public IHandleDtmfResponse OnStopToneDetected<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, RecognizeCompleted>(RequestId, $"On{nameof(StopToneDetected)}");
        return this;
    }

    public IHandleDtmfTimeout OnFail<TRecognizeFail>(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback)
        where TRecognizeFail : IRecognizeDtmfFailed
    {
        HelperCallbacks.AddDelegateCallback<TRecognizeFail>(RequestId, callback);
        return this;
    }

    public IHandleDtmfResponse OnFail<TRecognizeFail, THandler>()
        where TRecognizeFail : IRecognizeDtmfFailed
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, TRecognizeFail>(RequestId, $"On{typeof(TRecognizeFail).Name}");
        return this;
    }

    public IHandleDtmfResponse OnFail<TRecognizeFail>(Func<ValueTask> callback)
        where TRecognizeFail : IRecognizeDtmfFailed
    {
        HelperCallbacks.AddDelegateCallback<TRecognizeFail>(RequestId, callback);
        return this;
    }

    public IHandleDtmfResponse OnRecognizeFailed(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback)
    {
        HelperCallbacks.AddDelegateCallback<RecognizeFailed>(RequestId, callback);
        return this;
    }

    public IHandleDtmfResponse OnRecognizeFailed<THandler>()
            where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, RecognizeFailed>(RequestId, $"On{nameof(RecognizeFailed)}");
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        // invoke recognize API
        PlaySource playSource = default;

        if (_fileUri is not null)
        {
            playSource = new FileSource(_fileUri);
        }

        if (_textToSpeak is not null)
        {
            playSource = new TextSource(_textToSpeak);
        }

        var recognizeOptions = new CallMediaRecognizeDtmfOptions(
            _recognizeInputFromParticipant,
            _recognizeOptions.MaxToneCount)
        {
            OperationContext = RequestId,
            Prompt = playSource,
            InterruptCallMediaOperation = _recognizeOptions.AllowInterruptExistingMediaOperation,
            InterruptPrompt = _recognizeOptions.AllowInterruptPrompt,
            InterToneTimeout = TimeSpan.FromSeconds(_recognizeOptions.WaitBetweenTonesInSeconds),
            InitialSilenceTimeout = TimeSpan.FromSeconds(_recognizeOptions.WaitForResponseInSeconds),
            StopTones = _recognizeOptions.StopTones
        };

        await _callMedia.StartRecognizingAsync(recognizeOptions);
    }
}