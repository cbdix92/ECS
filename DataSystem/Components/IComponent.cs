using System;

namespace CMDR
{
    public interface IComponent
    {
        ulong ID { get; internal set; }
    }

    public interface IComponent<T> : IComponent { }
}
