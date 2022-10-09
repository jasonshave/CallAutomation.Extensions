﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Messaging;

namespace CallAutomation.Extensions.Interfaces;

public interface ICallAutomationEventPublisher
{
    ValueTask PublishAsync(CloudEvent[] cloudEvents, string? requestId = default);
}