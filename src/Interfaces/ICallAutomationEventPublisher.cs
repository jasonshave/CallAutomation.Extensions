// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Messaging;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallAutomationEventPublisher
{
    /// <summary>
    /// Inject and use to send the Webhook callback <see cref="CloudEvent"/> collection and invoke handlers and/or delegates.
    /// </summary>
    /// <param name="cloudEvents"></param>
    /// <param name="requestId"></param>
    /// <returns></returns>
    ValueTask PublishAsync(CloudEvent[] cloudEvents);
}