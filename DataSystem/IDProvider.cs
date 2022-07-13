using System;
using System.Collections.Generic;

namespace CMDR.DataSystem
{
    internal sealed class IDProvider
    {
        #region PUBLIC_MEMBERS

        /// <summary>
        /// Provides an unused GameObject ID.
        /// </summary>
        public ulong NewGameObjectID
        {
            get
            {

                if(_availableGameObjectIDs.TryDequeue(out uint result))
                {
                    ulong batch = (result & _batchMask) >> 24;
                    
                    batch++;

                    return (batch << 24) | result;
                }

                try
                {
                    return ++_currentGameObjectID;
                }
                catch(OverflowException)
                {
                    throw new OverflowException("RIP MEMORY!");
                }
            }
        }

        #endregion

        public IDProvider()
        {
            _availableGameObjectIDs = new Queue<ulong>();

            _currentGameObjectID = 0;
        }

        #region PRIVATE_MEMBERS

        private Queue<uint> _availableGameObjectIDs;

        private uint _currentGameObjectID;

        #endregion

        #region PUBLIC_METHODS

        /// <summary>
        /// Generate a new unused GameObject ID for the provided GameObject. 
        /// Bit 64 (Alive/Dead)
        /// Bit 41 - 63 (Reserved)
        /// Bit 25 - 40 (BatchID)
        /// Bit 1 - 24 (GameObject ID)
        /// </summary>
        /// <returns>  </returns>
        public ID GenerateID()
        {
            return new ID(NewGameObjectID);
        }

        /// <summary>
        /// Finds the most significant bit of generated IDs.
        /// </summary>
        /// <returns> Returns the most significant bit position for generated IDs. </returns>
        public int GetMaxIDBitPosition()
        {
            int max = 1;

            while(max < _currentGameObjectID)
            {
                max <<= 4;
            }

            return max;
        }

        #endregion
    }
}