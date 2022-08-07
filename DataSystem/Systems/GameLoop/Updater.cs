using System.Diagnostics;

namespace CMDR.Systems
{
    internal class Updater
    {
        #region PUBLIC_MEMBERS
        
        public int PerSecond
        {
            get { return _perSecond; }

            set
            {
                _perSecond = value;
                
                _target = Stopwatch.Frequency / value;
            }
        }
        
        public event UpdateHandler Handler;

        #endregion

        #region PRIVATE_MEMBERS

        private long _lastUpdate;
        
        private long _target;
        
        private int _perSecond;

        #endregion

        #region PUBLIC_METHODS

        public void Update(long ticks)
        {
            if (_lastUpdate + _target <= ticks && Handler != null)
            {
                Handler(ticks);

                _lastUpdate = ticks;
            }
        }

        #endregion

    }
}
