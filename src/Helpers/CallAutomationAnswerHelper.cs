// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationAnswerHelper : HelperCallbackBase,
    IAnswerWithCallbackUri,
    ICallConnectionHandler,
    ICanExecuteAsync
{
    private readonly CallAutomationClient _client;
    private readonly IncomingCall _incomingCall;
    private Uri _callbackUri;

    internal CallAutomationAnswerHelper(CallAutomationClient client, IncomingCall incomingCall, string requestId)
        : base(requestId)
    {
        _client = client;
        _incomingCall = incomingCall;
    }

    public ICallConnectionHandler WithCallbackUri(string callbackUri)
    {
        _callbackUri = new Uri(callbackUri);
        return this;
    }

    public ICallConnectionHandler OnCallConnected<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<CallConnected, THandler>($"On{nameof(CallConnected)}", typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public ICallConnectionHandler OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<CallDisconnected, THandler>($"On{nameof(CallDisconnected)}", typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public ICallConnectionHandler OnCallConnected(Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public ICallConnectionHandler OnCallConnected(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public ICallConnectionHandler OnCallDisconnected(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallDisconnected>(callbackFunction);
        return this;
    }

    public ICallConnectionHandler OnCallDisconnected(Func<CallDisconnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        CallbackRegistry.RegisterHelperCallback(this, new[] { typeof(CallConnected), typeof(CallDisconnected) });

        await _client.AnswerCallAsync(new AnswerCallOptions(_incomingCall.IncomingCallContext, _callbackUri));
    }
}