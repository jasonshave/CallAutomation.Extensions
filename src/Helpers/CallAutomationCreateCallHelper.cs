// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Extensions;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationCreateCallHelper : HelperCallbackWithContext,
    ICreateCallFrom,
    ICreateCallWithCallbackUri,
    ICreateCallHandling
{
    private static readonly IEnumerable<Type> _types = new[] { typeof(CallConnected), typeof(CallDisconnected) };
    private readonly CallAutomationClient _client;
    private readonly List<CommunicationIdentifier> _destinations = new();
    private string _from;
    private CallFromOptions? _callFromOptions;
    private Uri _callbackUri;

    internal CallAutomationCreateCallHelper(CallAutomationClient client, string to, string requestId)
        : base(requestId, _types)
    {
        _client = client;
        _destinations.Add(to.ConvertToCommunicationIdentifier());
    }

    public ICreateCallFrom WithCallbackHandler(ICallbacksHandler handler)
    {
        CallbackHandler = handler;
        return this;
    }

    public ICreateCallWithCallbackUri From(string id, Action<CallFromOptions> options)
    {
        _from = id;

        var callFromOptions = new CallFromOptions();
        options(callFromOptions);
        _callFromOptions = callFromOptions;

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
        CallbackHandler.AddHandlerCallback<THandler, CallConnected>(RequestId, $"On{nameof(CallConnected)}");
        return this;
    }

    public ICreateCallHandling OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler
    {
        CallbackHandler.AddHandlerCallback<THandler, CallDisconnected>(RequestId, $"On{nameof(CallDisconnected)}");
        return this;
    }

    public ICreateCallHandling OnCallConnected(Func<ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<CallConnected>(RequestId, callbackFunction);
        return this;
    }

    public ICreateCallHandling OnCallConnected(Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<CallConnected>(RequestId, callbackFunction);
        return this;
    }

    public ICreateCallHandling OnCallDisconnected(Func<ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<CallDisconnected>(RequestId, callbackFunction);
        return this;
    }

    public ICreateCallHandling OnCallDisconnected(Func<CallDisconnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackHandler.AddDelegateCallback<CallDisconnected>(RequestId, callbackFunction);
        return this;
    }

    IExecuteAsync<CreateCallResult> ICallbackContext<IExecuteAsync<CreateCallResult>>.WithContext(IOperationContext context)
    {
        WithContext(context);
        return this;
    }

    public async ValueTask<CreateCallResult> ExecuteAsync()
    {
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
            OperationContext = JSONContext,
        };

        var result = await _client.CreateCallAsync(createCallOptions);
        return result;
    }
}