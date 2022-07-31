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
            gameObjectBuilder.Bind(transform);

            Scene.Populate(gameObjectBuilder);
        }

        static Transform convert(IComponent component)
        {
            return (Transform)component;
        }
    }
}
