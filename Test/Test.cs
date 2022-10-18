using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CMDR;

namespace Test
{
    class Test
    {
        static Random r = new Random();
        
        static List<ID> ids = new List<ID>();

        static Stopwatch stopwatch = new Stopwatch();
        
        static Scene Scene = new Scene();
        
        static GameObjectBuilder gameObjectBuilder = new GameObjectBuilder();

        static GameObjectBuilder gameObjectBuilder2 = new GameObjectBuilder();
        
        static Transform transform = new Transform();

        static Collider collider = new Collider();

        static long ns = 1_000_000_000 / Stopwatch.Frequency;
        static void Main(string[] args)
        {
            Query query = Scene.RegisterQuery<Transform>(MyFilter);

            stopwatch.Start();

            for (int i = 0; i < 10; i++)
            {
                transform.Teleport(new Vector3(r.Next(0, 100)));
                gameObjectBuilder.Bind(transform);

                //if (i == 1)
                //{
                //    gameObjectBuilder.Bind(collider);
                //    ids.Add(Scene.Populate(gameObjectBuilder));
                //    gameObjectBuilder.UnbindAll();
                //}

                ids.Add(Scene.Populate(gameObjectBuilder));
            }

            stopwatch.Stop();

            Console.WriteLine(stopwatch.ElapsedTicks * ns);

            Span<Transform> transformQuery;

            ID id = ids[1];
            ID id2 = ids[2];

            Console.WriteLine(id);
            Console.WriteLine(id2);

            Scene.DestroyGameObject(ref id);
            Scene.DestroyGameObject(ref id2);

            stopwatch.Reset();

            stopwatch.Start();

            while(Scene.GetQuery(query, out transformQuery))
            {
                for(int i = 0; i < transformQuery.Length; i++)
                {
                    Console.WriteLine(transformQuery[i].ID);
                    //Scene.GetGameObject(transformQuery[i].ID, out GameObject gameObject);
                    //Console.WriteLine($"{loopCounter++} {gameObject.ContainsComponent<Collider>()}");
                }
            }

            stopwatch.Stop();
            var time = stopwatch.ElapsedTicks * ns;
            var perSecond = 1000000000 / time;
            Console.WriteLine($"System Loop Time ns:{stopwatch.ElapsedTicks * ns}");
            Console.WriteLine($"perSecond: {perSecond}");

            Console.ReadKey();
        }

        private static bool MyFilter(GameObject gameObject)
        {
            return gameObject.ContainsComponent<Transform>() && gameObject.ContainsComponent<Collider>() == false;
        }
    }
}
