// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Extensions.Helpers;
using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions;

public static class CallRecordingExtensions
{
    /// <summary>
    /// Initiates the call recording sequence while allowing <see cref="StartRecordingOptions"/> configuration.
    /// </summary>
    /// <param name="callRecording"></param>
    /// <param name="callLocator"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IStartRecordingCallbackkWithHandler StartRecording(this CallRecording callRecording, string correlationId, CallLocator callLocator, Action<StartRecordingOptions> options)
    {
        var recordingOptions = new StartRecordingOptions(callLocator);
        options(recordingOptions);
        return new CallAutomationStartRecordingHelper(callRecording, recordingOptions, correlationId);
    }
}