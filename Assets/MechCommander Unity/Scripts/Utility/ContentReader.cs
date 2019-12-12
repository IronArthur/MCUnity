// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MechCommanderUnity.API;
using System.Xml.Serialization;
using System.Linq;

namespace MechCommanderUnity.Utility
{
    /// <summary>
    /// General interface between MCGConnect API reader classes and Unity.
    /// </summary>
    public class ContentReader
    {
        MechCommanderUnity mcUnity;
        bool isReady = false;
        string mcPath;

        public static bool PaletteXpansion = false;

        PakFile objPakFile = null;
        PakFile spritesPakFile = null;
        PakFile shapesPakFile = null;
        PakFile shapes90PakFile = null;
        PakFile torsoPakFile = null;
        PakFile legsPakFile = null;
        PakFile larmPakFile = null;
        PakFile rarmPakFile = null;
        PakFile tilesPakFile = null;
        PakFile gtilesPakFile = null;

        
        string MapFitPath = "Terrain";
        string objpakpath = "OBJECTS/OBJECT2.PAK";
        string spritespakpath = "SPRITES/SPRITES.PAK";

        string shapespakpath = "SPRITES/SHAPES.PAK";
        string shapes90pakpath = "SPRITES/SHAPES90.PAK";

        string pathTorso = "SPRITES/TORSOS90.PAK";
        string pathLegs = "SPRITES/LEGS90.PAK";
        string pathLarm = "SPRITES/LARMS90.PAK";
        string pathRarm = "SPRITES/RARMS90.PAK";

        string pathTiles = "TILES/TILES.PAK";
        string pathTiles90 = "TILES/TILES90.PAK";
        string pathGTiles = "TILES/GTILES.PAK";
        string pathGTiles90 = "TILES/GTILES90.PAK";

        
        const string palettePath = @"palette\HB.PAL";
        const string GpalettePath = @"palette\GHB.PAL";

        int lastObjIndex = -1;
        PakFile auxPak = null;

        Dictionary<int, PakFile> shapePakFiles = new Dictionary<int, PakFile>();

        public bool IsReady
        {
            get { return isReady; }
        }

        public PakFile ObjPakFile
        {
            get { return objPakFile; }
        }

        public PakFile SpritesPakFile
        {
            get { return spritesPakFile; }
        }

        public PakFile ShapesPakFile
        {
            get { return shapesPakFile; }
        }

        public PakFile Shapes90PakFile
        {
            get { return shapes90PakFile; }
        }

        public PakFile TorsoPakFile
        {
            get { return torsoPakFile; }
        }

        public PakFile LegsPakFile
        {
            get { return legsPakFile; }
        }

        public PakFile LarmPakFile
        {
            get { return larmPakFile; }
        }

        public PakFile RarmPakFile
        {
            get { return rarmPakFile; }
        }

        public PakFile TilesPakFile
        {
            get { return tilesPakFile; }
        }

        public PakFile GTilesPakFile
        {
            get { return gtilesPakFile; }
        }


        #region Constructors

        public ContentReader(string mcPath, MechCommanderUnity mcUnity)
        {
            this.mcPath = mcPath;
            this.mcUnity = mcUnity;

            SetupReaders();
        }

        #endregion

        #region Blocks & Locations

        public bool GetFitObject(int ObjIndex, out FITFile fitFile)
        {
            fitFile = new FITFile();

            if (!isReady)
                return false;

            // Get block data

            var data = objPakFile.GetFileInner(ObjIndex);


            if (data == null)
            {
                MechCommanderUnity.LogMessage(string.Format("Unknown FITFILE OBJECT.PAK '{0}'.", ObjIndex), true);
                return false;
            }

            fitFile = new FITFile(data);

            return true;
        }


        public bool GetFitSprite(int ObjIndex, out FITFile fitFile)
        {
            fitFile = new FITFile();

            if (!isReady)
                return false;

            // Get block data

            var data = spritesPakFile.GetFileInner(ObjIndex);


            if (data == null)
            {
                MechCommanderUnity.LogMessage(string.Format("Unknown FITFILE SPRITE.PAK '{0}'.", ObjIndex), true);
                return false;
            }

            fitFile = new FITFile(data);

            return true;
        }

