using System;
using CMDR;

namespace Test
{
    class Test
    {
        static Scene Scene = new Scene();

        static GameObjectBuilder gameObject = new GameObjectBuilder();

        static Transform transform = new Transform();
        static void Main(string[] args)
        {
            gameObject.Bind<Transform>(transform);

            Scene.Populate(gameObject);


            Console.ReadKey();
        }
    }
}
