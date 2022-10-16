// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationDtmfHelper :
    IRecognizeDtmf,
    IHandleDtmfResponse,
    IHandleDtmfTimeout,
    ICanRecognizeDtmfOptions,
    ICanChooseRecognizeOptions,
    ICallAutomationCallback<DtmfTone>,
    ICallAutomationCallback<Type>
{
    public string RequestId { get; }

    private readonly DtmfToneCallbacks _callbacks = new ();
    private readonly CallMedia _callMedia;
    private readonly int _numTones;

    private Uri _fileUri;
    private CommunicationIdentifier _recognizeInputFromParticipant;
    private RecognizeOptions _recognizeOptions;

    public List<Delegate> GetCallbacks(DtmfTone tone)
    {
        return _callbacks.GetCallbacks(tone.Convert());
    }

    public List<Delegate> GetCallbacks(Type eventTYpe)
    {
        return _callbacks.GetCallbacks(RequestId, eventTYpe);
    }

    internal CallAutomationDtmfHelper(CallMedia callMedia, string requestId)
    {
        _callMedia = callMedia;
        _numTones = 1;
        RequestId = requestId;
    }

    public ICanRecognizeDtmfOptions WithPrompt(string fileUri)
    {
        _fileUri = new Uri(fileUri);
        return this;
    }

    public ICanChooseRecognizeOptions FromParticipant(string rawId)
    {
        // todo: fix this!
        //_recognizeInputFromParticipant = CommunicationIdentifier.FromRawId(rawId.Replace("4:", ""));
        _recognizeInputFromParticipant = new PhoneNumberIdentifier(rawId.Replace("4:", ""));
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone>(Func<RecognizeCompleted, CallConnection, CallMedia, CallRecording, IReadOnlyList<DtmfTone>, ValueTask> callback)
        where TTone : IDtmfTone
    {
        _callbacks.AddCallback<TTone>(callback);
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone>(Func<ValueTask> callback)
        where TTone : IDtmfTone
    {
        _callbacks.AddCallback<TTone>(callback);
        return this;
    }

    public IHandleDtmfResponse OnPress<TTone>(Action callback)
        where TTone : IDtmfTone
    {
        _callbacks.AddCallback<TTone>(callback);
        return this;
    }

    public IHandleDtmfResponse WithOptions(Action<RecognizeOptions> options)
    {
        var recognizeOptions = new RecognizeOptions();
        options(recognizeOptions);
        _recognizeOptions = recognizeOptions;
        return this;
    }

    public IHandleDtmfTimeout OnToneTimeout(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback)
    {
        _callbacks.AddCallback<RecognizeFailed>(RequestId, callback);
        return this;
    }

    public IHandleDtmfTimeout OnToneTimeout(Func<ValueTask> callback)
    {
        _callbacks.AddCallback<RecognizeFailed>(RequestId, callback);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        // register callbacks
        CallbackRegistry.Register<DtmfTone>(this, typeof(RecognizeCompleted));
        CallbackRegistry.Register<Type>(this, typeof(RecognizeFailed));

        // invoke method
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