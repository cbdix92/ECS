using System;
using System.Collections.Generic;

namespace CMDR
{
    internal class ComponentCollection<T> : IComponentCollection<T> where T : struct, IComponent
    {
        private Dictionary<uint, T> _components;

        private Dictionary<uint, uint> _idToIndexLookUp;

        private T[] components;

        internal ComponentCollection()
        {
            _components = new Dictionary<uint, T>();
        }

        
        #region PUBLIC_METHODS
        public void Add(T component)
        {
            throw new NotImplementedException();
        }

        public void AddRange(T[] components)
        {
            throw new NotImplementedException();
        }

        public void Remove(T component)
        {
            throw new NotImplementedException();
        }

        public void Remove(uint id)
        {
            throw new NotImplementedException();
        }

        public bool Contains(uint id)
        {
            throw new NotImplementedException();
        }

        public T Get(uint id)
        {
            return _components[id];
        }

        
    }

    internal interface IComponentCollection<T>
    {
        public void Add(T component);

        public void AddRange(T[] components);

        public void Remove(T component);

        public void Remove(uint id);

        public bool Contains(uint id);

        public T Get(uint id);
    }

    internal interface IComponentCollection : IComponentCollection<IComponent>
    {
    }

    
}