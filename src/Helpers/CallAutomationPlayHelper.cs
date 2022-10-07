// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Interfaces;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Models;

namespace JasonShave.Azure.Communication.CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationPlayHelper :
    IPlayMediaCallback,
    ICanExecuteAsync
{
    private readonly CallMedia _callMedia;
    private readonly string _requestId;
    private readonly List<CommunicationIdentifier> _playToParticipants = new ();
    private readonly PlayMediaOptions _playMediaOptions;

    internal CallAutomationPlayHelper(CallMedia callMedia, PlayMediaOptions playMediaOptions, string requestId)
    {
        _callMedia = callMedia;
        _playMediaOptions = playMediaOptions;
        _requestId = requestId;
    }

    public IPlayMediaCallback ToParticipant<T>(string id)
        where T : CommunicationIdentifier
    {
        _playToParticipants.Add(CommunicationIdentifier.FromRawId(id));
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackRegistry.Register<THandler, PlayCompleted>(_requestId);
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted(Func<ValueTask> callbackFunction)
    {
        CallbackRegistry.Register<PlayCompleted>(_requestId, callbackFunction);
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted(Func<PlayCompleted, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackRegistry.Register(_requestId, callbackFunction);
        return this;
    }

    public IPlayMediaCallback OnPlayFailed<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackRegistry.Register<THandler, PlayFailed>(_requestId);
        return this;
    }

    public IPlayMediaCallback OnPlayFailed(Func<PlayFailed, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackRegistry.Register(_requestId, callbackFunction);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        if (_playToParticipants.Any())
        {
            await _callMedia.PlayAsync(new FileSource(new Uri(_playMediaOptions.FileUrl)), _playToParticipants, new PlayOptions()
            {
                OperationContext = _requestId,
                Loop = _playMediaOptions.Loop,
            });
        }
        else
        {
            await _callMedia.PlayToAllAsync(new FileSource(new Uri(_playMediaOptions.FileUrl)), new PlayOptions()
            {
                OperationContext = _requestId,
                Loop = _playMediaOptions.Loop,
            });
        }
    }
}