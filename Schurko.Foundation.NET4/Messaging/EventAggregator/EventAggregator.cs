
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Schurko.Foundation.Messaging.EventAggregator
{
  public class EventAggregator : IEventAggregator
  {
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>> _hub = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>>();
    private static readonly object _hubLock = new object();
    private readonly Type _containerType;

    public static IEventAggregator GetInstance(object container) => (IEventAggregator) new Schurko.Foundation.Messaging.EventAggregator.EventAggregator(container.GetType());

    public EventAggregator(Type containerType) => this._containerType = containerType;

    public void Subscribe<TEvent>(Action<Type, TEvent> action)
    {
      lock (Schurko.Foundation.Messaging.EventAggregator.EventAggregator._hubLock)
      {
        ConcurrentDictionary<Type, object> orAdd = Schurko.Foundation.Messaging.EventAggregator.EventAggregator._hub.GetOrAdd(typeof (TEvent), (Func<Type, ConcurrentDictionary<Type, object>>) (type => new ConcurrentDictionary<Type, object>()));
        object obj;
        if (!orAdd.TryGetValue(this._containerType, out obj))
          orAdd.TryAdd(this._containerType, (object) action);
        else if (!(obj is Action<Type, TEvent> action2))
        {
          orAdd[this._containerType] = (object) action;
        }
        else
        {
          Action<Type, TEvent> action1 = action2 + action;
          orAdd[this._containerType] = (object) action1;
        }
      }
    }

    public void Unsubscribe<TEvent>(Action<Type, TEvent> action = null)
    {
      lock (Schurko.Foundation.Messaging.EventAggregator.EventAggregator._hubLock)
      {
        ConcurrentDictionary<Type, object> concurrentDictionary;
        if (!Schurko.Foundation.Messaging.EventAggregator.EventAggregator._hub.TryGetValue(typeof (TEvent), out concurrentDictionary))
          return;
        if (action == null)
        {
          if (!concurrentDictionary.TryRemove(this._containerType, out object _))
            return;
        }
        else
        {
          object obj;
          if (!concurrentDictionary.TryGetValue(this._containerType, out obj) || !(obj is Action<Type, TEvent> action1))
            return;
          Action<Type, TEvent> action2 = action1 - action;
          concurrentDictionary[this._containerType] = (object) action2;
        }
        if (!concurrentDictionary.IsEmpty)
          return;
        Schurko.Foundation.Messaging.EventAggregator.EventAggregator._hub.TryRemove(typeof (TEvent), out concurrentDictionary);
      }
    }

    public void Publish<TEvent>() => this.Publish<TEvent>(default (TEvent));

    public void Publish<TEvent>(TEvent publishedEvent) => this.Publish<TEvent>(publishedEvent, (Type) null);

    public void Publish<TEvent>(TEvent publishedEvent, Type target)
    {
      ConcurrentDictionary<Type, object> source;
      if (!Schurko.Foundation.Messaging.EventAggregator.EventAggregator._hub.TryGetValue(typeof (TEvent), out source))
        return;
      (target == (Type) null ? (IEnumerable<KeyValuePair<Type, object>>) source : source.Where<KeyValuePair<Type, object>>((Func<KeyValuePair<Type, object>, bool>) (pair => pair.Key == target))).AsParallel<KeyValuePair<Type, object>>().ForAll<KeyValuePair<Type, object>>((Action<KeyValuePair<Type, object>>) (kvp =>
      {
        if (!(kvp.Value is Action<Type, TEvent> action2))
          return;
        action2(this._containerType, publishedEvent);
      }));
    }
  }
}
