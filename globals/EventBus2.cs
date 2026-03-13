using System;
using System.Collections.Generic;
using Godot;

static class EventBus2
{
    private static Dictionary<string, List<EventHandler>> channels = new();

    public delegate void EventHandler(Node3D sender, string channel, object[] args);

    public class Subscription : IDisposable
    {
        private EventHandler handler;
        private string channel;
        public bool IsDisposed { get; private set; } = false;

        public Subscription(string channel, EventHandler handler)
        {
            this.handler = handler;
            this.channel = channel;
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            if (channels.TryGetValue(channel, out var handlers)) handlers.Remove(handler);
        }
    }

    public static Subscription Subscribe(string channel, EventHandler handler)
    {
        List<EventHandler> listeners;
        if (channels.ContainsKey(channel)) listeners = channels[channel];
        else channels[channel] = listeners = new();
        listeners.Add(handler);
        return new Subscription(channel, handler);
    }

    public static int Publish(Node3D sender, string channel, params object[] args)
    {
        if (!channels.TryGetValue(channel, out var handlers)) return 0;
        handlers.ForEach(handler => handler(sender, channel, args));
        return handlers.Count;
    }
}