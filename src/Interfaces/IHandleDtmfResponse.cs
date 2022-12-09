// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;

namespace CallAutomation.Extensions.Interfaces;

public interface IHandleDtmfResponseWithHandler : IWithCallbackHandler<IHandleDtmfResponse>, IHandleDtmfResponse
{
}

public interface IHandleDtmfResponse : ICallbackContext
{
    /// <summary>
    /// Specifies the callback delegate when the requested <see cref="IDtmfTone"/> is received.
    /// </summary>
    /// <typeparam name="TTone"></typeparam>
    /// <param name="callback"></param>
    IHandleDtmfResponse OnPress<TTone>(Func<RecognizeCompleted, CallConnection, CallMedia, CallRecording, IReadOnlyList<DtmfTone>, ValueTask> callback)
        where TTone : IDtmfTone;

    /// <summary>
    /// Specifies the handler to invoke when the requested <see cref="IDtmfTone"/> is received.
    /// </summary>
    /// <typeparam name="TTone"></typeparam>
    /// <typeparam name="THandler"></typeparam>
    /// <returns></returns>
    IHandleDtmfResponse OnPress<TTone, THandler>()
        where TTone : IDtmfTone
        where THandler : CallAutomationHandler;

    /// <summary>
    /// Specifies the callback delegate when the requested <see cref="IDtmfTone"/> is received.
    /// </summary>
    /// <typeparam name="TTone"></typeparam>
    /// <param name="callback"></param>
    IHandleDtmfResponse OnPress<TTone>(Func<ValueTask> callback)
        where TTone : IDtmfTone;

    /// <summary>
    /// Specifies the handler to invoke when more than one tone is received.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <returns></returns>
    IHandleDtmfResponse OnRecognizeCompleted<THandler>()
        where THandler : CallAutomationHandler;

    /// <summary>
    /// Specifies the callback delegate when more than one tone is received.
    /// </summary>
    /// <param name="callback"></param>
    IHandleDtmfResponse OnRecognizeCompleted(Func<ValueTask> callback);

    /// <summary>
    /// Specifies the callback delegate when the collection of DTMF fails.
    /// </summary>
    /// <typeparam name="TRecognizeFail"></typeparam>
    /// <param name="callback"></param>
    IHandleDtmfTimeout OnFail<TRecognizeFail>(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback)
        where TRecognizeFail : IRecognizeDtmfFailed;

    /// <summary>
    /// Specifies the callback delegate when the collection of DTMF fails.
    /// </summary>
    /// <typeparam name="TRecognizeFail"></typeparam>
    /// <typeparam name="THandler"></typeparam>
    IHandleDtmfTimeout OnFail<TRecognizeFail, THandler>()
        where TRecognizeFail : IRecognizeDtmfFailed
        where THandler : CallAutomationHandler;

    /// <summary>
    /// Specifies the callback delegate when the collection of DTMF fails.
    /// </summary>
    /// <param name="callback"></param>
    IHandleDtmfTimeout OnRecognizeFailed(Func<RecognizeFailed, CallConnection, CallMedia, CallRecording, ValueTask> callback);

    /// <summary>
    /// Specifies the callback delegate when the collection of DTMF fails.
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    IHandleDtmfTimeout OnRecognizeFailed<THandler>()
        where THandler : CallAutomationHandler;
}