        public bool GetShape(int ObjIndex, out List<MCBitmap> lstImgs, int innerShp = -1, int indexImg = -1)
        {
            lstImgs = new List<MCBitmap>();

            if (!isReady)
                return false;

            // Get block data

            var maxShp = shapesPakFile.PakInnerFileCount;

            bool result = false;

            PakFile pak = null;

            if (!shapePakFiles.ContainsKey(ObjIndex))
            {
                var pakData = shapesPakFile.GetFileInner(ObjIndex);

                if (pakData == null)
                {
                    pakData = shapes90PakFile.GetFileInner(ObjIndex);
                }
                if (pakData != null)
                {
                    pak = new PakFile(pakData);
                    shapePakFiles.Add(ObjIndex, pak);
                }

            } else
            {
                pak = shapePakFiles[ObjIndex];
            }


            if (pak != null)
            {
                var shpdata = pak.GetFileInner(innerShp);

                Debug.Log("SHP: " + ObjIndex+" - "+ innerShp);

                if (shpdata != null)
                {
                    result = true;
                    
                    var shp = new ShpFile(shpdata);

                    if (indexImg > -1)
                    {
                        lstImgs.Add(shp.GetBitMap(indexImg));
                    } else
                    {
                        lstImgs.AddRange(shp.GetBitMaps());
                    }
                }


            }

            if (!result)
            {
                MechCommanderUnity.LogMessage(string.Format("Unknown Shape '{0}' '{1}' '{2}'.", ObjIndex, innerShp, indexImg), true);
                return false;
            }

            return true;
        }

