// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Services;

internal abstract class HelperCallbackBase : ICallAutomationHelperCallback
{
    public IOperationContext Context { get; }

    public IEnumerable<Type> Types { get; }

    public CallAutomationCallbacks HelperCallbacks { get; }

    protected HelperCallbackBase(IOperationContext context, IEnumerable<Type> types)
    {
        Context = context;
        HelperCallbacks = new CallAutomationCallbacks(context);
        Types = types.ToList();

        CallbackRegistry.RegisterHelperCallback(this, Types);
    }
}