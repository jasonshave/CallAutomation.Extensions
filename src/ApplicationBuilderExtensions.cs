// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;

namespace CallAutomation.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseEventGridWebHookValidationMiddleware(this IApplicationBuilder app)
    {
        // experimental
        app.UseMiddleware<EventGridWebhookValidation>();
        return app;
    }
}