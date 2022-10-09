// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationRecognizeDtmfHelper : IRecognizeDtmf, IHandleDtmfResponse, ICanChooseRecognizeOptions
{
    private readonly CallMedia _callMedia;
    private readonly int _numTones;
    private readonly DtmfOptions _dtmfOptions;

    internal CallAutomationRecognizeDtmfHelper(CallMedia callMedia, int numTones, string requestId)
    {
        _callMedia = callMedia;
        _numTones = numTones;
    }

    public ICanChooseRecognizeOptions FromParticipant<T>(string id)
        where T : CommunicationIdentifier
    {
        return this;
    }

    public IHandleDtmfResponse WithPrompt(string fileUri) => throw new NotImplementedException();

    public IHandleDtmfResponse OnPress<TTone>(Func<RecognizeCompleted, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction) 
        where TTone : IDtmfTone => throw new NotImplementedException();

    public IHandleDtmfResponse OnPress<TTone>(Action callbackAction) 
        where TTone : IDtmfTone => throw new NotImplementedException();

    public ValueTask ExecuteAsync() => throw new NotImplementedException();
    ICanRecognizeDtmfOptions ICanChooseRecognizeOptions.WithPrompt(string fileUri) => throw new NotImplementedException();
}