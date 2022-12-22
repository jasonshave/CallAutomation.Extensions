// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure;
using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationAnswerHelper : HelperCallbackBase,
    IAnswerWithCallbackUriWithHandler,
    IAnswerCallHandling
{
    private readonly CallAutomationClient _client;
    private readonly string? _incomingCallContext;

    private Uri _callbackUri;
    private MediaStreamingOptions? _mediaStreamingOptions;

    internal CallAutomationAnswerHelper(CallAutomationClient client, IncomingCall incomingCall, string requestId)
        : base(requestId)
    {
        _client = client;
        _incomingCallContext = incomingCall.IncomingCallContext;
    }

    /// <summary>
    /// Used to handle <see cref="CallNotification"/> payload from the Call Notification Service
    /// </summary>
    /// <param name="client"></param>
    /// <param name="callNotification"></param>
    /// <param name="requestId"></param>
    internal CallAutomationAnswerHelper(CallAutomationClient client, CallNotification callNotification, string requestId)
        : base(requestId)
    {
        _client = client;
        _incomingCallContext = callNotification.IncomingCallContext;
        _callbackUri = new Uri(callNotification.MidCallEventsUri);
    }

    public IAnswerWithCallbackUri WithCallbackHandler(ICallbacksHandler handler)
    {
        CallbackHandler = handler;
        return this;
    }

    public IAnswerCallHandling WithCallbackUri(string callbackUri)
    {
        _callbackUri = new Uri(callbackUri);
        return this;
    }

    public IAnswerCallHandling WithInboundMediaStreaming(string streamingUri)
    {
        _mediaStreamingOptions = new MediaStreamingOptions(new Uri(streamingUri), MediaStreamingTransport.Websocket,
            MediaStreamingContent.Audio, MediaStreamingAudioChannel.Mixed);
        return this;
    }

    public IAnswerCallHandling OnCallConnected<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, CallConnected>(RequestId, $"On{nameof(CallConnected)}");
        return this;
    }

    public IAnswerCallHandling OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, CallDisconnected>(RequestId, $"On{nameof(CallDisconnected)}");
        return this;
    }

    public IAnswerCallHandling OnCallConnected(Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<CallConnected>(RequestId, callbackFunction);
        return this;
    }

    public IAnswerCallHandling OnCallConnected(Func<ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<CallConnected>(RequestId, callbackFunction);
        return this;
    }

    public IAnswerCallHandling OnCallDisconnected(Func<ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<CallDisconnected>(RequestId, callbackFunction);
        return this;
    }

    public IAnswerCallHandling OnCallDisconnected(Func<CallDisconnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<CallConnected>(RequestId, callbackFunction);
        return this;
    }

    public async ValueTask<Response<AnswerCallResult>> ExecuteAsync()
    {
        var answerCallOptions = new AnswerCallOptions(_incomingCallContext, _callbackUri);
        if (_mediaStreamingOptions is not null)
        {
            answerCallOptions.MediaStreamingOptions = _mediaStreamingOptions;
        }

        var result = await _client.AnswerCallAsync(answerCallOptions);
        return result;
    }
}