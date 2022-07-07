using System;
using System.Collections.Generic;

namespace CMDR
{
    internal class ComponentCollection<T> : IComponentCollection<T> where T : struct, IComponent
    {
        private Dictionary<uint, T> _components;


        T IComponentCollection<T>.Get(uint id)
        {
            return _components[id];
        }

        internal ComponentCollection()
        {
            _components = new Dictionary<uint, T>();
        }
    }

    internal interface IComponentCollection<T>
    {
        internal T Get(uint id);
    }

    internal interface IComponentCollection : IComponentCollection<IComponent>
    {
    }

    
}