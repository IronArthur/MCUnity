using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MechCommanderUnity.API
{
    public class FITFile
    {

        #region Class Variables

        /// <summary>
        /// Managed file.
        /// </summary>
        internal FileProxy managedFile = new FileProxy();

        private List<string> _sections;

        private Dictionary<string, List<FITEntry>> _allSectionData;

        private int _packetNumber;

        private string ActualSection = "";

        #endregion

        #region Class Structures
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FITFile()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="filePath">Absolute path to *.IMG file.</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public FITFile(string filePath, FileUsage usage = FileUsage.UseMemory, bool readOnly = false)
        {
            this._sections = new List<string>();
            this._allSectionData = new Dictionary<string, List<FITEntry>>();

            Load(filePath, usage, readOnly);
        }

        /// <summary>
        /// Load constructor 
        /// </summary>
        /// <param name="data">byte[] data</param>
        /// <param name="usage">Specify if file will be accessed from disk, or loaded into RAM.</param>
        /// <param name="readOnly">File will be read-only if true, read-write if false.</param>
        public FITFile(byte[] data, FileUsage usage = FileUsage.UseMemory, bool readOnly = false)
        {
            this._sections = new List<string>();
            this._allSectionData = new Dictionary<string, List<FITEntry>>();

            Load(data, usage, readOnly);

        }

        #endregion

        #region Public Properties

        public int SectionNumber
        {
            get { return _sections.Count; }
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
            if (data == null)
                return false;

            // Load file
            if (!managedFile.Load(data, ""))
                return false;

            // Read file
            if (!Read())
                return false;

            return true;
        }

        public List<string> getFITSections()
        {
            return this._sections;
        }

        public Dictionary<string, List<FITEntry>> getAllFITData()
        {
            return this._allSectionData;
        }

        public List<FITEntry> getSectionData(string sectionName)
        {
            sectionName = this.insertBrackets(sectionName);
            List<FITEntry> list;
            if (!this._allSectionData.TryGetValue(sectionName, out list))
                return (List<FITEntry>)null;
            return list;
        }

        public void addSectionData(string sectionName, List<FITEntry> data)
        {
            sectionName = this.insertBrackets(sectionName);
            if (!this._sections.Contains(sectionName))
                this._sections.Add(sectionName);
            this.removeSectionData(sectionName);
            this._allSectionData.Add(sectionName, data);
        }

        public bool removeSectionData(string sectionName)
        {
            sectionName = this.insertBrackets(sectionName);
            return this._allSectionData.Remove(sectionName);
        }

        public bool getKeyValueBySection(string sectionName, string key, out string value)
        {
            value = "";
            List<FITEntry> sectionData = this.getSectionData(sectionName);
            if (sectionData == null)
                return false;
            //throw new ArgumentException("Error: Section does not exist.", sectionName );
            //foreach (INIEntry iniEntry in sectionData)
            //{
            //    string outValue;
            //    if (iniEntry.tryGetValueByKey(key, out outValue))
            //    {
            //        value = outValue.Replace("\"", "").Replace(";", "");
            //        return true;
            //    }

            //}
            for (int i = 0; i < sectionData.Count; i++)
            {
                string outValue;
                if (sectionData[i].tryGetValueByKey(key, out outValue))
                {
                    value = outValue.Replace("\"", "").Replace(";", "");
                    return true;
                }
            }

            return false;
            //throw new ArgumentException("Error: Key does not exist in section " + sectionName + ".", key);
        }

        public void setKeyValueBySection(string sectionName, string key, string value)
        {
            List<FITEntry> sectionData = this.getSectionData(sectionName);
            if (sectionData == null)
                return;
            foreach (FITEntry iniEntry in sectionData)
            {
                string outValue;
                if (iniEntry.tryGetValueByKey(key, out outValue))
                {
                    iniEntry.setValue(value);
                    break;
                }
            }
            this.addSectionData(sectionName, sectionData);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("FITini");
            stringBuilder.AppendLine();
            foreach (string sectionName in this._sections)
            {
                stringBuilder.AppendLine(sectionName);
                foreach (FITEntry iniEntry in this.getSectionData(sectionName))
                    stringBuilder.AppendLine(iniEntry.ToString());
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendLine("FITend");
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        public bool SeekSection(string sectionName)
        {
            List<FITEntry> sectionData = this.getSectionData(sectionName);
            if (sectionData == null)
                return false;

            this.ActualSection = sectionName;
            return true;

        }

        public bool GetFloat(string key, out float value)
        {
            value = -1;
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                if (float.TryParse(txtValue.Replace('.',','), out value))
                {
                    return true;
                }
            }

            return false;
        }

        public bool GetBool(string key, out bool value)
        {
            value = false;
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                if (bool.TryParse(txtValue, out value))
                {
                    return true;
                }
            }

            return false;
        }

        public bool GetInt(string key, out int value)
        {
            value = -1;
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                if (txtValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    //  txtValue = txtValue.Substring(4); , System.Globalization.NumberStyles.HexNumber, new System.Globalization.CultureInfo("en-US"),
                    try
                    {
                        value = Convert.ToInt32(txtValue, 16);
                        return true;
                    } catch (Exception e)
                    { UnityEngine.Debug.LogError(e.Message); }
                }
                if (int.TryParse(txtValue, out value))
                {
                    return true;
                }
            }

            return false;
        }

        public bool GetString(string key, out string value)
        {
            value = "";
            if (this.ActualSection == "")
            {
                return false;
            }

            if (this.getKeyValueBySection(this.ActualSection, key, out value))
            {
                return true;
            }
            return false;
        }

        public bool GetDouble(string key, out double value)
        {
            value = -1;
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                if (double.TryParse(txtValue, out value))
                {
                    return true;
                }
            }

            return false;
        }
        public bool GetLong(string key, out long value)
        {
            value = -1;
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                if (txtValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    //  txtValue = txtValue.Substring(4); , System.Globalization.NumberStyles.HexNumber, new System.Globalization.CultureInfo("en-US"),
                    try
                    {
                        value = Convert.ToInt64(txtValue, 16);
                        return true;
                    } catch (Exception e)
                    { UnityEngine.Debug.LogError(e.Message); }                   
                }
                if (long.TryParse(txtValue, out value))
                {
                    return true;
                }
            }

            return false;
        }
        public bool GetULong(string key, out ulong value)
        {
            value = 0;
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                if (ulong.TryParse(txtValue, out value))
                {
                    return true;
                }
            }

            return false;
        }
        public bool GetShort(string key, out short value)
        {
            value = -1;
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                if (short.TryParse(txtValue, out value))
                {
                    return true;
                }
            }

            return false;
        }
        public bool GetUShort(string key, out ushort value)
        {
            value = 0;
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                if (ushort.TryParse(txtValue, out value))
                {
                    return true;
                }
            }

            return false;
        }
        public bool GetChar(string key, out sbyte value)
        {
            value = 0;
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                if (sbyte.TryParse(txtValue, out value))
                {
                    return true;
                }
            }

            return false;
        }
        public bool GetUChar(string key, out byte value)
        {
            value = 0;
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                if (byte.TryParse(txtValue, out value))
                {
                    return true;
                }
            }

            return false;
        }
        public bool GetFloatArray(string key, out float[] value)
        {
            value = new float[0];
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                string[] auxArray = txtValue.Split(',');
                value = new float[auxArray.Length];

                for (int i = 0; i < auxArray.Length; i++)
                {
                    float valueAux;
                    if (!float.TryParse(auxArray[i].Replace(".", ","), out valueAux))
                    {
                        return false;
                    } else
                    {
                        value[i] = valueAux;
                    }
                }


            }

            return false;
        }
        public bool GetLongArray(string key, out long[] value)
        {
            value = new long[0];
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                string[] auxArray = txtValue.Split(',');
                value = new long[auxArray.Length];

                for (int i = 0; i < auxArray.Length; i++)
                {
                    long valueAux;
                    if (!long.TryParse(auxArray[i], out valueAux))
                    {
                        return false;
                    } else
                    {
                        value[i] = valueAux;
                    }
                }


            }

            return false;
        }
        public bool GetULongArray(string key, out ulong[] value)
        {
            value = new ulong[0];
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                string[] auxArray = txtValue.Split(',');
                value = new ulong[auxArray.Length];

                for (int i = 0; i < auxArray.Length; i++)
                {
                    ulong valueAux;
                    if (!ulong.TryParse(auxArray[i], out valueAux))
                    {
                        return false;
                    } else
                    {
                        value[i] = valueAux;
                    }
                }


            }

            return false;
        }
        public bool GetShortArray(string key, out short[] value)
        {
            value = new short[0];
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                string[] auxArray = txtValue.Split(',');
                value = new short[auxArray.Length];

                for (int i = 0; i < auxArray.Length; i++)
                {
                    short valueAux;
                    if (!short.TryParse(auxArray[i], out valueAux))
                    {
                        return false;
                    } else
                    {
                        value[i] = valueAux;
                    }
                }


            }

            return false;
        }
        public bool GetUShortArray(string key, out ushort[] value)
        {
            value = new ushort[0];
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                string[] auxArray = txtValue.Split(',');
                value = new ushort[auxArray.Length];

                for (int i = 0; i < auxArray.Length; i++)
                {
                    ushort valueAux;
                    if (!ushort.TryParse(auxArray[i], out valueAux))
                    {
                        return false;
                    } else
                    {
                        value[i] = valueAux;
                    }
                }


            }

            return false;
        }
        public bool GetCharArray(string key, out byte[] value)
        {
            value = new byte[0];
            if (this.ActualSection == "")
            {
                return false;
            }
            string txtValue = "";
            if (this.getKeyValueBySection(this.ActualSection, key, out txtValue))
            {
                string[] auxArray = txtValue.Split(',');
                value = new byte[auxArray.Length];

                for (int i = 0; i < auxArray.Length; i++)
                {
                    byte valueAux;
                    if (!byte.TryParse(auxArray[i], out valueAux))
                    {
                        return false;
                    } else
                    {
                        value[i] = valueAux;
                    }
                }


            }

            return false;
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
                var Reader = managedFile.GetStreamReader();

                List<string> list1 = new List<string>();
                while (!Reader.EndOfStream)
                {
                    string str = Reader.ReadLine();
                    if (str.StartsWith("[") && !str.EndsWith("]") && str.Contains("]"))
                    {
                        list1.Add(str.Split('/')[0].Trim().Split(']')[0] + "]");
                    } else if (!str.StartsWith("//"))
                        list1.Add(str.Split('/')[0].Trim());
                }
                foreach (string str in list1)
                {
                    if (str.StartsWith("[") && str.EndsWith("]") && !str.Contains("TransitionTable"))
                        this._sections.Add(str);
                    else if (str.StartsWith("[") && str.Contains("]") && !str.Contains("TransitionTable"))
                    {
                        var aux = str.Split(']');

                        this._sections.Add(aux[0] + "]");
                        list1[list1.IndexOf(str)] = aux[0] + "]";
                    }
                }

                foreach (string key in this._sections)
                {

                    List<FITEntry> list2 = new List<FITEntry>();
                    int index = list1.IndexOf(key) + 1;
                    while (index < list1.Count && !list1[index].StartsWith("[") && !list1[index].StartsWith("\u001a"))
                    {

                        if (list1[index].Equals("") || list1[index].StartsWith("fitend", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ++index;
                        } else if (list1[index].Split('=').Length == 1)
                        {
                            list2[list2.Count - 1].setValue(list2[list2.Count - 1].value + list1[index]);
                            ++index;
                        } else
                        {
                            try
                            {
                                list2.Add(new FITEntry(list1[index]));
                            } catch (Exception e2)
                            {
                                Console.WriteLine(e2.Message);
                                throw;
                            }

                            ++index;
                        }
                    }
                    this._allSectionData.Add(key, list2);
                }
                this._packetNumber = 0;//packet


            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private string insertBrackets(string sectionName)
        {
            if (!sectionName.StartsWith("[") || !sectionName.EndsWith("]"))
            {
                sectionName = sectionName.Insert(0, "[");
                sectionName = sectionName.Insert(sectionName.Length, "]");
            }
            return sectionName;
        }

        #endregion

    }
}
