﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Extensions;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationCreateCallHelper : HelperCallbackBase,
    ICallFrom,
    ICallWithCallbackUri,
    ICallConnectionHandler
{
    private readonly CallAutomationClient _client;
    private readonly List<CommunicationIdentifier> _destinations = new ();
    private string _from;
    private CallFromOptions? _callFromOptions;
    private Uri _callbackUri;

    internal CallAutomationCreateCallHelper(CallAutomationClient client, string to, string requestId)
        : base(requestId)
    {
        _client = client;
        _destinations.Add(to.ConvertToCommunicationIdentifier());
    }

    public ICallWithCallbackUri From(string from)
    {
        _from = from;
        return this;
    }

    public ICallWithCallbackUri From(string from, Action<CallFromOptions> options)
    {
        _from = from;

        var callFromOptions = new CallFromOptions();
        options(callFromOptions);
        _callFromOptions = callFromOptions;

        return this;
    }

    public ICallConnectionHandler WithCallbackUri(string callbackUri)
    {
        _callbackUri = new Uri($"{callbackUri}?=requestId={RequestId}");
        return this;
    }

    public ICallConnectionHandler OnCallConnected<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, CallConnected>($"On{nameof(CallConnected)}", typeof(CallConnected), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public ICallConnectionHandler OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, CallDisconnected>($"On{nameof(CallDisconnected)}", typeof(CallConnected), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording));
        return this;
    }

    public ICallConnectionHandler OnCallConnected(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public ICallConnectionHandler OnCallConnected(Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
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
        HelperCallbacks.AddDelegateCallback<CallDisconnected>(callbackFunction);
        return this;
    }

    public async ValueTask ExecuteAsync()
    {
        CallbackRegistry.RegisterHelperCallback(this, new[] { typeof(CallConnected), typeof(CallDisconnected) });

        var callSource = new CallSource(new CommunicationUserIdentifier(_callFromOptions.ApplicationId));
        if (_callFromOptions is not null)
        {
            callSource.DisplayName = _callFromOptions.CallerDisplayName;
        }

        if (_destinations.OfType<PhoneNumberIdentifier>().Any())
        {
            callSource.CallerId = new PhoneNumberIdentifier(_from);
        }

        var createCallOptions = new CreateCallOptions(callSource, _destinations, _callbackUri)
        {
            OperationContext = RequestId,
        };
        await _client.CreateCallAsync(createCallOptions);
    }
}