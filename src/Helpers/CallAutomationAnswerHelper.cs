// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure;
using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationAnswerHelper : HelperCallbackBase,
    IAnswerWithCallbackUri,
    IAnswerCallHandling
{
    private static readonly IEnumerable<Type> _types = new[] { typeof(CallConnected), typeof(CallDisconnected) };
    private readonly CallAutomationClient _client;
    private readonly IncomingCall? _incomingCall;
    private readonly CallNotification? _callNotification;

    private Uri _callbackUri;

    /// <summary>
    /// Used to handle Event Grid data directly
    /// </summary>
    /// <param name="client"></param>
    /// <param name="incomingCall"></param>
    /// <param name="requestId"></param>
    internal CallAutomationAnswerHelper(CallAutomationClient client, IncomingCall incomingCall, string requestId)
        : base(requestId, _types)
    {
        _client = client;
        _incomingCall = incomingCall;
    }

    /// <summary>
    /// Used to handle <see cref="CallNotification"/> payload from the Call Notification Service
    /// </summary>
    /// <param name="client"></param>
    /// <param name="callNotification"></param>
    /// <param name="requestId"></param>
    internal CallAutomationAnswerHelper(CallAutomationClient client, CallNotification callNotification, string requestId)
        : base(requestId, _types)
    {
        _client = client;
        _callNotification = callNotification;
        _callbackUri = new Uri(callNotification.MidCallEventsUri);
    }

    public IAnswerCallHandling WithCallbackUri(string callbackUri)
    {
        _callbackUri = new Uri(callbackUri);
        return this;
    }

    public IAnswerCallHandling OnCallConnected<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, CallConnected>($"On{nameof(CallConnected)}", typeof(CallConnected), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public IAnswerCallHandling OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, CallDisconnected>($"On{nameof(CallDisconnected)}", typeof(CallDisconnected), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public IAnswerCallHandling OnCallConnected(Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public IAnswerCallHandling OnCallConnected(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public IAnswerCallHandling OnCallDisconnected(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallDisconnected>(callbackFunction);
        return this;
    }

    public IAnswerCallHandling OnCallDisconnected(Func<CallDisconnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public async ValueTask<AnswerCallResult> ExecuteAsync()
    {
        Response<AnswerCallResult> result = await _client.AnswerCallAsync(new AnswerCallOptions(_incomingCall.IncomingCallContext, _callbackUri));
        return result;
    }
}