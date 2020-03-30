using System;

namespace SapNwRfc
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SapNameAttribute : Attribute
    {
        public SapNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
