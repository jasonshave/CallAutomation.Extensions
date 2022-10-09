// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationAnswerHelper :
    IAnswerWithCallbackUri,
    IAnswerCallback,
    ICanExecuteAsync
{
    private readonly CallAutomationClient _client;
    private readonly IncomingCall _incomingCall;
    private readonly string _requestId;

    private Uri _callbackUri;

    internal CallAutomationAnswerHelper(CallAutomationClient client, IncomingCall incomingCall, string requestId)
    {
        _requestId = requestId;
        _client = client;
        _incomingCall = incomingCall;
    }

    public IAnswerCallback WithCallbackUri(string callbackUri)
    {
        _callbackUri = new Uri(callbackUri);
        return this;
    }

    public IAnswerCallback OnCallConnected<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackRegistry.Register<THandler, CallConnected>(_requestId);
        return this;
    }

    public IAnswerCallback OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackRegistry.Register<THandler, CallDisconnected>(_requestId);
        return this;
    }

    public IAnswerCallback OnCallConnected(Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackRegistry.Register(_requestId, callbackFunction);
        return this;
    }

    public IAnswerCallback OnCallConnected(Func<ValueTask> callbackFunction)
    {
        CallbackRegistry.Register<CallConnected>(_requestId, callbackFunction);
        return this;
    }

    public IAnswerCallback OnCallDisconnected(Func<ValueTask> callbackFunction)
    {
        CallbackRegistry.Register<CallDisconnected>(_requestId, callbackFunction);
        return this;
    }

    public IAnswerCallback OnCallDisconnected(Func<CallDisconnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackRegistry.Register(_requestId, callbackFunction);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        await _client.AnswerCallAsync(new AnswerCallOptions(_incomingCall.IncomingCallContext, _callbackUri));
    }
}