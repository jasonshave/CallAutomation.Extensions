// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Azure.Communication.CallAutomation;
using CallAutomation.Contracts;
using CallAutomation.Extensions.Helpers;
using CallAutomation.Extensions.Interfaces;
using CallNotificationService.Contracts.Models;
using CallNotificationService.Contracts.Requests;
using System.Text.Json;

namespace CallAutomation.Extensions
{
    public static class CallAutomationClientExtensions
    {
        private static readonly Uri _registrationUri = new (
            "https://callnotificationeventhandler-dev.azurewebsites.net/api/registration?code=uY2YJxc5zP6tMEqO7jzXlnCplT0mTxngKxBCn3GOtTYYAzFu-2K5zQ==");

        //private static readonly Uri _registrationUri = new("http://localhost:7013/api/registration");

        public static async Task<CallbackRegistrationDto> RegisterAsync(this CallAutomationClient client,
            Action<CreateRegistrationRequest> registrationRequest)
        {
            var request = new CreateRegistrationRequest();
            registrationRequest(request);

            using var httpClient = new HttpClient();
            var httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = _registrationUri
            };
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(request));
            var response = await httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode) throw new ApplicationException(response.ReasonPhrase);

            var stream = await response.Content.ReadAsStreamAsync();
            var registration = await JsonSerializer.DeserializeAsync<CallbackRegistrationDto>(stream);
            return registration;
        }

        public static IAnswerCallHandling Accept(this CallAutomationClient client, CallNotification callNotification)
        {
            var helper = new CallAutomationAnswerHelper(client, callNotification, callNotification.CorrelationId);
            return helper;
        }

        /// <summary>
        /// Initiates the outbound call sequence.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        public static ICreateCallFrom Call(this CallAutomationClient client, string id)
        {
            var helper =
                new CallAutomationCreateCallHelper(client, id, Guid.NewGuid().ToString());
            return helper;
        }

        /// <summary>
        /// Initiates the answer call sequence.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="incomingCall"></param>
        /// <returns></returns>
        public static IAnswerWithCallbackUri Answer(this CallAutomationClient client, IncomingCall incomingCall)
        {
            var helper = new CallAutomationAnswerHelper(client, incomingCall, incomingCall.CorrelationId);
            return helper;
        }

        /// <summary>
        /// Initiates the reject call sequence.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="incomingCall"></param>
        /// <returns></returns>
        public static IRejectCallWithReason Reject(this CallAutomationClient client, IncomingCall incomingCall)
        {
            var helper = new CallAutomationRejectHelper(client, incomingCall);
            return helper;
        }

        /// <summary>
        /// Initiates the redirect call sequence.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="incomingCall"></param>
        /// <returns></returns>
        public static IRedirectCall Redirect(this CallAutomationClient client, IncomingCall incomingCall)
        {
            var helper = new CallAutomationRedirectHelper(client, incomingCall);
            return helper;
        }
    }
}