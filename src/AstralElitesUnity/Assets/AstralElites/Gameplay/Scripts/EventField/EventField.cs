using System;
using System.Collections.Generic;

[Serializable]
public class EventField<T> : IDisposable
{
    public struct HandlerCollection : IDisposable
    {
        public struct ContextWrapped
        {
            public IEventFieldHandler Result;

            private readonly EventField<T> Field;
            private readonly object Context;

            public ContextWrapped(EventField<T> field, object context)
            {
                Field = field;
                Context = context;
                Result = null;
            }

            public void Clear()
            {
                Field.Handlers.Clear(Context);
            }

            public static ContextWrapped operator +(ContextWrapped left, IEventFieldHandler right)
            {
                left.Result = right;
                return left;
            }
        }
        private readonly EventField<T> field;
        private List<KeyValuePair<object, IEventFieldHandler>> handlers;

        public HandlerCollection(EventField<T> field)
        {
            this.field = field;
            handlers = null;
        }

        public ContextWrapped this[object context]
        {
            readonly get => new(field, context);
            set
            {
                handlers ??= new List<KeyValuePair<object, IEventFieldHandler>>();

                handlers.Add(new KeyValuePair<object, IEventFieldHandler>(context, value.Result));
            }
        }

        public void Clear(object context)
        {
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                if (handlers[i].Key == context)
                {
                    handlers.RemoveAt(i);
                }
            }
        }

        public void Clear()
        {
            handlers.Clear();
        }

        public void InvokeBeforeChanged()
        {
            if (handlers == null)
            {
                return;
            }

            for (int i = 0; i < handlers.Count; i++)
            {
                handlers[i].Value.OnBeforeChanged();
            }
        }

        public void InvokeAfterChanged()
        {
            if (handlers == null)
            {
                return;
            }

            for (int i = 0; i < handlers.Count; i++)
            {
                handlers[i].Value.OnAfterChanged();
            }
        }

        public void Dispose()
        {
            if (handlers == null)
            {
                return;
            }

            for (int i = 0; i < handlers.Count; i++)
            {
                handlers[i].Value.Dispose();
            }
        }
    }

    public HandlerCollection Handlers;
    public Action OnBeforeChanged;
    public Action OnAfterChanged;

    private T internalValue;

    public T Value
    {
        get => internalValue;
        set
        {
            Handlers.InvokeBeforeChanged();
            OnBeforeChanged?.Invoke();

            internalValue = value;

            Handlers.InvokeAfterChanged();
            OnAfterChanged?.Invoke();
        }
    }

    public EventField()
    {
        Handlers = new HandlerCollection(this);
    }

    public EventField<TChild> Watch<TChild>(Func<T, EventField<TChild>> chain)
    {
        var watcher = new EventField<TChild>();
        Handlers[watcher] += new EventFieldChainHandler<T, TChild>(this, watcher, chain);
        return watcher;
    }

    public void Dispose()
    {
        Handlers.Dispose();
    }
}
