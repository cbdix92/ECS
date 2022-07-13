using System;
using System.Text;

namespace CMDR
{

    public struct ID
    {
        
        #region PUBLIC_MEMBERS

        public uint Id{ get => (uint)_id; }

        public uint MetaData { get => (uint)(_id >> 32); }

        public uint this[uint mask]
        {
            get => (mask & (uint)(_id >> 32));
            set
            {
                _id |= ((ulong)value << 32);
            }
        }

        #endregion

        #region PRIVATE_MEMBERS

        private ulong _id;

        private static uint _idMask = 0xffffffff;

        #endregion

        #region CONSTRUCTOR

        internal ID(ulong id) => _id = id;

        #endregion

        #region PUBLIC_METHODS

        public static uint operator |(ID id, uint n) => n | id.Id;

        public static uint operator &(ID id,  uint n) => n & id.Id; 

        public static uint operator ^(ID id, uint n) => n ^ id.Id;

        public static uint operator ~(ID id) => ~id.Id;

        public static uint operator >>(ID id, int n) => id.Id >> n;

        public static bool operator ==(ID id, ID id2) => id2.Id == id.Id;

        public static bool operator ==(ID id, uint n) => id.Id == n;

        public static bool operator ==(ID id, ulong n) => id.Id == (n & _idMask);

        public static bool operator !=(ID id, ID id2) => id.Id != id2.Id;

        public static bool operator !=(ID id, uint n) => id.Id != n;

        public static bool operator !=(ID id, ulong n) => id.Id != (n & _idMask);

        // object.GetHashCode needs to overridden so that Dictionary will only consider the ID part of _id;
        public override int GetHashCode() => Id.GetHashCode();
        
        // object.Equals is overriden so that Dictionary will only consider the ID part of _id.
        public override bool Equals(object obj) => Id.Equals(obj);

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(1 + (sizeof(ulong) * 8));

            sb.Append(Convert.ToString(MetaData, 2));

            sb.Append("_");

            sb.Append(Convert.ToString(Id, 2));

            return sb.ToString();
        }

        #endregion
    }

}