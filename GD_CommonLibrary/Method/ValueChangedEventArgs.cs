using System;

namespace GD_CommonLibrary
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T OldValue { get; }
        public T NewValue { get; }

        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

    }
}
