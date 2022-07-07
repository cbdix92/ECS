using System;
using System.Collections.Generic;

namespace CMDR
{
    public static class SceneManager
    {
        #region PUBLIC_MEMBERS

        public static Scene ActiveScene { get; private set; }

        #endregion

        #region PRIVATE_MEMBERS

        private static List<Scene> _loadedScenes = new List<Scene>();

        #endregion

        #region PUBLIC_METHODS
        
        public static uint LoadScene(Scene scene)
        {
            if(ActiveScene == null)
                ActiveScene = scene;

            _loadedScenes.Add(scene);
            return 0;
        }

        public static uint LoadScene(string path)
        {
            return 0;
        }

        public static void SaveScene() => SaveScene(ActiveScene);

        public static void SaveScene(Scene scene)
        {

        }

        #endregion
    }
}