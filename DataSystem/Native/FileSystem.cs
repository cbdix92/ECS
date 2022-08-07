using System;
using System.IO;
using System.Reflection;

namespace CMDR.Native
{
    internal static class FileSystem
    {

        #region PUBLIC_MEMBERS

        public static string SavePath => _savePath;

        #endregion

        #region PRIVATE_MEMBERS

        private static readonly string _dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        
        private static readonly string _tempDir = @"temp\";
        
        private static readonly string _saveDir = @"saves\";
        
        private static readonly string _tempPath = Path.Combine(_dir, _tempDir);
        
        private static readonly string _savePath = Path.Combine(_dir, _saveDir);
        
        private static readonly string _logFilePath = Path.Combine(_tempPath, "log.txt");

        #endregion

        #region PUBLIC_METHODS

        public static void CheckPath()
        {
            if(Directory.Exists(_tempPath) == false || File.Exists(_logFilePath) == false)
            {
                Directory.CreateDirectory(_tempPath);
                using (File.CreateText(_logFilePath)) { }
            }

            if(Directory.Exists(_savePath) == false)
            {
                Directory.CreateDirectory(_savePath);
            }
        }

        #endregion
    }
}