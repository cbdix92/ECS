using System;
using System.Text;

namespace CMDR
{

    public struct ID
    {
        
        #region PUBLIC_MEMBERS

        public uint BaseID { get => (uint)(_id & _idMask); }

        public short BatchID { get => (short)((_batchMask & _id) >> 24); }

        public ulong Id { get => (_id & _idWithBatchMask); }

        public uint MetaData { get => (uint)(_id >> 40); }

        public uint this[uint mask]
        {
            get => (mask & (uint)(_id >> 40));
            set
            {
                _id |= ((ulong)value << 40);
            }
        }

        #endregion

        #region PRIVATE_MEMBERS

        private ulong _id;

        private readonly static uint _idMask = 0xffffff;

        private readonly static ulong _batchMask = 0xffff000000;

        private readonly static ulong _idWithBatchMask = _idMask | _batchMask;

        private readonly static ulong _metaDataMask = 0xffffff0000000000;

        #endregion

        #region CONSTRUCTOR

        internal ID(ulong id) => _id = id;

        #endregion

        #region PUBLIC_METHODS

        public static ulong operator |(ID id, uint n) => n | id.Id;

        public static ulong operator &(ID id,  uint n) => n & id.Id; 

        public static ulong operator ^(ID id, uint n) => n ^ id.Id;

        public static ulong operator ~(ID id) => ~id.Id;

        public static ulong operator >>(ID id, int n) => id.Id >> n;

        public static bool operator ==(ID id, ID id2) => id.Id == id2.Id;

        public static bool operator ==(ID id, uint n) => id.Id == n;

        public static bool operator ==(ID id, ulong n) => id.Id == (n & _idMask);

        public static bool operator !=(ID id, ID id2) => id.Id != id2.Id;

        public static bool operator !=(ID id, uint n) => id.Id != n;

        public static bool operator !=(ID id, ulong n) => id.Id != (n & _idMask);

        // object.GetHashCode needs to overridden so that Dictionary will only consider the ID part of _id;
        public override int GetHashCode() => Id.GetHashCode();
        
        // object.Equals is overriden so that Dictionary will only consider the ID part of _id.
        public override bool Equals(object obj)
        {
            if (obj is ID id)
            {
                if (Id == id.Id)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a string of binary numbers to represent this ID. Meta data, Batch ID,
        /// and Base ID will be seperated by an underscore.
        /// </summary>
        /// <returns> A string that represents this ID. </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(18 + (sizeof(ulong) * 8));

            sb.Append(Convert.ToString(MetaData, 2));

            sb.Append("_");

            sb.Append(Convert.ToString(BatchID, 2));

            sb.Append("_");

            sb.Append(Convert.ToString(BaseID, 2));

            return sb.ToString();
        }

        #endregion

        #region INTERNAL_METHODS

        /// <summary>
        /// Inlay a new base ID and Batch ID. Used to initialize a components ID without disturbing Meta Data.
        /// </summary>
        /// <param name="id"> A base ID with Batch ID. </param>
        internal void InlayID(ulong id)
        {
            _id |= id;
        }

        internal void SetZero()
        {
            _id = 0;
        }

        #endregion
    }

}