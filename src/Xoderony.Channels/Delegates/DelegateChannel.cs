using System;

namespace Xoderony;

public sealed class DelegateChannel<TDelegate> : IDelegateSubscriber<TDelegate>, IDelegateDispatcher<TDelegate> where TDelegate : Delegate {

    private TDelegate? _handlers;

    public TDelegate? Handlers => _handlers;

    public void Subscribe(TDelegate handler) {
        _handlers = (TDelegate?)Delegate.Combine(_handlers, handler);
    }

    public void Unsubscribe(TDelegate handler) {
        _handlers = (TDelegate?)Delegate.Remove(_handlers, handler);
    }
}
