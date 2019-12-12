#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MechCommanderUnity.API;
using MechCommanderUnity.MCG;
using MechCommanderUnity.Utility;
using Unity.EditorCoroutines.Editor;

namespace MechCommanderUnity
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class MechCommanderUnity : MonoBehaviour
    {
        #region Fields

        bool isReady = false;
        bool isPathValidated = false;
        private ContentReader reader;
        private FileManager _fileManager;

        
        #endregion

        #region Public Fields

        public Mission MissionComponent;
        
        public string MCGRemotePath;
        public string MCGPath = "MCG_DATA";
        
        public int ModelImporter_ModelID = 0;
        public int Map_ModelIndex = 0;
        public string MapName = "";
        public bool PreviewHasChanged = false;

        public List<string> ListMaps = new List<string>();
        
        #endregion

        #region Class Properties

        public bool IsReady
        {
            get { return isReady; }
        }

        public ContentReader ContentReader
        {
            get
            {
                if (reader == null)
                    SetupContentReaders();
                return reader;
            }
        }
        
        public FileManager FileManager
        {
            get
            {
                if (_fileManager == null)
                    SetupContentReaders();
                return _fileManager;
            }
        }
        #endregion


        #region Singleton

        static MechCommanderUnity instance = null;
        public static MechCommanderUnity Instance
        {
            get
            {
                if (instance == null)
                {
                    if (!FindMechCommanderUnity(out instance))
                    {
                        GameObject go = new GameObject();
                        go.name = "MechCommanderUnity";
                        instance = go.AddComponent<MechCommanderUnity>();
                        
                        instance.MissionComponent = GameObject.FindObjectOfType(typeof(Mission)) as Mission;
                    }
                }
                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return (instance != null);
            }
        }

        #endregion

        #region Unity

        void Awake()
        {
            SetupSingleton();
            SetupMCGPath();
            //SetupContentReaders();
        }

        void Update()
        {
            // Instance must be set up
            //if (!Setup())
            //    return;

//#if UNITY_EDITOR
//
//            if (!isReady) SetupMCGPath();
//
//            // Content readers must be ready
//            // This is checked every update in editor as
//            // code changes can reset singleton fields
//            if (reader == null) SetupContentReaders();
//
//#endif
        }

        #endregion

        #region Editor-Only Methods

#if UNITY_EDITOR

        public void CopyMCGAssetsToLocal()
        {
            Debug.Log("StartingCoroutine Copying Assets");
            EditorCoroutineUtility.StartCoroutine(CopyMCGAssetsToLocal(MCGRemotePath, MCGPath, EditorResetMCGPath),this);
        }

        private IEnumerator CopyMCGAssetsToLocal(string remoteMCGPath, string localMCGPath, Action onFinish)
        {
            string remoteDataPath = Path.Combine(remoteMCGPath, "DATA");

            Directory.CreateDirectory(localMCGPath);
            
            yield return null;

            string[] PakFilesToCopy = 
            {
                "OBJECTS/OBJECT2.PAK",
                "SPRITES/SHAPES.PAK",
                "SPRITES/SHAPES90.PAK",
                "SPRITES/SPRITES.PAK",
                "TILES/TILES.PAK",
                "TILES/TILES90.PAK",
                "TILES/GTILES.PAK",
                "TILES/GTILES90.PAK",
            };
            
            string[] FstFilesToCopy = 
            {
                "MISC.FST",
                "MISSION.FST",
                "TERRAIN.FST",
            };

            var totalFilesToCopy = PakFilesToCopy.Length + FstFilesToCopy.Length;

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayProgressBar("Copying MCG Files to Local Dir",
                "Copying MCG FIles to Local Dir", (float) 0 / totalFilesToCopy);


            for (int i = 0; i < PakFilesToCopy.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Copying MCG Files to Local Dir",
                    $"Copying {PakFilesToCopy[i]} to Local Dir", (float)i / totalFilesToCopy);
                
                yield return CopyFile(Path.Combine(remoteDataPath, PakFilesToCopy[i]),
                        Path.Combine(localMCGPath, PakFilesToCopy[i]));
                
            }
            
            for (int i = 0; i < FstFilesToCopy.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Copying MCG Files to Local Dir",
                    $"Copying {FstFilesToCopy[i]} to Local Dir", (float)(i+PakFilesToCopy.Length) / totalFilesToCopy);
               
                
                yield return CopyFile(Path.Combine(remoteMCGPath, FstFilesToCopy[i]),
                    Path.Combine(localMCGPath, FstFilesToCopy[i]));
                
            }

            EditorUtility.ClearProgressBar();
            
            onFinish();
        }

        private IEnumerator CopyFile(string remoteFile, string localFile)
        {
            if (!File.Exists(localFile))
            {
//                LogMessage("Starting Copying "+localFile);
                Directory.CreateDirectory(Path.GetDirectoryName(localFile));

                if (remoteFile.EndsWith("PAK"))
                {
                    var pak= new PakFile(remoteFile);
                    pak.Save(localFile);
                }
                else if (remoteFile.EndsWith("FST"))
                {
                    var fst=new FSTFile(remoteFile);
                    fst.Save(localFile);
                }
                
            }

            yield return null;
        }
        
        /// <summary>
        /// Setup path and content readers again.
        /// Used by editor when setting new MCGPath.
        /// </summary>
        public void EditorResetMCGPath()
        {
            SetupMCGPath();
            SetupContentReaders(true);
        }

        /// <summary>
        /// Clear Arena2 path in editor.
        /// Used when you wish to decouple from MCG for certain builds.
        /// </summary>
        public void EditorClearMCGPath()
        {
            MCGPath = string.Empty;
            EditorResetMCGPath();
        }

        public void SelectMapToLoad(string MapName)
        {
            this.MapName = MapName;
            this.PreviewHasChanged = true;
            if (MissionComponent != null)
            {
                MissionComponent.MissionName = MapName;

                if (!EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = true;
                }
            }
            
        }
        
        
        [EditorButton]
        void ExportPalette(bool Image = true, int i = 1)
        {
            var pal = MechCommanderUnity.Instance.ContentReader.ShapesPakFile.Palette;

            if (!Image)
            {


                string ExportPal = "";
                for (int z = 0; z < 256; z++)
                {
                    ExportPal += z.ToString() + " : " + pal.Get(z, true).ToString() + "\r\n";
                }
                System.IO.File.WriteAllText("Palette" + ".txt", ExportPal);
            }
            {
                var bmpT = new MCBitmap(256, 1);


                for (int y = 0; y < 256; y++)
                {
                    MCBitmap.SetPixel(bmpT, y, 0, (byte)y);
                }
                var PalText = ImageProcessing.MakeTexture2D(bmpT, MechCommanderUnity.Instance.ContentReader.ShapesPakFile.Palette);
                ImageProcessing.SaveTextureAsPng(PalText, "PAL");
            }



            return;

            //  var mccolors=pal.ExportPalette();

            //  int i = 4;

            var bmp = new MCBitmap(16 * i, 16 * i);


            for (int y = 0; y < 16 * i; y++)
            {
                for (int x = 0; x < 16 * i; x++)
                {
                    int pos = (((y * bmp.Stride) / i) + (x / i)) * bmp.FormatWidth;

                    MCBitmap.SetPixel(bmp, x, y, (byte)pos);
                }
            }

            string Export = "";
            for (int y = 0; y < 16 * i; y++)
            {
                for (int x = 0; x < 16 * i; x++)
                {
                    int pos = (((y * bmp.Stride) / i) + (x / i)) * bmp.FormatWidth;

                    Export += bmp.Data[pos].ToString("D3") + ",";
                }

                Export += "\r\n";
            }

            System.IO.File.WriteAllText("PAL" + i + ".txt", Export);

            var MainText = ImageProcessing.MakeTexture2D(bmp, MechCommanderUnity.Instance.ContentReader.ShapesPakFile.Palette);
            ImageProcessing.SaveTextureAsPng(MainText, "PAL" + i);//Assets/

            //var MainText = ImageProcessing.MakeTexture2D(ref bmp);
            //ImageProcessing.SaveTextureAsPng(MainText, "PAL");//Assets/

        }
        
        [EditorButton]
        void ExportShpToTxt(int idShape)
        {
            List<MCBitmap> lstBmp = new List<MCBitmap>();

            var shp = MechCommanderUnity.Instance.ContentReader.GetShape(idShape, out lstBmp, 0);

            for (int i = 0; i < lstBmp.Count; i++)
            {
                var bmp = lstBmp[i];
                string Export = "";
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        int pos = (((y * bmp.Stride)) + (x)) * bmp.FormatWidth;

                        Export += bmp.Data[pos].ToString("D3") + ",";
                    }

                    Export += "\r\n";
                }
                System.IO.File.WriteAllText(idShape + "-" + i + ".txt", Export);
            }

        }
        
        [EditorButton]
        void ExtractArt()
        {
            var artpak = new PakFile(@"MCG_DATA\ART\ART.PAK");
            MCBitmap atlas;
            Dictionary<string, Rect> dict;
            for (int i = 0; i < artpak.PakInnerFileCount; i++)
            {
                var aux = artpak.GetFileInner(i);

                if (aux != null)
                {
                    if (aux[0] == 71 && aux[1] == 73 && aux[2] == 70)
                    {
                        System.IO.File.WriteAllBytes(@"a\" + i + ".gif", aux);
                    } else
                    {
                        System.IO.File.WriteAllBytes(@"a\" + i + ".shp", aux);

                        try
                        {
                            var shp = new ShpFile(aux);
                            ImageProcessing.CreateAtlas(shp.GetBitMaps(), out atlas, out dict);
                            var MainText = ImageProcessing.MakeTexture2D(atlas, MechCommanderUnity.Instance.ContentReader.ShapesPakFile.Palette);
                            ImageProcessing.SaveTextureAsPng(MainText, @"a\" + i);
                        } catch (Exception)
                        {}
                    }
                }
            }
        }
        
        
#endif


        #endregion

        #region Startup and Shutdown

        public void SetupMCGPath()
        {
            RaiseOnSetMCGSourceEvent();

            // Check path is valid
            if (ValidateLocalMCGPath())
            {
                isReady = true;
                isPathValidated = true;
                //LogMessage("MCG path validated.", true);
                //System.IO.File.Delete("cache/ObjDictionary.xml");

                //SetupMCGPath();
                SetupContentReaders();
                SetupMapList();

                return;
            } else
            {
                // Look for mcg folder in Application.dataPath at runtime
                if (Application.isPlaying)
                {
                    string path = Path.Combine(Application.dataPath, "MCG_DATA");
                    if (Directory.Exists(path))
                    {
                        // If it appears valid set this is as our path
                        if (ValidateMCGPath(path))
                        {
                            MCGPath = path;
                            isReady = true;
                            isPathValidated = true;
                            LogMessage($"Found valid MCG path at '{path}'.");
                            //System.IO.File.Delete("cache/ObjDictionary.xml");

                            //SetupSingleton();
                            //SetupMCGPath();
                            //SetupContentReaders();

                            return;
                        }
                    }
                }
            }

            // No path was found but we can try to carry on without one
            isReady = false;
            isPathValidated = false;

            // Singleton is now ready
            RaiseOnReadyEvent();

        }

        private void SetupSingleton()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
            {
                if (Application.isPlaying)
                {
                    LogMessage("Multiple MechCommanderUnity instances detected!", true);
                    Destroy(gameObject);
                }
            }
        }

        private void SetupContentReaders(bool force = false)
        {
            if (isPathValidated && _fileManager==null)
            {
                _fileManager=new FileManager(MCGPath);
            }
            if (reader == null || force)
            {
                // Ensure content readers available even when path not valid
                if (isPathValidated)
                {
                    //MechCommanderUnity.LogMessage(string.Format("Setting up content readers with MCG path '{0}'.", MCGPath));
                    reader = new ContentReader(MCGPath, this);
                } else
                {

                    MechCommanderUnity.LogMessage(string.Format("Setting up content readers without MCG path. Not all features will be available."));
                    reader = new ContentReader(string.Empty, this);
                }
            }
            
        }

        private void SetupMapList()
        {
            if (isPathValidated && isReady)
            {
                ListMaps = new List<string>();

                var allMaps=FileManager.Files(s => s.StartsWith("MISSIONS") && s.EndsWith(".FIT"));
                ListMaps.AddRange(allMaps.Keys.Select(Path.GetFileNameWithoutExtension));
            }
        }

        public bool ValidateLocalMCGPath()
        {
            if (!ValidateMCGPath(MCGPath))
                return false;
            
            if (!ValidateMCGAssetPath(Path.Combine(MCGPath, "MISC.FST")))
                return false;
            if (!ValidateMCGAssetPath(Path.Combine(MCGPath, "MISSION.FST")))
                return false;
            if (!ValidateMCGAssetPath(Path.Combine(MCGPath, "TERRAIN.FST")))
                return false;
            
            return true;
        }
        
        public bool ValidateRemoteMCGPath(string remotePath)
        {
            if (!ValidateMCGPath(Path.Combine(remotePath, "DATA")))
                return false;
            
            if (!ValidateMCGAssetPath(Path.Combine(remotePath, "MISC.FST")))
                return false;
            if (!ValidateMCGAssetPath(Path.Combine(remotePath, "MISSION.FST")))
                return false;
            if (!ValidateMCGAssetPath(Path.Combine(remotePath, "TERRAIN.FST")))
                return false;

            return true;
        }

        public bool ValidateMCGAssetPath(string pathToAsset)
        {
            if (File.Exists(pathToAsset))
                return true;
            LogMessage("Can´t find: "+pathToAsset);
            return false;
        }
        public bool ValidateMCGPath(string path)
        {
            if (!ValidateMCGAssetPath(Path.Combine(path, "OBJECTS/OBJECT2.PAK")))
                return false;
            
            if (!ValidateMCGAssetPath(Path.Combine(path, "SPRITES/SHAPES.PAK")))
                return false;
            if (!ValidateMCGAssetPath(Path.Combine(path, "SPRITES/SHAPES90.PAK")))
                return false;
            
            if (!ValidateMCGAssetPath(Path.Combine(path, "SPRITES/SPRITES.PAK")))
                return false;
            
            if (!ValidateMCGAssetPath(Path.Combine(path, "TILES/TILES.PAK")))
                return false;
            if (!ValidateMCGAssetPath(Path.Combine(path, "TILES/TILES90.PAK")))
                return false;
            if (!ValidateMCGAssetPath(Path.Combine(path, "TILES/GTILES.PAK")))
                return false;
            if (!ValidateMCGAssetPath(Path.Combine(path, "TILES/GTILES90.PAK")))
                return false;
            
           
            
            return true;
        }

        
        

#if UNITY_EDITOR
        private void LoadDeveloperMCGPath()
        {
            const string devMCGPath = "devMCGPath";

            // Do nothing if path already set or playing
            if (!string.IsNullOrEmpty(MCGPath) || Application.isPlaying)
                return;

            // Attempt to load persistent dev path from Resources
            TextAsset path = Resources.Load<TextAsset>(devMCGPath);
            if (path)
            {
                if (Directory.Exists(path.text))
                {
                    // If it looks valid set this is as our path
                    if (ValidateMCGPath(path.text))
                    {
                        MCGPath = path.text;
                        EditorUtility.SetDirty(this);
                    }
                }
            }
        }
#endif

        #endregion

        #region Public Static Methods

        public static void LogMessage(string message, bool showInEditor = true)
        {
            if (showInEditor || Application.isPlaying) Debug.Log(string.Format("MC: {0}", message));
        }

        public static void ClearConsole()
        {
            // This simply does "LogEntries.Clear()" the long way:
            var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }

        public static bool FindMechCommanderUnity(out MechCommanderUnity mcUnityOut)
        {
            mcUnityOut = GameObject.FindObjectOfType(typeof(MechCommanderUnity)) as MechCommanderUnity;
            if (mcUnityOut == null)
            {
                LogMessage("Could not locate MechCommanderUnity GameObject instance in scene!", true);
                return false;
            }

            return true;
        }

        #endregion

        #region Event Handlers

        // OnReady
        public delegate void OnReadyEventHandler();
        public static event OnReadyEventHandler OnReady;
        protected virtual void RaiseOnReadyEvent()
        {
            if (OnReady != null)
                OnReady();
        }

        // OnSetArena2Source
        public delegate void OnSetMCGSourceEventHandler();
        public static event OnSetMCGSourceEventHandler OnSetMCGSource;
        protected virtual void RaiseOnSetMCGSourceEvent()
        {
            if (OnSetMCGSource != null)
                OnSetMCGSource();
        }

        #endregion
    }
}