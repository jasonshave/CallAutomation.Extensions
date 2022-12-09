// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;
using System.Text.Json;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationPlayHelper : HelperCallbackWithContext, IPlayMediaCallbackWithHandler
{
    private readonly CallMedia _callMedia;
    private readonly List<CommunicationIdentifier> _playToParticipants = new();
    private readonly PlayMediaOptions _playMediaOptions;

    internal CallAutomationPlayHelper(CallMedia callMedia, PlayMediaOptions playMediaOptions, string requestId)
        : base(requestId)
    {
        _callMedia = callMedia;
        _playMediaOptions = playMediaOptions;
    }

    public IPlayMediaCallback WithCallbackHandler(ICallbacksHandler handler)
    {
        CallbackHandler = handler;
        return this;
    }

    public IPlayMediaCallback ToParticipant(string rawId)
    {
        _playToParticipants.Add(CommunicationIdentifier.FromRawId(rawId));
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, PlayCompleted>(RequestId, $"On{nameof(PlayCompleted)}");
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted(Func<ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<PlayCompleted>(RequestId, callbackFunction);
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted(Func<PlayCompleted, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<PlayCompleted>(RequestId, callbackFunction);
        return this;
    }

    public IPlayMediaCallback OnPlayFailed<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, PlayFailed>(RequestId, $"On{nameof(PlayFailed)}");
        return this;
    }

    public IPlayMediaCallback OnPlayFailed(Func<PlayFailed, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<PlayFailed>(RequestId, callbackFunction);
        return this;
    }

    public IExecuteAsync WithContext(OperationContext context)
    {
        SetContext(context);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        if (_playToParticipants.Any())
        {
            await _callMedia.PlayAsync(new FileSource(new Uri(_playMediaOptions.FileUrl))
            {
                // todo: need to verify how this works
                PlaySourceId = RequestId,
            }, _playToParticipants, new PlayOptions()
            {
                OperationContext = JSONContext,
                Loop = _playMediaOptions.Loop,
            });
        }
        else
        {
            await _callMedia.PlayToAllAsync(new FileSource(new Uri(_playMediaOptions.FileUrl))
            {
                // todo: need to verify how this works
                PlaySourceId = RequestId,
            }, new PlayOptions()
            {
                OperationContext = JSONContext,
                Loop = _playMediaOptions.Loop,
            });
        }
    }
}