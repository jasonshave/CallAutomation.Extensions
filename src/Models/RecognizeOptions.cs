// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Models;

public sealed class RecognizeOptions
{
    public int MaxToneCount { get; set; } = 1;

    public int WaitForResponseInSeconds { get; set; } = 5;

    public int WaitBetweenTonesInSeconds { get; set; } = 2;

    public bool AllowInterruptPrompt { get; set; }

    public bool AllowInterruptExistingMediaOperation { get; set; }

    public List<DtmfTone> StopTones { get; } = new();
}