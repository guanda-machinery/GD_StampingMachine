using System;

namespace GD_CommonLibrary
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public readonly T? OldValue;
        public readonly T? NewValue;

        public ValueChangedEventArgs(T? oldValue, T? newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

    }
}
