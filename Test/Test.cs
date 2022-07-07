using System;
using CMDR;

namespace Test
{
    class Test
    {
        static void Main(string[] args)
        {
            Random R = new Random();
            ulong[] GameObjects = new ulong[50];
            for (int i = 0; i < GameObjects.Length; i++)
            {
                GameObjects[i] = (ulong)R.Next(int.MaxValue);
                Console.WriteLine(GameObjects[i]);
            }

            ulong[] output = Radix.Sort(sizeof(uint) * 8, GameObjects);

            for(int i = 0; i < output.Length; i++)
            {
                Console.WriteLine(output[i]);
            }

            Console.ReadKey();


        }
    }
}
