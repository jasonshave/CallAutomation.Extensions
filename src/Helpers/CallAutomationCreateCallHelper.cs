// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure;
using Azure.Communication;
using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Extensions;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using CallAutomation.Extensions.Services;

namespace CallAutomation.Extensions.Helpers;

internal sealed class CallAutomationCreateCallHelper : HelperCallbackBase,
    ICreateCallFrom,
    ICreateCallWithCallbackUri,
    ICreateCallHandling
{
    private readonly CallAutomationClient _client;
    private readonly List<CommunicationIdentifier> _destinations = new();
    private string _from;
    private CallFromOptions? _callFromOptions;
    private Uri _callbackUri;
    private MediaStreamingOptions? _mediaStreamingOptions;

    internal CallAutomationCreateCallHelper(CallAutomationClient client, string to, string requestId)
        : base(requestId)
    {
        _client = client;
        _destinations.Add(to.ConvertToCommunicationIdentifier());
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

    public ICreateCallHandling WithInboundMediaStreaming(string streamingUri)
    {
        _mediaStreamingOptions = new MediaStreamingOptions(new Uri(streamingUri), MediaStreamingTransport.Websocket,
            MediaStreamingContent.Audio, MediaStreamingAudioChannel.Mixed);
        return this;
    }

    public ICreateCallHandling OnCallConnected<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, CallConnected>(RequestId, $"On{nameof(CallConnected)}");
        return this;
    }

    public ICreateCallHandling OnCallDisconnected<THandler>()
        where THandler : CallAutomationHandler
    {
        HelperCallbacks.AddHandlerCallback<THandler, CallDisconnected>(RequestId, $"On{nameof(CallDisconnected)}");
        return this;
    }

    public ICreateCallHandling OnCallConnected(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallConnected>(RequestId, callbackFunction);
        return this;
    }

    public ICreateCallHandling OnCallConnected(Func<CallConnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallConnected>(RequestId, callbackFunction);
        return this;
    }

    public ICreateCallHandling OnCallDisconnected(Func<ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallDisconnected>(RequestId, callbackFunction);
        return this;
    }

    public ICreateCallHandling OnCallDisconnected(Func<CallDisconnected, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
    {
        HelperCallbacks.AddDelegateCallback<CallDisconnected>(RequestId, callbackFunction);
        return this;
    }

    public async ValueTask<Response<CreateCallResult>> ExecuteAsync()
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
            OperationContext = RequestId,
        };

        if (_mediaStreamingOptions is not null)
            createCallOptions.MediaStreamingOptions = _mediaStreamingOptions;

        var result = await _client.CreateCallAsync(createCallOptions);
        return result;
    }
}