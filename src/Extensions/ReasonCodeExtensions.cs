// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Extensions;

internal static class ReasonCodeExtensions
{
    public static IRecognizeDtmfFailed? Convert(this ReasonCode reasonCode)
    {
        if (reasonCode == ReasonCode.RecognizeInitialSilenceTimedOut) return default(SilenceTimeout);
        if (reasonCode == ReasonCode.RecognizeInterDigitTimedOut) return default(InterDigitTimeout);
        if (reasonCode == ReasonCode.RecognizePlayPromptFailed) return default(PromptFailed);
        if (reasonCode == ReasonCode.RecognizeStopToneDetected) return default(StopToneDetected);

        return null;
    }
}