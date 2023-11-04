using System;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class EventExtensions
    {
        public static bool Raise(this EventHandler @event, object sender, EventArgs args)
        {
            if (@event == null)
                return false;
            @event(sender, args);
            return true;
        }

        public static bool Raise<T>(this EventHandler<T> @event, object sender, T args) where T : EventArgs
        {
            if (@event == null)
                return false;
            @event(sender, args);
            return true;
        }
    }
}
