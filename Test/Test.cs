using System;
using CMDR;

namespace Test
{
    class Test
    {
        static Scene Scene = new Scene();
        
        static GameObjectBuilder gameObjectBuilder = new GameObjectBuilder();
        
        static Transform transform = new Transform();
        
        static void Main(string[] args)
        {
            transform.Teleport(new Vector3(5));
            Console.WriteLine($"Main {transform.Position.X}");

            gameObjectBuilder.Bind(transform);

            gameObjectBuilder.GetComponents(out Type[] types, out IComponent[] components);
            Transform j = (Transform)components[0];
            Console.WriteLine($"Builder {j.Position.X}");

            ID id = Scene.Populate(gameObjectBuilder);

            Scene.GetComponent(id, out Transform t);
            Console.WriteLine($"DataSYS {t.Position.X}");
        }
    }
}
