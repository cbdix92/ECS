using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CMDR;

namespace Test
{
    class Test
    {
        static Scene Scene = new Scene();
        
        static GameObjectBuilder gameObjectBuilder = new GameObjectBuilder();
        
        static Transform transform = new Transform();

        static Random r = new Random();

        static int size = 1_000;

        static List<ID> ids = new List<ID>(size);

        static List<float> initialPos = new List<float>(size);

        static Stopwatch stopwatch = new Stopwatch();

        static long ns = 1_000_000_000 / Stopwatch.Frequency;
        static void Main(string[] args)
        {
            Console.WriteLine(Marshal.SizeOf<Transform>());
            stopwatch.Start();

            Query query = Scene.RegisterQuery<Transform>(MyFilter);

            for (int i = 0; i < size; i++)
            {
                initialPos.Add(r.Next(0, 500));
                transform.Teleport(new Vector3(1));
                gameObjectBuilder.Bind(transform);
                ids.Add(Scene.Populate(gameObjectBuilder));
            }

            stopwatch.Stop();

            Console.WriteLine(stopwatch.ElapsedTicks / ns);

            Span<Transform> transforms;

            ID id = ids[0];
            //Scene.DestroyGameObject(ref id);

            float count = 0;
            for( int j = 0; j < 10; j++)
            {
                stopwatch.Reset();
                stopwatch.Start();

                while(Scene.GetQuery(query, out transforms))
                {
                    for(int i = 0; i < transforms.Length; i++)
                    {
                        count += transforms[i].Position.X;
                    }
                }

                stopwatch.Stop();

                Console.WriteLine(stopwatch.ElapsedTicks / ns);
            }

            Console.WriteLine(count);

            Console.ReadKey();
        }

        private static bool MyFilter(GameObject gameObject)
        {
            if (gameObject.ContainsComponent<Transform>())
                return true;
            return false;
        }
    }
}
