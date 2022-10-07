// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;
using Azure.Communication.CallAutomation;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Interfaces;
using JasonShave.Azure.Communication.CallAutomation.Extensions.Models;

namespace JasonShave.Azure.Communication.CallAutomation.Extensions.Helpers;

internal sealed class CallFromAutomationCreateCallHelper :
    ICanCallFrom,
    ICallWithCallbackUri,
    IAnswerCallback
{
    private readonly CallAutomationClient _client;
    private readonly List<CommunicationIdentifier> _destinations = new ();
    private readonly PstnParticipantOptions? _pstnParticipantOptions;
    private readonly string _requestId;
    private CommunicationUserIdentifier _from;
    private Uri _callbackUri;

    internal CallFromAutomationCreateCallHelper(CallAutomationClient client, CommunicationIdentifier id, string requestId)
    {
        _client = client;
        _destinations.Add(id);
        _requestId = requestId;
    }

    internal CallFromAutomationCreateCallHelper(CallAutomationClient client, PhoneNumberIdentifier id, PstnParticipantOptions pstnParticipantOptions, string requestId)
    {
        _client = client;
        _pstnParticipantOptions = pstnParticipantOptions;
        _destinations.Add(id);
        _requestId = requestId;
    }

    public ICallWithCallbackUri From(string id)
    {
        _from = new CommunicationUserIdentifier(id);
        return this;
    }

    public IAnswerCallback WithCallbackUri(string callbackUri)
    {
        _callbackUri = new Uri($"{callbackUri}?=requestId={_requestId}");
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

    public IAnswerCallback OnCallConnected(Func<ValueTask> callbackFunction)
    {
        CallbackRegistry.Register<CallConnected>(_requestId, callbackFunction);
        return this;
    }

    public IAnswerCallback OnCallConnected(Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        CallbackRegistry.Register(_requestId, callbackFunction);
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
        var callSource = new CallSource(_from);

        if (_pstnParticipantOptions is not null)
        {
            callSource.CallerId = new PhoneNumberIdentifier(_pstnParticipantOptions.SourceCallerIdNumber);
        }

        var createCallOptions = new CreateCallOptions(callSource, _destinations, _callbackUri);
        await _client.CreateCallAsync(createCallOptions);
    }
}