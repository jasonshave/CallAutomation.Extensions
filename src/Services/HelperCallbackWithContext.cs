﻿// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using CallAutomation.Extensions.Interfaces;
using CallAutomation.Extensions.Models;

namespace CallAutomation.Extensions.Services;

internal abstract class HelperCallbackWithContext : HelperCallbackBase
{
    protected string JSONContext => JsonSerializer.Serialize(Context);
    private OperationContext Context { get; set; }

    protected HelperCallbackWithContext(string requestId, IEnumerable<Type> types)
        : base(requestId, types)
    {
        Context = new OperationContext { RequestId = requestId };
    }

    public void SetContext(OperationContext context)
    {
        context.RequestId = this.Context.RequestId;
        this.Context = context;
    }
}