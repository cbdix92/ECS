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
        public uint NewGameObjectID
        {
            get
            {
                if(_availableGameObjectIDs.TryDequeue(out uint result))
                {
                    return result;
                }

                try
                {
                    return ++_currentGameObjectID;
                }
                catch(OverflowException)
                {
                    throw new OverflowException("Max number of 4,294,967,295 GameObjects has been reached. . Quite impressive.");
                }
            }
        }

        #endregion

        public IDProvider()
        {
            _availableGameObjectIDs = new Queue<uint>();

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
        /// Bit 33 - 63 (Reserved)
        /// Bit 1 - 32 (GameObject ID)
        /// </summary>
        /// <param name="gameObject"> GameObject to be give a new ID. </param>
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