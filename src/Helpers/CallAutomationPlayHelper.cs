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

    internal CallAutomationPlayHelper(CallMedia callMedia, PlayMediaOptions playMediaOptions)
        : base(_types)
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
        AddHandlerCallback<THandler, PlayCompleted>($"On{nameof(PlayCompleted)}", typeof(PlayCompleted), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted(Func<ValueTask> callbackFunction)
    {
        AddDelegateCallback<PlayCompleted>(callbackFunction);
        return this;
    }

    public IPlayMediaCallback OnPlayCompleted(Func<PlayCompleted, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        AddDelegateCallback<PlayCompleted>(callbackFunction);
        return this;
    }

    public IPlayMediaCallback OnPlayFailed<THandler>()
        where THandler : CallAutomationHandler
    {
        AddHandlerCallback<THandler, PlayFailed>($"On{nameof(PlayFailed)}", typeof(PlayFailed), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public IPlayMediaCallback OnPlayFailed(Func<PlayFailed, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        AddDelegateCallback<PlayFailed>(callbackFunction);
        return this;
    }

    public IPlayMediaCallback WithContext(OperationContext context)
    {
        SetContext(context);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        if (_playToParticipants.Any())
        {
            await _callMedia.PlayAsync(new FileSource(new Uri(_playMediaOptions.FileUrl)), _playToParticipants, new PlayOptions()
            {
                OperationContext = GetSerializedContext(),
                Loop = _playMediaOptions.Loop,
            });
        }
        else
        {
            await _callMedia.PlayToAllAsync(new FileSource(new Uri(_playMediaOptions.FileUrl)), new PlayOptions()
            {
                OperationContext = GetSerializedContext(),
                Loop = _playMediaOptions.Loop,
            });
        }
    }
}