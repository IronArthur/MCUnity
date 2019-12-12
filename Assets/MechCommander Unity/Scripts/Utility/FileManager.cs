using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using MechCommanderUnity.API;
using UnityEngine;

namespace MechCommanderUnity.Utility
{
    public class FileManager
    {
        string[] FstToLoad = new string[]
        {
            "MISC.FST",
            "MISSION.FST",
            "TERRAIN.FST",
        };
        
        FSTFile _fstFile= new FSTFile();

        public FileManager(string basePath)
        {
            foreach (var file in FstToLoad)
            {
                _fstFile.Load(Path.Combine(basePath,file));
            }
        }
        
        public bool FileExist(string path)
        {
            return _fstFile.FileExist(path);
        }

        public byte[] File(string path)
        {
            string pathUpper=path.ToUpper();
            if (FileExist(pathUpper))
                return _fstFile.File(pathUpper);
            Debug.LogError($"Can´t find {pathUpper} int {_fstFile.NumberOfFiles} Files ");
            //Debug.LogError(string.Join(", ",_fstFile.ManagedPaths));
            return null;
        }
        
        public Dictionary<string,byte[]> Files(Func<string, bool> where)
        {
            return _fstFile.Files(where);
        }
        
        
    }
}