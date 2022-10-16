// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Microsoft.AspNetCore.Http;

namespace CallAutomation.Extensions;

public class EventGridWebhookValidation
{
    private readonly RequestDelegate _next;

    public EventGridWebhookValidation(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        httpContext.Request.EnableBuffering();
        BinaryData events = await BinaryData.FromStreamAsync(httpContext.Request.Body);
        httpContext.Request.Body.Position = 0;

        try
        {
            EventGridEvent[] eventGridEvents = EventGridEvent.ParseMany(events);

            foreach (EventGridEvent eventGridEvent in eventGridEvents)
            {
                // Handle system events
                if (eventGridEvent.TryGetSystemEventData(out object eventData))
                {
                    // Handle the subscription validation event
                    if (eventData is SubscriptionValidationEventData subscriptionValidationEventData)
                    {
                        var responseData = new SubscriptionValidationResponse()
                        {
                            ValidationResponse = subscriptionValidationEventData.ValidationCode,
                        };
                        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(responseData));
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        await _next(httpContext);
    }
}