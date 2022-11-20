// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure;
using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Extensions;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationDtmfHelper : HelperCallbackBase,
    IRecognizeDtmf,
    IHandleDtmfTimeout,
    ICanRecognizeDtmfOptions,
    ICanChooseRecognizeOptions,
    IHandleDtmfResponse
{
    private static readonly IEnumerable<Type> _types = new[] { typeof(RecognizeFailed), typeof(RecognizeCompleted), typeof(SilenceTimeout) };
    private readonly CallMedia _callMedia;
    private readonly int _numTones;

    private Uri _fileUri;
    private CommunicationIdentifier _recognizeInputFromParticipant;
    private RecognizeOptions _recognizeOptions;

    internal CallAutomationDtmfHelper(CallMedia callMedia)
        : base(_types)
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

    public IHandleDtmfResponse OnPress<TTone>(Func<RecognizeCompleted, CallConnection, CallMedia, CallRecording, IReadOnlyList<DtmfTone>, IOperationContext, ValueTask> callback)
        where TTone : IDtmfTone
    {
        AddDelegateCallback<TTone>(callback);
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone, THandler>()
        where TTone : IDtmfTone
        where THandler : CallAutomationHandler
    {
        AddHandlerCallback<THandler, TTone>($"On{nameof(RecognizeCompleted)}", typeof(RecognizeCompleted), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording), typeof(IReadOnlyList<DtmfTone>), typeof(IOperationContext));
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone>(Func<ValueTask> callback)
        where TTone : IDtmfTone
    {
        AddDelegateCallback<TTone>(callback);
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone>(Action callback)
        where TTone : IDtmfTone
    {
        AddDelegateCallback<TTone>(callback);
        return this;
    }

    public IHandleDtmfResponse WithOptions(Action<RecognizeOptions> options)
    {
        var recognizeOptions = new RecognizeOptions();
        options(recognizeOptions);
        _recognizeOptions = recognizeOptions;
        return this;
    }

    public IHandleDtmfTimeout OnFail<TRecognizeFail>(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, IOperationContext, ValueTask> callback)
        where TRecognizeFail : IRecognizeDtmfFailed
    {
        AddDelegateCallback<TRecognizeFail>(callback);
        return this;
    }

    public IHandleDtmfTimeout OnFail<TRecognizeFail>(Func<ValueTask> callback)
        where TRecognizeFail : IRecognizeDtmfFailed
    {
        AddDelegateCallback<TRecognizeFail>(callback);
        return this;
    }

    public IHandleDtmfTimeout OnFail<TRecognizeFail, THandler>()
        where TRecognizeFail : IRecognizeDtmfFailed
        where THandler : CallAutomationHandler
    {
        AddHandlerCallback<THandler, TRecognizeFail>($"On{typeof(TRecognizeFail).Name}", typeof(RecognizeFailed), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording), typeof(IOperationContext));
        return this;
    }

    public IHandleDtmfTimeout OnInputTimeout(Func<ValueTask> callback)
    {
        AddDelegateCallback<RecognizeFailed>(callback);
        return this;
    }

    public IHandleDtmfTimeout WithContext(IOperationContext context)
    {
        OperationContext = context;
        return this;
    }

    public async ValueTask<Response> ExecuteAsync()
    {
        // invoke recognize API
        var recognizeOptions = new CallMediaRecognizeDtmfOptions(_recognizeInputFromParticipant, _numTones)
        {
            OperationContext = GetSerializedContext(),
            Prompt = new FileSource(_fileUri),
            InterruptCallMediaOperation = _recognizeOptions.AllowInterruptExistingMediaOperation,
            InterruptPrompt = _recognizeOptions.AllowInterruptPrompt,
        };

        if (_recognizeOptions.WaitBetweenTonesInSeconds < 0)
            recognizeOptions.InterToneTimeout = TimeSpan.FromSeconds(_recognizeOptions.WaitBetweenTonesInSeconds);

        if (_recognizeOptions.WaitForResponseInSeconds < 0)
            recognizeOptions.InitialSilenceTimeout = TimeSpan.FromSeconds(_recognizeOptions.WaitForResponseInSeconds);

        return await _callMedia.StartRecognizingAsync(recognizeOptions);
    }
}