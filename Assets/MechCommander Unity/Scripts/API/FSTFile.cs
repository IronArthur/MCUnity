using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

namespace MechCommanderUnity.API
{
    public class FSTFile
    {
        #region Class Variables
        
        private struct TOCEntry
        {
            internal uint Offset;
            internal uint CompressedSize;
            internal uint UncompressedSize;
            internal string Filename;
        }

        /// <summary>Abstracts PAK file to a managed disk or memory stream.</summary>
        private FileProxy managedFile = new FileProxy();

        private Dictionary<string, byte[]> filesOnFst=new Dictionary<string, byte[]>();
        #endregion

        #region Public Properties
        public int NumberOfFiles
        {
            get { return filesOnFst.Count; }
        }

        public string[] ManagedPaths
        {
            get { return filesOnFst.Keys.ToArray(); }
        }
        #endregion

        #region Constructors

        public FSTFile()
        {
            filesOnFst=new Dictionary<string, byte[]>();
        }
        
        public FSTFile(string filePath)
        {
            Load(filePath);
        }
        
        public FSTFile(byte[] data)
        {
            Load(data);
        }
        #endregion
        
        
        #region Public Methods
        public bool Load(string filePath)
        {
            filePath = filePath.ToUpper();

            // Load file
            if (!managedFile.Load(filePath, FileUsage.UseMemory, true))
                return false;

            // Read file
            if (!Initialize())
                return false;

            // Managed file is no longer needed
            managedFile.Close();


            return true;
        }
        
        public bool Load(byte[] data)
        {
            // Load file
            if (!managedFile.Load(data, ""))
                return false;

            // Read file
            if (!Initialize())
                return false;
            
            managedFile.Close();

            return true;
        }

       

        public bool FileExist(string path)
        {
            return filesOnFst.ContainsKey(path);
        }

        public byte[] File(string path)
        {
            if (FileExist(path))
                return filesOnFst[path];
            return null;
        }
        
        public Dictionary<string,byte[]> Files(Func<string, bool> where)
        {
            return filesOnFst.Where(x => where(x.Key)).ToDictionary(x=>x.Key,x=>x.Value);
        }

        public void Save(string saveLocation)
        {
            List<FSTFile.TOCEntry> listEntries = new List<FSTFile.TOCEntry>();

            BinaryWriter binaryWriter = new BinaryWriter((Stream)new FileStream(saveLocation, FileMode.Create));
            binaryWriter.Write(filesOnFst.Count);
            int offset = filesOnFst.Count*262;
            foreach (var file in filesOnFst)
            {
                binaryWriter.Write(offset);
                binaryWriter.Write(file.Value.Length);
                binaryWriter.Write(file.Value.Length);
                char[] name= new char[250];
                Array.Copy(file.Key.ToCharArray(),name,file.Key.ToCharArray().Length);
                binaryWriter.Write(name);
                offset += file.Value.Length;
            }

            foreach (var file in filesOnFst)
            {
                binaryWriter.Write(file.Value);
            }
            
            binaryWriter.Close();
        }
        #endregion
        
        #region Private Methods

        private bool Initialize()
        {
            BinaryReader binaryReader = managedFile.GetReader(0);
            int length = binaryReader.ReadInt32();
            FSTFile.TOCEntry[] tocEntryArray = new FSTFile.TOCEntry[length];
            List<byte[]> list = new List<byte[]>();
            for (int index = 0; index < length; ++index)
            {
                tocEntryArray[index].Offset = binaryReader.ReadUInt32();
                tocEntryArray[index].CompressedSize = binaryReader.ReadUInt32();
                tocEntryArray[index].UncompressedSize = binaryReader.ReadUInt32();
                tocEntryArray[index].Filename = new string(binaryReader.ReadChars(250)).TrimEnd(new char[1]).Replace(@"data\","").ToUpper();
            }
            for (int index = 0; index < length; ++index)
            {
                byte[] outputBuffer = new byte[tocEntryArray[index].UncompressedSize];
                if ((int)tocEntryArray[index].CompressedSize == (int)tocEntryArray[index].UncompressedSize)
                    outputBuffer = binaryReader.ReadBytes((int)tocEntryArray[index].CompressedSize);
                else
                    LZFuncs.LZDecomp(out outputBuffer, binaryReader.ReadBytes((int)tocEntryArray[index].CompressedSize)
                        , tocEntryArray[index].UncompressedSize,
                        tocEntryArray[index].CompressedSize);
                list.Add(outputBuffer);
            }
            for (int index = 0; index < length; ++index)
            {
                //Debug.Log("Added "+tocEntryArray[index].Filename);
                filesOnFst.Add(tocEntryArray[index].Filename, list[index]);
            }

            return true;
        }
        

        #endregion
    }
}