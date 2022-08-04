using System;
using System.Collections.Generic;
using System.Diagnostics;
using CMDR;

namespace Test
{
    class Test
    {
        static Scene Scene = new Scene();
        
        static GameObjectBuilder gameObjectBuilder = new GameObjectBuilder();
        
        static Transform transform = new Transform();

        static Random r = new Random();

        static int size = 10000;

        static List<ID> ids = new List<ID>(size);

        static List<float> initialPos = new List<float>(size);

        static Stopwatch stopwatch = new Stopwatch();

        static long ns = 1000000000 / Stopwatch.Frequency;
        static void Main(string[] args)
        {
            stopwatch.Start();

            Query query = Scene.RegisterQuery<Transform>(MyFilter);

            for (int i = 0; i < size; i++)
            {
                initialPos.Add(r.Next(0, 500));
                transform.Teleport(new Vector3(initialPos[i]));
                gameObjectBuilder.Bind(transform);
                ids.Add(Scene.Populate(gameObjectBuilder));
            }

            stopwatch.Stop();

            Console.WriteLine(stopwatch.ElapsedTicks / ns);

            stopwatch.Reset();

            Span<Transform> transforms;

            stopwatch.Start();
            ID id = ids[0];
            Scene.DestroyGameObject(ref id);

            while(Scene.GetQuery(query, out transforms))
            {
                for(int i = 0; i < transforms.Length; i++)
                {
                    Transform t = transforms[i];
                }
            }

            stopwatch.Stop();

            Console.WriteLine(stopwatch.ElapsedTicks / ns);

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
