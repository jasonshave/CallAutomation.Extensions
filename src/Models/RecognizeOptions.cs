// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Models;

public sealed class RecognizeOptions
{
    public int MaxToneCount { get; set; } = 1;

    public int WaitForResponseInSeconds { get; set; } = 5;

    public int WaitBetweenTonesInSeconds { get; set; } = 2;

    public bool AllowInterruptPrompt { get; set; } = false;

    public bool AllowInterruptExistingMediaOperation { get; set; } = false;
}