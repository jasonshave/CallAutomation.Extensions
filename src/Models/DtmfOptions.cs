// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication;

namespace CallAutomation.Extensions.Models;

public sealed class DtmfOptions
{
    public CommunicationIdentifier Target { get; set; }

    public int TonesToCollect { get; set; }
}