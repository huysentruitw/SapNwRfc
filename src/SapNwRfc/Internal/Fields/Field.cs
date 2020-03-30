using System;
using System.Collections.Generic;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Fields
{
    internal abstract class Field<TValue> : IField
    {
        protected Field(string name, TValue value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public TValue Value { get; }

        public abstract void Apply(RfcInterop interop, IntPtr dataHandle);

        public override bool Equals(object obj)
        {
            return obj is Field<TValue> @base &&
                   Name == @base.Name &&
                   EqualityComparer<TValue>.Default.Equals(Value, @base.Value);
        }

        public override int GetHashCode()
        {
            int hashCode = -244751520;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<TValue>.Default.GetHashCode(Value);
            return hashCode;
        }
    }

    internal interface IField
    {
        void Apply(RfcInterop interop, IntPtr dataHandle);
    }
}
