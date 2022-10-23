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

    IHandleDtmfTimeout OnFail<TRecognizeFail>(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback)
        where TRecognizeFail : IRecognizeDtmfFailed;

    IHandleDtmfTimeout OnFail<TRecognizeFail, THandler>()
        where TRecognizeFail : IRecognizeDtmfFailed
        where THandler : CallAutomationHandler;

    IHandleDtmfTimeout OnInputTimeout(Func<ValueTask> callback);

    ValueTask ExecuteAsync();
}