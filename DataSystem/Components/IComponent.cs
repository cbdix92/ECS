using System;

namespace CMDR
{
    public interface IComponent
    {
        ID ID { get; set; }
    }

    public interface IComponent<T> : IComponent { }
}
