using System;

namespace CMDR
{
    public interface IComponent
    {
        ID ID { get; internal set; }
    }

    public interface IComponent<T> : IComponent { }
}
