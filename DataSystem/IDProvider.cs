

namespace CMDR.DataSystem
{
    internal sealed class IDProvider
    {
        #region PUBLIC_MEMBERS

        /// <summary>
        /// Provides an unused GameObject ID.
        /// </summary>
        internal static uint NewGameObjectID
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
        }

        #region PRIVATE_MEMBERS

        private uint _currentGameObjectID;

        private static Queue<uint> _availableGameObjectIDs;

        #endregion

        #region PUBLIC_METHODS

        internal static int GetMaxIDRange()
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