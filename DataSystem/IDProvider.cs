

namespace CMDR.DataSystem
{
    internal sealed class IDProvider
    {
        #region PUBLIC_MEMBERS

        /// <summary>
        /// Provides an unused GameObject ID.
        /// </summary>
        public static uint NewGameObjectID
        {
            get
            {
                if(_availableGameObjectIDs.Any())
                {
                    return _availableGameObjectIDs.Dequeue();
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

        private static Queue<uint> _availableGameObjectIDs;

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
        public static void GenerateGameObjectID(ref GameObject gameObject)
        {
            gameObject.ID = 0;
            gameObject.ID |= 0x8000000000000000 | NewGameObjectID;
        }

        public static int GetMaxIDRange()
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