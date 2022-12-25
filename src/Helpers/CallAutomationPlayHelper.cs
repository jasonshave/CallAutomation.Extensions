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
    private readonly List<CommunicationIdentifier> _playToParticipants = new();
    private readonly PlayMediaOptions _playMediaOptions;
    private readonly string? _textToSpeak;

    internal CallAutomationPlayHelper(CallMedia callMedia, PlayMediaOptions playMediaOptions, string requestId)
        : base(requestId, _types)
    {
        _callMedia = callMedia;
        _playMediaOptions = playMediaOptions;
    }

    internal CallAutomationPlayHelper(CallMedia callMedia, string textToSpeak, string requestId)
        : base(requestId, _types)
    {
        _callMedia = callMedia;
        _textToSpeak = textToSpeak;
    }

    public IPlayMediaCallback ToParticipant(string rawId)
    {
        _playToParticipants.Add(CommunicationIdentifier.FromRawId(rawId));
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, PlayCompleted>(RequestId, $"On{nameof(PlayCompleted)}");
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<PlayCompleted>(RequestId, callbackFunction);
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted(Func<PlayCompleted, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<PlayCompleted>(RequestId, callbackFunction);
        return this;
    }

    public IPlayMediaCallback OnPlayFailed<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, PlayFailed>(RequestId, $"On{nameof(PlayFailed)}");
        return this;
    }

    public IPlayMediaCallback OnPlayFailed(Func<PlayFailed, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<PlayFailed>(RequestId, callbackFunction);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        PlaySource? playSource = null;
        PlayOptions playOptions = new()
        {
            OperationContext = RequestId
        };

        if (_playMediaOptions is not null)
        {
            playSource = new FileSource(new Uri(_playMediaOptions.FileUrl));
            playOptions.Loop = _playMediaOptions.Loop;
        }

        if (_textToSpeak is not null)
        {
            playSource = new TextSource(_textToSpeak);
        }

        if (_playToParticipants.Any())
        {
            await _callMedia.PlayAsync(playSource, _playToParticipants, playOptions);
        }
        else
        {
            await _callMedia.PlayToAllAsync(playSource, playOptions);
        }
    }
}