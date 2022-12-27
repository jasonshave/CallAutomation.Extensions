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

    public async ValueTask PublishAsync(CloudEvent[] cloudEvents)
    {
        foreach (var cloudEvent in cloudEvents)
        {
            CallAutomationEventBase callAutomationEventBase = CallAutomationEventParser.Parse(cloudEvent);
            _logger.LogDebug("Received event: {eventName}", callAutomationEventBase.GetType().Name);

            if (callAutomationEventBase is CallConnected or CallDisconnected)
            {
                if (callAutomationEventBase.OperationContext is null)
                {
                    // OperationContext will be null for inbound calls
                    await _callAutomationEventHandler.Handle(callAutomationEventBase, callAutomationEventBase.CorrelationId);
                }
                else
                {
                    // outbound calls won't have a correlation ID so we have to use the operation context.
                    await _callAutomationEventHandler.Handle(callAutomationEventBase, callAutomationEventBase.OperationContext);
                }

                return;
            }

            // handle recognition events
            if (callAutomationEventBase is RecognizeCompleted or RecognizeFailed)
            {
                await _callAutomationRecognizeDtmfHandler.Handle(callAutomationEventBase, callAutomationEventBase.OperationContext);
                return;
            }

            // handle all other events
            await _callAutomationEventHandler.Handle(callAutomationEventBase, callAutomationEventBase.OperationContext);
        }
    }
}