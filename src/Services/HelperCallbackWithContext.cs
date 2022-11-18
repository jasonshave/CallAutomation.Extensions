// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Services;

internal abstract class HelperCallbackWithContext : HelperCallbackBase
{
    protected string JSONContext => JsonSerializer.Serialize((object)Context);
    private IOperationContext Context { get; set; }

    protected HelperCallbackWithContext(string requestId, IEnumerable<Type> types)
        : base(requestId, types)
    {
        Context = new OperationContext { RequestId = requestId };
    }

    public void SetContext(IOperationContext context)
    {
        context.RequestId = RequestId;
        Context = context;
    }
}