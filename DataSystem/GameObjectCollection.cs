using System;
using System.Collections;
using System.Collections.Generic;

namespace CMDR
{
    internal class GameObjectCollection
    {
        #region PUBLIC_MEMBERS

        public uint Count { get; private set; }

        public GameObject this[int index]
        {
            get
            {

                if(index >= Count || index < 0)
                    throw new IndexOutOfRangeException($"{index} is out of range for GameObjectCollection");
                
                return _data[index];

            }
        }

        #endregion

        #region PRIVATE_MEMBERS

        private GameObject[] _data;

        #endregion

        #region CONSTRUCTOR

        internal GameObjectCollection()
        {
            _data = new GameObject[Data.StorageScale];
        }

        #endregion

        #region PUBLIC_METHODS


        public virtual void Add(GameObject gameObject)
        {

            if(Count == _data.Length)
                Array.Resize(ref _data, _data.Length + Data.StorageScale);
            
            _data[Count] = gameObject;

            Count++;

        }

        public GameObject BinarySearch(uint id)
        {
            return BinarySearch(id, 0, Count - 1);
        }

        public GameObject BinarySearch(uint id, int low, int high)
        {
            if(high >= Count)
                high = Count - 1;
            
            if(low < 0)
                low = 0;

            return BinarySearchInternal(id, low, high);
        }

        public virtual void Remove(uint id)
        {
            // TODO ...
            // Remove all GameObjects components

            // TODO ... 
            // Tell DataSystem to remove GameObject from Query

            if(id == --Count)
            {
                _data[id] = GameObject.Default;
                
                return;
            }

            // Place GameObject at the end of the array in the deleted GameObjects place.
            _data[id] = _data[Count];

            GameObjectMoved(ref _data[id], id);

        }

        public virtual GameObject[] ToArray()
        {
            
            ArraySegment<GameObject> _ = new ArraySegment<GameObject>(_data, 0, Count);
            
            return _.ToArray();

        }

        #endregion

        #region PRIVATE_METHODS

        private void GameObjectMoved(ref GameObject gameObject, uint newID)
        {
            gameObject.ID = newID;
            
            // TODO ... 
            // Tell Components of GameObject the new Parent ID
            
        }

        private GameObject BinarySearchInternal(uint id, int low, int high)
        {
            int mid = high / 2;

            if(_data[mid].ID == id)
            {
                return _data[mid];
            }
            else if(id < mid)
            {
                return BinarySearchInternal(id, low, mid - 1);
            }
            else
            {
                return BinarySearchInternal(id, mid + 1, high);
            }
        }

        #endregion
    }
}