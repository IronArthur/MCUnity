//using MechCommanderUnity.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MechCommanderUnity.API
{
    public class MapObjFile
    {
        #region Class Variables

        PakFile objPakFile;

        List<MCObj> Objs;
        /// <summary>
        /// Managed file.
        /// </summary>
        internal FileProxy managedFile = new FileProxy();

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
         /// <summary>
        /// Default constructor.
        /// </summary>
        public MapObjFile()
        {
        }

        /// <summary>
        /// Load constructor
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file.</param>
        public MapObjFile(string filePath)
        {

            Objs = new List<MCObj>();
            Load(filePath, FileUsage.UseMemory, true);
        }

        /// <summary>
        /// Load constructor
        /// </summary>
        /// <param name="data">Byte data of file</param>
        public MapObjFile(byte[] data)
        {
            Objs = new List<MCObj>();
            Load(data, FileUsage.UseMemory, true);
        }

        #endregion

        #region Public Properties



        #endregion

        #region Class Structures

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MCObj
        {
            public short ObjId;
            public short pixelOffsetX;
            public short pixelOffsetY;
            public short Vertex;
            public short Block;
            public byte damage;
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Loads an IMG file.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(string filePath, FileUsage usage, bool readOnly)
        {
            objPakFile = new PakFile(filePath);

            // Exit if this file already loaded
            //if (managedFile.FilePath == filePath)
            //    return true;

            //// Load file
            //if (!managedFile.Load(filePath, usage, readOnly))
            //    return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }

        /// <summary>
        /// Loads an IMG file.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public bool Load(byte[] data, FileUsage usage, bool readOnly)
        {
            objPakFile = new PakFile(data);

            //// Load file
            //if (!managedFile.Load(data, ""))
            //    return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }

        public MCObj[] GetObjs()
        {

            return Objs.ToArray();
           
        }

        public List<MCObj> GetLstObjs()
        {

            return Objs;

        }

        #endregion

        #region Private Methods



        #endregion

        #region Readers

        /// <summary>
        /// Read file.
        /// </summary>
        /// <returns>True if succeeded, otherwise false.</returns>
        private bool Read()
        {
            try
            {

                for (int i = 0; i < objPakFile.PakInnerFileCount; i++)
                {
                    var objdata = objPakFile.GetFileInner(i);

                    if (objdata == null || objdata.Length==0)
                        continue;

                    for (int x = 0; x < (objdata.Length/11); x++)
                    {
                        var obj = objdata.Skip((x * 11)).ToArray().Read<MCObj>();

                        Objs.Add(obj);
                    }

                }
                

            }
            catch (Exception e)
            {
               MechCommanderUnity.LogMessage(e.Message);
                return false;
            }

            return true;
        }

       
        #endregion
    }
}
