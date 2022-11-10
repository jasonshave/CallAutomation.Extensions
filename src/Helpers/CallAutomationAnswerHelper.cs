// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Services;
using CallNotificationService.Contracts.Models;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationAnswerHelper : HelperCallbackBase,
    IAnswerWithCallbackUri,
    IAnswerCallHandling
{
    private static readonly IEnumerable<Type> _types = new[] { typeof(CallConnected), typeof(CallDisconnected) };
    private readonly CallAutomationClient _client;
    private readonly string _incomingCallContext;
    private readonly Uri? _acceptCallUri;

    private Uri _midEventCallbackUri;

    internal CallAutomationAnswerHelper(CallAutomationClient client, IncomingCall incomingCall, string requestId)
        : base(requestId, _types)
    {
        _client = client;
        _incomingCallContext = incomingCall.IncomingCallContext;
    }

    //internal CallAutomationAnswerHelper(CallAutomationClient client, CallNotification callNotification, string requestId)
    //    : base(requestId, _types)
    //{
    //    _client = client;
    //    _midEventCallbackUri = new Uri(callNotification.MidCallEventsUri);
    //    //_incomingCallContext = callNotification.IncomingCallContext;
    //}

    internal CallAutomationAnswerHelper(CallAutomationClient client, CallNotification callNotification, Uri acceptCallUri, string requestId)
        : base(requestId, _types)
    {
        _client = client;
        _midEventCallbackUri = new Uri(callNotification.MidCallEventsUri);
        _acceptCallUri = acceptCallUri;
    }

    public IAnswerCallHandling WithCallbackUri(string callbackUri)
    {
        _midEventCallbackUri = new Uri(callbackUri);
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

    public async ValueTask<AnswerCallResult?> ExecuteAsync()
    {
        AnswerCallResult result;
        if (_acceptCallUri is null)
        {
            result = await _client.AnswerCallAsync(new AnswerCallOptions(_incomingCallContext, _midEventCallbackUri));
        }
        else
        {
            using var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = _acceptCallUri
            };

            await httpClient.SendAsync(httpRequest);
        }

        return null;
    }
}