using System;

namespace Avalonia.Interactivity
{
    public delegate void RoutedEventHandler(object sender, RoutedEventArgs e);

    public static class EventManager
    {
        private class CompatRoutedEvent : RoutedEvent
        {
            private readonly Type _handlerType;
            public CompatRoutedEvent(
                string name,
                RoutingStrategies routingStrategies,
                Type handlerType,
                Type ownerType)
                : base(name, routingStrategies, typeof(RoutedEventArgs), ownerType)
            {
                Contract.Requires<ArgumentNullException>(handlerType != null);

                _handlerType = handlerType;
            }

            protected override void VerifyHandler(Delegate handler)
            {
                var handlerType = handler.GetType();
                if (handlerType != _handlerType && handlerType != typeof(RoutedEventHandler))
                {
                    throw new ArgumentException(
                        $"Illegal handler type: {handlerType.FullName} (expected {_handlerType.FullName})");
                }
            }
        }

        public static RoutedEvent RegisterRoutedEvent(
            string name,
            RoutingStrategies routingStrategy,
            Type handlerType,
            Type ownerType)
        {
            var routedEvent = new CompatRoutedEvent(name, routingStrategy, handlerType, ownerType);

            RoutedEventRegistry.Instance.Register(ownerType, routedEvent);
            return routedEvent;
        }
    }
}
