using System;
using CMDR;

namespace Test
{
    class Test
    {
        static Scene Scene = new Scene();
        
        static GameObjectBuilder gameObjectBuilder = new GameObjectBuilder();
        
        static Transform transform = new Transform();

        static Random r = new Random();
        
        static void Main(string[] args)
        {
            ID id;
            Transform tOut;
            for (int i = 0; i < 200; i++)
            {
                transform.Teleport(new Vector3(r.Next(0, 300)));
                gameObjectBuilder.Bind(transform);
                id = Scene.Populate(gameObjectBuilder);
                Scene.GetComponent(id, out tOut);
                Console.Write($"{transform.Position.X} - ");
                Console.WriteLine(tOut.Position.X);

            }

            transform.Teleport(new Vector3(5));
            Console.WriteLine($"Main {transform.Position.X}");

            gameObjectBuilder.Bind(transform);

            gameObjectBuilder.GetComponents(out Type[] types, out IComponent[] components);
            Transform j = (Transform)components[0];
            Console.WriteLine($"Builder {j.Position.X}");

            id = Scene.Populate(gameObjectBuilder);

            Scene.GetComponent(id, out Transform t);
            Console.WriteLine($"DataSYS {t.Position.X}");
        }
    }
}
