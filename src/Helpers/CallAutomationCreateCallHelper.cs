// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationCreateCallHelper : HelperCallbackBase,
    ICreateCallFrom,
    ICreateCallWithCallbackUri,
    ICreateCallHandling
{
    private static readonly IEnumerable<Type> _types = new[] { typeof(CallConnected), typeof(CallDisconnected) };
    private readonly CallAutomationClient _client;
    private readonly CallTarget _callTarget;
    private CommunicationIdentifier _from;
    private Uri _callbackUri;

    internal CallAutomationCreateCallHelper(CallAutomationClient client, CallTarget callTarget)
        : base(_types)
    {
        _client = client;
        _callTarget = callTarget;
    }

    public ICreateCallWithCallbackUri From(string applicationId)
    {
        _from = CommunicationIdentifier.FromRawId(applicationId);
        return this;
    }

    public ICreateCallHandling WithCallbackUri(string callbackUri)
    {
        _callbackUri = new Uri(callbackUri);
        return this;
    }

    public ICreateCallHandling OnCallConnected<THandler>()
        where THandler : CallAutomationHandler
    {
        AddHandlerCallback<THandler, CallConnected>($"On{nameof(CallConnected)}", typeof(CallConnected), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording), typeof(OperationContext));
        return this;
    }

    public ICreateCallHandling OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler
    {
        AddHandlerCallback<THandler, CallDisconnected>($"On{nameof(CallDisconnected)}", typeof(CallDisconnected), typeof(CallConnection), typeof(CallMedia), typeof(CallRecording), typeof(OperationContext));
        return this;
    }

    public ICreateCallHandling OnCallConnected(Func<ValueTask> callbackFunction)
    {
        AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public ICreateCallHandling OnCallConnected(Func<CallConnected, CallConnection, CallMedia, CallRecording, OperationContext, ValueTask> callbackFunction)
    {
        AddDelegateCallback<CallConnected>(callbackFunction);
        return this;
    }

    public ICreateCallHandling OnCallDisconnected(Func<ValueTask> callbackFunction)
    {
        AddDelegateCallback<CallDisconnected>(callbackFunction);
        return this;
    }

    public ICreateCallHandling OnCallDisconnected(Func<CallDisconnected, CallConnection, CallMedia, CallRecording, OperationContext, ValueTask> callbackFunction)
    {
        AddDelegateCallback<CallDisconnected>(callbackFunction);
        return this;
    }

    public ICreateCallHandling WithContext(OperationContext context)
    {
        OperationContext = context;
        return this;
    }

    public async ValueTask<CreateCallResult> ExecuteAsync()
    {
        OperationContext ??= new OperationContext();

        var callSource = new CallSource(_from);
        if (_callTarget.DisplayName is not null)
        {
            callSource.DisplayName = _callTarget.DisplayName;
        }

        if (_callTarget.Target is PhoneNumberIdentifier)
        {
            callSource.CallerId = _callTarget.CallerId;
        }

        var targets = new List<CommunicationIdentifier>()
        {
            _callTarget.Target,
        };

        var createCallOptions = new CreateCallOptions(callSource, targets, _callbackUri)
        {
            OperationContext = GetSerializedContext(),
        };

        var result = await _client.CreateCallAsync(createCallOptions);
        return result;
    }
}