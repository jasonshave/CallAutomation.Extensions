// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using Azure.Messaging;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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
        //string? RequestIDFrom(string operationContext) =>
        //    (operationContext is null)
        //    ? null
        //    : (JsonSerializer.Deserialize<OperationContext>(operationContext)?.RequestId ?? operationContext);

        foreach (var cloudEvent in cloudEvents)
        {
            CallAutomationEventBase callAutomationEventBase = CallAutomationEventParser.Parse(cloudEvent);
            OperationContext? operationContext = callAutomationEventBase.OperationContext is null
                ? null
                : JsonSerializer.Deserialize<OperationContext>(callAutomationEventBase.OperationContext);

            if (callAutomationEventBase is CallConnected or CallDisconnected)
            {
                if (callAutomationEventBase.OperationContext is null)
                {
                    // OperationContext will be null for inbound calls
                    await _callAutomationEventHandler.Handle(callAutomationEventBase, operationContext, callAutomationEventBase.CorrelationId);
                }
                else
                {
                    // outbound calls won't have a correlation ID so we have to use the operation context.
                    await _callAutomationEventHandler.Handle(callAutomationEventBase, operationContext, operationContext?.RequestId);
                }

                return;
            }

            if (callAutomationEventBase is not RecognizeCompleted or RecognizeFailed)
            {
                await _callAutomationEventHandler.Handle(callAutomationEventBase, operationContext, operationContext?.RequestId);
                return;
            }

            await _callAutomationRecognizeDtmfHandler.Handle(callAutomationEventBase, operationContext?.RequestId);
        }
    }
}