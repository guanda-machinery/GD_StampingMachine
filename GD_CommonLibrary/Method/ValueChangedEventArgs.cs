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
    /*public class ValueEventArgs<T> : EventArgs
    {
        public T Message { get; }

        public ValueEventArgs(T message)
        {
            Message = message;
        }
    }*/



}
