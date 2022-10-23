// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

namespace CallAutomation.Extensions.Models;

public sealed class RecognizeOptions
{
    public int WaitForResponseInSeconds { get; set; }

    public int WaitBetweenTonesInSeconds { get; set; }

    public bool AllowInterruptPrompt { get; set; }

    public bool AllowInterruptExistingMediaOperation { get; set; }
}