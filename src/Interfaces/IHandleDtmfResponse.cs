// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface IHandleDtmfResponse
{
    IHandleDtmfResponse OnPress<TTone>(Func<RecognizeCompleted, CallConnection, CallMedia, CallRecording, ValueTask> callbackFunction)
        where TTone : IDtmfTone;

    IHandleDtmfResponse OnPress<TTone>(Action callbackAction)
        where TTone : IDtmfTone;

    ValueTask ExecuteAsync();
}