        public bool GetShapes(int ObjIndex, out List<MCBitmap> lstImgs, List<int> shpIds)
        {
            lstImgs = new List<MCBitmap>();

            if (!isReady)
                return false;

            // Get block data

            var maxShp = shapesPakFile.PakInnerFileCount;

            bool result = false;

            PakFile pak = null;

            try
            {
                if (!shapePakFiles.ContainsKey(ObjIndex))
                {
                    var pakData = shapesPakFile.GetFileInner(ObjIndex);

                    if (pakData == null)
                    {
                        pakData = shapes90PakFile.GetFileInner(ObjIndex);
                    }
                    if (pakData != null)
                    {
                        pak = new PakFile(pakData);
                        shapePakFiles.Add(ObjIndex, pak);
                    }
                } else
                {
                    pak = shapePakFiles[ObjIndex];
                }


                if (pak != null)
                {
                    foreach (var innerShp in shpIds)
                    {
                        var shpdata = pak.GetFileInner(innerShp);

                        if (shpdata != null)
                        {
                            result = true;
                            var shp = new ShpFile(shpdata);

                            var lstBmps = shp.GetBitMaps();
                            for (int i = 0; i < lstBmps.Length; i++)
                            {
                                if (lstBmps[i] == null)
                                {
                                    continue;
                                    Debug.Log("BMP Vacio exportando: " + ObjIndex+ " - " + innerShp +" : " + i);
                                    lstBmps[i] = new MCBitmap(1, 1);
                                }
                                lstBmps[i].Name = innerShp  + "-" + i;
                            }
                            lstImgs.AddRange(lstBmps.Where(x=>x!=null).ToArray());
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
            if (!result)
            {
                MechCommanderUnity.LogMessage(string.Format("Unknown Shape '{0}' '{1}' t:{2}.", ObjIndex, string.Join(",", shpIds.Select(x => x.ToString()).ToArray()), pak.PakInnerFileCount), true);
                return false;
            }

            return true;
        }

        public bool GetAllShapes(int ObjIndex, out List<MCBitmap> lstImgs)
        {
            lstImgs = new List<MCBitmap>();

            if (!isReady)
                return false;

            // Get block data

            var maxShp = shapesPakFile.PakInnerFileCount;

            bool result = false;

            var pakData = shapesPakFile.GetFileInner(ObjIndex);

            if (pakData == null)
            {
                pakData = shapes90PakFile.GetFileInner(ObjIndex);
            }
            if (pakData != null)
            {
                var pak = new PakFile(pakData);

                for (int i = 0; i < pak.PakInnerFileCount; i++)
                {
                    var shpdata = pak.GetFileInner(i);

                    if (shpdata != null)
                    {
                        result = true;
                        var shp = new ShpFile(shpdata);

                        lstImgs.AddRange(shp.GetBitMaps());
                    }
                }
            }

            return result;
        }
        

        public bool GetMapPreview(string Mapname, out Texture2D preview)
        {
            preview = new Texture2D(1, 1);
            if (!isReady || Mapname=="")
                return false;

            var mapPreviewPath = Path.Combine(MapFitPath, Mapname + ".tga").ToUpper();
            var fileData = MechCommanderUnity.Instance.FileManager.File(mapPreviewPath);
            if (fileData!=null)
            { 
                MechCommanderUnity.LogMessage("TGA file " + mapPreviewPath);
                preview= TGALoader.LoadTGA(new MemoryStream(fileData));
                return  true;
            }
            else
            {
                var missionFile = new FITFile(MechCommanderUnity.Instance.FileManager.File(MCGExtensions.PathCombine(new string[] { "missions", Mapname + ".fit"})));
                missionFile.SeekSection("TerrainSystem");
                missionFile.GetString("TerrainFileName", out string terrainFileName);
                mapPreviewPath = Path.Combine(MapFitPath, terrainFileName + ".tga").ToUpper();
                fileData = MechCommanderUnity.Instance.FileManager.File(mapPreviewPath);
                if (fileData!=null)
                { 
                    MechCommanderUnity.LogMessage("TGA file DIFFERENT NAME " + mapPreviewPath);
                    preview= TGALoader.LoadTGA(new MemoryStream(fileData));
                    return  true;
                }
            }

            MechCommanderUnity.LogMessage("Can´t Find TGA file " + mapPreviewPath);
            return false;

        }
        
        public bool GetListMaps(out List<string> ListMaps)
        {
            ListMaps = new List<string>();
            if (!isReady)
                return false;

            DirectoryInfo dir = new DirectoryInfo(Path.Combine(mcPath, MapFitPath));
            FileInfo[] info = dir.GetFiles("*.fit");

            for (int i = 0; i < info.Length; i++)
            {
                ListMaps.Add(Path.GetFileNameWithoutExtension(info[i].Name));
            }

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Setup API file readers.
        /// </summary>
        private void SetupReaders()
        {
            // Setup general content readers
            // Ensure reader is created

            //palettePath = Path.Combine(mcPath, "logistic.pal");


            //if (objPakFile == null)
            //{
            //    objPakFile = new PakFile(Path.Combine(mcPath, objpakpath), palettePath);
            //}
            //if (spritesPakFile == null)
            //{
            //    spritesPakFile = new PakFile(Path.Combine(mcPath, spritespakpath), palettePath);
            //}
            if (shapesPakFile == null)
            {
                shapesPakFile = new PakFile(Path.Combine(mcPath, shapespakpath), 
                    MechCommanderUnity.Instance.FileManager.File(PaletteXpansion? GpalettePath: palettePath));
            }
            if (shapes90PakFile == null)
            {
                shapes90PakFile = new PakFile(Path.Combine(mcPath, shapes90pakpath), 
                    MechCommanderUnity.Instance.FileManager.File(PaletteXpansion? GpalettePath: palettePath));
            }
            //if (torsoPakFile == null)
            //{
            //    torsoPakFile = new PakFile(Path.Combine(mcPath, pathTorso), palettePath);
            //}
            //if (legsPakFile == null)
            //{
            //    legsPakFile = new PakFile(Path.Combine(mcPath, pathLegs), palettePath);
            //}
            //if (rarmPakFile == null)
            //{
            //    rarmPakFile = new PakFile(Path.Combine(mcPath, pathRarm), palettePath);
            //}
            //if (larmPakFile == null)
            //{
            //    larmPakFile = new PakFile(Path.Combine(mcPath, pathLarm), palettePath);
            //}
            //if (tilesPakFile == null)
            //{
            //    tilesPakFile = new PakFile(Path.Combine(mcPath, pathTiles90), palettePath);
            //}
            //if (gtilesPakFile == null)
            //{
            //    gtilesPakFile = new PakFile(Path.Combine(mcPath, pathGTiles90), GpalettePath);
            //}
            
            isReady = true;
        }

        #endregion
    }
}
