// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using Azure.Messaging;
using CallAutomation.Extensions.Interfaces;
using Microsoft.Extensions.Logging;

namespace CallAutomation.Extensions.Services;

internal sealed class CallAutomationEventPublisher : ICallAutomationEventPublisher
{
    private readonly ICallAutomationEventHandler _callAutomationEventHandler;
    private readonly ICallAutomationRecognizeDtmfHandler _callAutomationRecognizeDtmfHandler;
    private readonly ILogger<CallAutomationEventPublisher> _logger;

    public CallAutomationEventPublisher(
        ICallAutomationEventHandler callAutomationEventHandler,
        ICallAutomationRecognizeDtmfHandler callAutomationRecognizeDtmfHandler,
        ILogger<CallAutomationEventPublisher> logger)
    {
        _callAutomationEventHandler = callAutomationEventHandler;
        _callAutomationRecognizeDtmfHandler = callAutomationRecognizeDtmfHandler;
        _logger = logger;
    }

    public async ValueTask PublishAsync(CloudEvent[] cloudEvents, string? requestId = default)
    {
        foreach (var cloudEvent in cloudEvents)
        {
            CallAutomationEventBase callAutomationEventBase = CallAutomationEventParser.Parse(cloudEvent);

            if (callAutomationEventBase is CallConnected or CallDisconnected)
            {
                if (callAutomationEventBase.CorrelationId is not null)
                {
                    // inbound calls will have a correlationId on the base class used to correlate the callback
                    await _callAutomationEventHandler.Handle(callAutomationEventBase, callAutomationEventBase.CorrelationId);
                }
                else
                {
                    // outbound calls won't have a correlation ID so we have to use the operation context.
                    await _callAutomationEventHandler.Handle(callAutomationEventBase, callAutomationEventBase.OperationContext);
                }
            }

            if (callAutomationEventBase is RecognizeCompleted or RecognizeFailed)
            {
                await _callAutomationRecognizeDtmfHandler.Handle(callAutomationEventBase, callAutomationEventBase.OperationContext);
            }
            else
            {
                await _callAutomationEventHandler.Handle(callAutomationEventBase, callAutomationEventBase.OperationContext);
            }
        }
    }
}