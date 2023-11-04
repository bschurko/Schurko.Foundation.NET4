
using System;


#nullable enable
namespace Schurko.Foundation.Messaging.EventAggregator
{
  public interface IEventAggregator
  {
    void Subscribe<TEvent>(Action<Type, TEvent> action);

    void Unsubscribe<TEvent>(Action<Type, TEvent> action = null);

    void Publish<TEvent>();

    void Publish<TEvent>(TEvent publishedEvent);

    void Publish<TEvent>(TEvent publishedEvent, Type target);
  }
}
