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

        static GameObjectBuilder gameObjectBuilder2 = new GameObjectBuilder();
        
        static Transform transform = new Transform();

        static Random r = new Random();

        static int size = 1_000;

        static List<ID> ids = new List<ID>(size + 1);

        static List<float> initialPos = new List<float>(size + 1);

        static Stopwatch stopwatch = new Stopwatch();

        static long ns = 1_000_000_000 / Stopwatch.Frequency;
        static void Main(string[] args)
        {
            stopwatch.Start();

            Query query = Scene.RegisterQuery<Transform>(MyFilter);

            for (int i = 0; i < size; i++)
            {
                initialPos.Add(r.Next(0, 500));

                if (i % 2 == 0)
                {
                    gameObjectBuilder2.Bind(transform);
                    gameObjectBuilder2.Bind(new Collider());
                    ids.Add(Scene.Populate(gameObjectBuilder2));
                    continue;
                }

                transform.Teleport(new Vector3(initialPos[i]));
                gameObjectBuilder.Bind(transform);
                ids.Add(Scene.Populate(gameObjectBuilder));
            }

            stopwatch.Stop();

            Console.WriteLine(stopwatch.ElapsedTicks / ns);

            Span<Transform> transforms;

            ID id = ids[size / 2];
            Scene.DestroyGameObject(ref id);

            float count = 0;
            for( int j = 0; j < 1; j++)
            {
                stopwatch.Reset();
                stopwatch.Start();

                while(Scene.GetQuery(query, out transforms))
                {
                    for(int i = 0; i < transforms.Length; i++)
                    {
                        Scene.GetGameObject(transforms[i].ID, out GameObject gameObject);
                        Console.WriteLine(gameObject.ContainsComponent<Collider>());
                        count += transforms[i].Position.X;
                    }
                }

                stopwatch.Stop();

                Console.WriteLine($"System Loop Time ns:{stopwatch.ElapsedTicks / ns}");
            }

            Console.ReadKey();
        }

        private static bool MyFilter(GameObject gameObject)
        {
            if (gameObject.ContainsComponent<Transform>() && gameObject.ContainsComponent<Collider>() == false)
                return true;
            return false;
        }
    }
}
