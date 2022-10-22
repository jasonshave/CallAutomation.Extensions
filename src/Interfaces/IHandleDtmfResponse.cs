// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface IHandleDtmfResponse
{
    IHandleDtmfResponse OnPress<TTone>(Func<RecognizeCompleted, CallConnection, CallMedia, CallRecording, IReadOnlyList<DtmfTone>, ValueTask> callback)
        where TTone : IDtmfTone;

    IHandleDtmfResponse OnPress<TTone, THandler>()
        where TTone : IDtmfTone
        where THandler : CallAutomationHandler;

    IHandleDtmfResponse OnPress<TTone>(Func<ValueTask> callback)
        where TTone : IDtmfTone;

    IHandleDtmfResponse OnPress<TTone>(Action callback)
        where TTone : IDtmfTone;

    IHandleDtmfTimeout OnInputTimeout(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback);

    IHandleDtmfTimeout OnInputTimeout<THandler>()
        where THandler : CallAutomationHandler;

    IHandleDtmfTimeout OnInterToneTimeout(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback);

    IHandleDtmfTimeout OnInterToneTimeout<THandler>()
        where THandler : CallAutomationHandler;

    IHandleDtmfTimeout OnInputTimeout(Func<ValueTask> callback);

    ValueTask ExecuteAsync();
}