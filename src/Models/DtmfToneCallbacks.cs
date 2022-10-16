// Copyright (c) 2022 Jason Shave. All rights reserved.
// Licensed under the MIT License.

using CallAutomation.Extensions.Interfaces;

namespace CallAutomation.Extensions.Models;

internal sealed class DtmfToneCallbacks
{
    private readonly Dictionary<Type, List<Delegate>> _toneMap = new ();
    private readonly Dictionary<(string, Type), List<Delegate>> _callbacks = new ();

    public void AddCallback<TTone>(Delegate callback)
        where TTone : IDtmfTone
    {
        if (!_toneMap.ContainsKey(typeof(TTone)))
        {
            // new map
            _toneMap.Add(typeof(TTone), new List<Delegate> { callback });
            return;
        }

        // add map
        _toneMap[typeof(TTone)].Add(callback);
    }

    public void AddCallback<TEvent>(string requestId, Delegate callback)
    {
        if (!_callbacks.ContainsKey((requestId, typeof(TEvent))))
        {
            // new map
            _callbacks.Add((requestId, typeof(TEvent)), new List<Delegate> { callback });
            return;
        }

        // add map
        _callbacks[(requestId, typeof(TEvent))].Add(callback);
    }

    public List<Delegate> GetCallbacks(IDtmfTone tone) => _toneMap[tone.GetType()];

    public List<Delegate> GetCallbacks(string requestId, Type eventType) => _callbacks[(requestId, eventType)];
}