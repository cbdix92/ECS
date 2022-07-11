

namespace CMDR
{

    public struct ID
    {
        
        #region PUBLIC_MEMBERS

        public uint ID{ get => _id & _idMask; }

        public uint MetaData { get => _id & _metaDataMask; }

        public uint this[int index]
        {
            get => _id & (index << 32);
            set
            {
                _id |= ((ulong)value << 32)
            }
        }

        #endregion

        #region PRIVATE_MEMBERS

        private ulong _id;

        private uint _idMask = 0xffffffff;

        private ulong _metaDataMask = 0xffffffff00000000;

        #endregion

        #region CONSTRUCTOR

        internal ID(ulong id) => _id = id;

        #endregion

        #region PUBLIC_METHODS

        public uint operator |(uint n) => n | MetaData;

        public uint operator &(uint n) => n & MetaData; 

        public uint operator ^(uint n) => n ^ MetaData;

        public uint operator ~() => ~MetaData;

        public uint operator >>(uint n) => MetaData >> n;

        public uint operator <<(uint n) => MetaData << n;

        public bool operator ==(ID id) => ID == id.ID;

        public bool operator ==(uint id) => ID == id;

        public bool operator ==(ulong id) => ID == (uint)(id & _idMask);

        public bool operator !=(ID id) => ID != id.ID;

        public bool operator !=(uint id) =>  ID != id;

        public bool operator !=(ulong id) => ID != (uint)(id & _idMask);

        // object.GetHashCode needs to overridden so that Dictionary will only consider the ID part of _id;
        public override int GetHashCode() => ID.GetHashCode();
        
        // object.Equals is overriden so that Dictionary will only consider the ID part of _id.
        public override bool Equals(object obj) => ID.Equals(obj);

        public override string ToString()
        {
            StringBuilder sb = StringBuilderCache.Acquire();

            //StringBuilder sb = new StringBuilder(++(sizeof(_id) * 8));

            sb.Append(Convert.ToString(MetaData, 2);
            sb.Append("_");
            sb.Append(Convert.ToString(ID, 2));
            
            //return sb.ToString();

            return StringBuilderCache.GetStringAndRelease(sb);
        }

        #endregion
    }

}