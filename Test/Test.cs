using System;
using System.Collections.Generic;
using CMDR;

namespace Test
{
    class Test
    {
        static Scene Scene = new Scene();
        
        static GameObjectBuilder gameObjectBuilder = new GameObjectBuilder();
        
        static Transform transform = new Transform();

        static Random r = new Random();

        static int size = 3000;

        static List<ID> ids = new List<ID>(size);

        static List<float> initialPos = new List<float>(size);
        
        static void Main(string[] args)
        {
            Transform tOut;

            for (int i = 0; i < size; i++)
            {
                initialPos.Add(r.Next(0, 500));
                transform.Teleport(new Vector3(initialPos[i]));
                gameObjectBuilder.Bind(transform);
                ids.Add(Scene.Populate(gameObjectBuilder));
            }

            for (int i = 0; i < size; i++)
            {
                Scene.GetComponent(ids[i], out tOut);
                Console.Write($"{initialPos[i]} - ");
                Console.WriteLine(tOut.Position.X);
            }

            Console.ReadKey();
        }
    }
}
