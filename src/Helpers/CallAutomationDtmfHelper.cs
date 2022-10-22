﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
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
    IHandleDtmfResponse,
    IHandleDtmfTimeout,
    ICanRecognizeDtmfOptions,
    ICanChooseRecognizeOptions
{
    private readonly CallMedia _callMedia;
    private readonly int _numTones;

    private Uri _fileUri;
    private CommunicationIdentifier _recognizeInputFromParticipant;
    private RecognizeOptions _recognizeOptions;

    internal CallAutomationDtmfHelper(CallMedia callMedia, string requestId)
        : base(requestId)
    {
        _callMedia = callMedia;
        _numTones = 1;
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

    public IHandleDtmfResponse OnPress<TTone>(Func<RecognizeCompleted, CallConnection, CallMedia, CallRecording, IReadOnlyList<DtmfTone>, ValueTask> callback)
        where TTone : IDtmfTone
    {
        HelperCallbacks.AddDelegateCallback<TTone>(callback);
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone, THandler>()
        where TTone : IDtmfTone
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, TTone>($"On{nameof(RecognizeCompleted)}", typeof(RecognizeCompleted), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording), typeof(IReadOnlyList<DtmfTone>));
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone>(Func<ValueTask> callback)
        where TTone : IDtmfTone
    {
        HelperCallbacks.AddDelegateCallback<TTone>(callback);
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone>(Action callback)
        where TTone : IDtmfTone
    {
        HelperCallbacks.AddDelegateCallback<TTone>(callback);
        return this;
    }

    public IHandleDtmfResponse WithOptions(Action<RecognizeOptions> options)
    {
        var recognizeOptions = new RecognizeOptions();
        options(recognizeOptions);
        _recognizeOptions = recognizeOptions;
        return this;
    }

    public IHandleDtmfTimeout OnInputTimeout(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback)
    {
        HelperCallbacks.AddDelegateCallback<RecognizeFailed>(callback);
        return this;
    }

    public IHandleDtmfTimeout OnInputTimeout<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, RecognizeFailed>($"{nameof(OnInputTimeout)}", typeof(RecognizeFailed), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public IHandleDtmfTimeout OnInterToneTimeout(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback)
    {
        HelperCallbacks.AddDelegateCallback<RecognizeFailed>(callback);
        return this;
    }

    public IHandleDtmfTimeout OnInterToneTimeout<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, RecognizeFailed>($"{nameof(OnInterToneTimeout)}", typeof(RecognizeFailed), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public IHandleDtmfTimeout OnInputTimeout(Func<ValueTask> callback)
    {
        HelperCallbacks.AddDelegateCallback<RecognizeFailed>(callback);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        // register callbacks
        CallbackRegistry.RegisterHelperCallback(this, new[] { typeof(RecognizeFailed), typeof(RecognizeCompleted) });

        // invoke recognize API
        var recognizeOptions = new CallMediaRecognizeDtmfOptions(_recognizeInputFromParticipant, _numTones)
        {
            OperationContext = RequestId,
            Prompt = new FileSource(_fileUri),
            InterruptCallMediaOperation = _recognizeOptions.AllowInterruptExistingMediaOperation,
            InterruptPrompt = _recognizeOptions.AllowInterruptPrompt,
        };

        if (_recognizeOptions.WaitBetweenTonesInSeconds < 0)
            recognizeOptions.InterToneTimeout = TimeSpan.FromSeconds(_recognizeOptions.WaitBetweenTonesInSeconds);

        if (_recognizeOptions.WaitForResponseInSeconds < 0)
            recognizeOptions.InitialSilenceTimeout = TimeSpan.FromSeconds(_recognizeOptions.WaitForResponseInSeconds);

        await _callMedia.StartRecognizingAsync(recognizeOptions);
    }
}