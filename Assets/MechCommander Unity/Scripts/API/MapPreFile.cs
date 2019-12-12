using MechCommanderUnity.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MechCommanderUnity.API
{
    class MapPreFile
    {
        #region Class Variables


        /// <summary>
        /// Managed file.
        /// </summary>
        internal FileProxy managedFile = new FileProxy();

        List<int> LstTileIndex;

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
         /// <summary>
        /// Default constructor.
        /// </summary>
        public MapPreFile()
        {
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file.</param>
        public MapPreFile(string filePath)
        {
            LstTileIndex = new List<int>();
            Load(filePath, FileUsage.UseMemory, true);
        }

        /// <summary>
        /// Load constructor
        /// Some IMG files contain palette information, this will overwrite the specified palette.
        /// </summary>
        /// <param name="data">Byte data of file</param>
        public MapPreFile(byte[] data)
        {
            LstTileIndex = new List<int>();
            Load(data, FileUsage.UseMemory, true);
        }

        #endregion

        #region Public Properties



        #endregion

        #region Class Structures

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
            // Exit if this file already loaded
            if (managedFile.FilePath == filePath)
                return true;

            // Load file
            if (!managedFile.Load(filePath, usage, readOnly))
                return false;

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
            // managedFile = new FileProxy(data, "");

            /*// Exit if this file already loaded
             if (managedFile.FilePath == filePath)
                 return true;*/

            // Load file
            if (!managedFile.Load(data, ""))
                return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }

        public List<int> GetLstTileIndex()
        {

             return LstTileIndex;
           
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
                // Step through file
                BinaryReader Reader = managedFile.GetReader();

                while (Reader.BaseStream.Position != Reader.BaseStream.Length)
                {
                    LstTileIndex.Add(Reader.ReadInt32());
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
