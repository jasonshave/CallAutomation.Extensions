// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationPlayHelper : HelperCallbackBase, IPlayMediaCallback
{
    private static IEnumerable<Type> _types = new[] { typeof(PlayCompleted), typeof(PlayFailed) };
    private readonly CallMedia _callMedia;
    private readonly List<CommunicationIdentifier> _playToParticipants = new ();
    private readonly PlayMediaOptions _playMediaOptions;

    internal CallAutomationPlayHelper(CallMedia callMedia, PlayMediaOptions playMediaOptions, string requestId)
        : base(requestId, _types)
    {
        _callMedia = callMedia;
        _playMediaOptions = playMediaOptions;
    }

    public IPlayMediaCallback ToParticipant(string rawId)
    {
        _playToParticipants.Add(CommunicationIdentifier.FromRawId(rawId));
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, PlayCompleted>($"On{nameof(PlayCompleted)}", typeof(PlayCompleted), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<PlayCompleted>(callbackFunction);
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted(Func<PlayCompleted, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<PlayCompleted>(callbackFunction);
        return this;
    }

    public IPlayMediaCallback OnPlayFailed<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, PlayCompleted>($"On{nameof(PlayFailed)}", typeof(PlayFailed), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public IPlayMediaCallback OnPlayFailed(Func<PlayFailed, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<PlayFailed>(callbackFunction);
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
                OperationContext = RequestId,
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
                OperationContext = RequestId,
                Loop = _playMediaOptions.Loop,
            });
        }
    }
}