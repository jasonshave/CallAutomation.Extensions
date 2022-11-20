﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure;
using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationAnswerHelper : HelperCallbackBase,
    IAnswerWithCallbackUri,
    IAnswerCallHandling
{
    private static readonly IEnumerable<Type> _types = new[] { typeof(CallConnected), typeof(CallDisconnected) };
    private readonly CallAutomationClient _client;
    private readonly string? _incomingCallContext;

    private Uri? _callbackUri;

    internal CallAutomationAnswerHelper(CallAutomationClient client, IncomingCall incomingCall)
        : base(_types, new DefaultOperationContext())
    {
        _client = client;
        _incomingCallContext = incomingCall.IncomingCallContext;
    }

    internal CallAutomationAnswerHelper(CallAutomationClient client, CallNotification callNotification)
        : base(_types)
    {
        _client = client;
        _incomingCallContext = callNotification.IncomingCallContext;
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
        AddHandlerCallback<THandler, CallConnected>($"On{nameof(CallConnected)}", typeof(CallConnected), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording), typeof(IOperationContext));
        return this;
    }

    public IAnswerCallHandling OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler
    {
        AddHandlerCallback<THandler, CallDisconnected>($"On{nameof(CallDisconnected)}", typeof(CallDisconnected), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording), typeof(IOperationContext));
        return this;
    }

    public IAnswerCallHandling OnCallConnected(Func<CallConnected, CallConnection, CallMedia, CallRecording, IOperationContext, ValueTask> callbackFunction)
    {
        AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public IAnswerCallHandling OnCallConnected(Func<ValueTask> callbackFunction)
    {
        AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public IAnswerCallHandling OnCallDisconnected(Func<ValueTask> callbackFunction)
    {
        AddDelegateCallback<CallDisconnected>(callbackFunction);
        return this;
    }

    public IAnswerCallHandling OnCallDisconnected(Func<CallDisconnected, CallConnection, CallMedia, CallRecording, IOperationContext, ValueTask> callbackFunction)
    {
        AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public IAnswerCallHandling WithContext(IOperationContext context)
    {
        OperationContext = context;
        return this;
    }

    public async ValueTask<AnswerCallResult> ExecuteAsync()
    {
        Response<AnswerCallResult> result = await _client.AnswerCallAsync(new AnswerCallOptions(_incomingCallContext, _callbackUri));
        return result;
    }
}