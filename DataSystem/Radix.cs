using System;
using System.Buffers;

namespace CMDR
{
    public static class Radix
    {

        private static ArrayPool<ulong> _countPool = ArrayPool<ulong>.Create();

        private static ulong[] count;

        #region PUBLIC_METHODS
        
        /// <summary>
        /// Sort an array using Radix sort.
        /// </summary>
        /// <param name="max"> The maximum expected value that is contained within the input array.
        /// The max value can be obtained from <see href="Data._currentGameObjectID"> or <see href="Data._currentComponentID">. </param>
        /// <param name="input"> The input array that will be sorted. </param>
        public static ulong[] B2Sort(uint max, ulong[] input)
        {
            return B2InternalSort(max, 0, 1L, input, new ulong[input.Length]);
        }

        public static ulong[] B10Sort(uint max, ulong[] input)
        {
            return B10InternalSort(max, 0, input, new ulong[input.Length]);
        }

        public static ulong[] B16Sort(uint max, ulong[] input)
        {
            return B16InternalSort(max, 0, 0xf, input, new ulong[input.Length]);
        }

        #endregion

        #region PRIVATE_METHODS

        private static ulong[] B2InternalSort(uint max, int pos, ulong mask, ulong[] input, ulong[] output)
        {
            uint[] count = new uint[2];

            for(uint i = 0; i < input.Length; i++)
            {
                count[(input[i] & mask) >> pos]++;
            }

            // PREFIX SUM
            count[1] += count[0];

            for(int i = input.Length - 1; i > -1; i--)
            {
                output[--count[(input[i] & mask) >> pos]] = input[i];
            }

            if(pos == max)
                return output;

            return B2InternalSort(max, ++pos, mask << 1, output, input);
        }

        private static ulong[] B10InternalSort(uint max, uint pos, ulong[] input, ulong[] output)
        {
            uint[] count = new uint[10];

            for(uint i = 0; i < count.Length; i++)
            {
                count[input[i] % (10 * pos)]++;
            }

            for(uint i = 0; i < 9; i++)
            {
                count[i + 1] += count[i];
            }

            for(int i = input.Length - 1; i > -1; i--)
            {
                output[input[count[input[i] % (10 * pos)]--]] = input[i];
            }

            if(pos == max)
                return output;

            return B10InternalSort(max, ++pos, output, input);

        }

        private static ulong[] B16InternalSort(uint max, int pos, ulong mask, ulong[] input, ulong[] output)
        {
            count = _countPool.Rent(16);        
            //count = new uint[16];

            // Count
            for(uint i = 0; i < input.Length; i++)
            {
                count[(input[i] & mask) >> pos]++;
            }

            // Prefix Sum
            for(uint i = 0; i < 15; i++)
            {
                count[i + 1] += count[i];
            }

            // Build Output
            for(int i = input.Length - 1; i > -1; i--)
            {
                output[--count[(input[i] & mask) >> pos]] = input[i];
            }

            // Exit Conditions
            if (pos >= max)
                return output;

            // Recursion
            return B16InternalSort(max, pos + 4, mask << 4, output, input);
        }

#endregion
    }
}