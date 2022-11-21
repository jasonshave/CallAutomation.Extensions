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
            {
                try
                {
                    if (callAutomationEventBase is RecognizeCompleted or RecognizeFailed)
                    {
                        // Handle Recognize API events
                        _logger.LogInformation("Handling {event} for call {callConnectionId} | CorrelationId: {correlationId}", callAutomationEventBase.GetType().Name, callAutomationEventBase.CallConnectionId, callAutomationEventBase.CorrelationId);
                        await _callAutomationRecognizeDtmfHandler.Handle(callAutomationEventBase);
                        return;
                    }

                    // Handle all other events
                    _logger.LogInformation("Handling {event} for call {callConnectionId} | CorrelationId: {correlationId}", callAutomationEventBase.GetType().Name, callAutomationEventBase.CallConnectionId, callAutomationEventBase.CorrelationId);
                    await _callAutomationEventHandler.Handle(callAutomationEventBase);
                    return;
                }
                catch (Exception e)
                {
                    _logger.LogError("There was a problem handling {event}. Message: {message} | CorrelationID: {correlationId} | CallConnectionId: {callConnectionId}", callAutomationEventBase.GetType().Name, e.Message, callAutomationEventBase.CorrelationId, callAutomationEventBase.CallConnectionId);
                    throw new ApplicationException(e.Message, e);
                }
            }
        }
    }
}