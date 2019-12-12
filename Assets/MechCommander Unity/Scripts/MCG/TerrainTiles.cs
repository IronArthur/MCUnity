using MechCommanderUnity.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MechCommanderUnity.Utility;
using UnityEngine;
using sspack;
using System.Threading;

namespace MechCommanderUnity.MCG
{
    public class TerrainTiles
    {

        #region Class Variables

        string terrainFileName;
        string TerrainTileFile;

        List<int> LstTileIndexes;

        private Dictionary<int, MCBitmap> LstTiles;
        private Dictionary<int, MCBitmap> LstTilesOV;

        public Texture2D MainText;
        public Texture2D MainOVText;
        public Texture2D PalText;
        public Dictionary<string, TileInfo> DictTileInfo;
        public Dictionary<string, TileInfo> DictTileOVInfo;

        
        const string palettePath = @"palette\HB.PAL";
        const string GpalettePath = @"palette\GHB.PAL";
        #endregion



        #region Class Structures

        public struct TileInfo
        {
            public Rect rect;
            public Vector2 pivot;
        }

        #endregion

        #region Constructors

        public TerrainTiles(string terrainFileName, string TerrainTileFile, int NumBlocks, int NumVertices)
        {
            this.terrainFileName = terrainFileName;
            this.TerrainTileFile = TerrainTileFile;



            /*  string PreFileName = System.IO.Path.ChangeExtension(terrainFileName, "pre");

              MapPreFile terrainPreFile = new MapPreFile(MCGExtensions.PathCombine(new string[] { MechCommanderUnity.Instance.MCGPath, "terrain", PreFileName }));

              var lstPreloadTiles = terrainPreFile.GetLstTileIndex();*/

            string ElvFileName = Path.ChangeExtension(terrainFileName, "elv");
            
            MapElvFile terrainElvFile = new MapElvFile(MechCommanderUnity.Instance.FileManager.File(MCGExtensions.PathCombine(new string[]
                {"terrain", ElvFileName})), NumBlocks, NumVertices);

            LstTileIndexes = terrainElvFile.GetDifferentTileIds();



            /*
               MCBitmap atlas;
               Dictionary<string, Rect> rects;
               var result = ImageProcessing.CreateAtlas(LstTiles.Values.ToArray(), out atlas, out rects);

               if (result)
               {
                   UnityMainThreadDispatcher.Instance().Enqueue(() =>
                   {
                       var packed = ImageProcessing.MakeTexture2D(ref atlas, pak.Palette);
                       ImageProcessing.SaveTextureAsPng(packed, "prefiles");
                   });
               }
               */


        }


        #endregion


        public MCBitmap GetTileBitmap(int index)
        {
            if (LstTiles.ContainsKey(index))
            {
                return LstTiles[index];
            } else return null;
        }

        public IEnumerator Init(Func<IEnumerator> OnFinish)
        {
            if (this.IsCached())
            {
                yield return OnFinish();
            }
            else
            {
               PakFile pak;
            
                if (TerrainTileFile.ToUpper() == "Tiles".ToUpper())
                {
                    pak = new PakFile(Path.Combine(MechCommanderUnity.Instance.MCGPath, "TILES/TILES90.PAK"), 
                        MechCommanderUnity.Instance.FileManager.File(palettePath));
                } else if (TerrainTileFile.ToUpper() == "GTiles".ToUpper())
                {
                    pak = new PakFile(Path.Combine(MechCommanderUnity.Instance.MCGPath, "TILES/GTILES90.PAK"), 
                        MechCommanderUnity.Instance.FileManager.File(GpalettePath));
                } else
                {
                    throw (new Exception());
                }

                this.LstTiles = new Dictionary<int, MCBitmap>();
                this.LstTilesOV = new Dictionary<int, MCBitmap>();

                for (int i = 0; i < LstTileIndexes.Count; i++)
                {
                    var tiledata = pak.GetFileInner(LstTileIndexes[i]);

                    if (tiledata == null)
                        continue;

                    MCBitmap bitmap;
                    if (tiledata[0] != 68 && tiledata[1] != 78)
                    {
                        var tile = new MCTileFile(tiledata);
                        bitmap = tile.GetBitMap();
                        if (bitmap == null)
                            continue;
                        bitmap.Name = LstTileIndexes[i].ToString();//this.TerrainTileFile.ToUpper() + "-" + 

                        LstTiles.Add(LstTileIndexes[i], bitmap);
                        
                    } else
                    {
                        var tile = new MCTileFileOverlay(tiledata);
                        bitmap = tile.GetBitMap();
                        if (bitmap == null)
                            continue;
                        
                        bitmap.Name = LstTileIndexes[i].ToString();//this.TerrainTileFile.ToUpper() + "-" + 

                        LstTilesOV.Add(LstTileIndexes[i], bitmap);
                    }
                    // lstText.Add(ImageProcessing.MakeTexture2D(ref bitmap, pak.Palette));
                    
                }

                yield return null;
                
                MCBitmap atlas;
                MCBitmap atlasOV;

                Dictionary<string, Rect> DictRects;
                Dictionary<string, Rect> DictRectsOV;
                
                MechCommanderUnity.LogMessage("Starting Terrain Atlas Generation after Tiles");
    //            try
    //            {
                    var result = ImageProcessing.CreateAtlas(LstTiles.Values.ToArray(), out atlas, out DictRects);
                    
                    yield return null;
                    
                    this.CacheMap(atlas);
                    
                    DictTileInfo = new Dictionary<string, TileInfo>();
                    foreach (var rect in DictRects)
                    {
                        var bitmap = LstTiles[int.Parse(rect.Key)];
                        DictTileInfo.Add(rect.Key, new TileInfo()
                        {
                            rect = rect.Value,
                            pivot = bitmap.Pivot //new Vector2((float)((double)bitmap.PivotX / bitmap.Width), 1f - (float)((double)bitmap.PivotY / bitmap.Height))
                        });
                    }

                    this.CacheTileInfo(DictTileInfo);
                    
                    if (result)
                    {
                        MainText = ImageProcessing.MakeIndexedTexture2D(atlas);
                        PalText = pak.Palette.ExportPaletteTexture();

                        atlas.Dispose();

                    } else
                    {
                        MechCommanderUnity.LogMessage("Error Packing file " + result);
                    }
                    
                    var resultOv = ImageProcessing.CreateAtlas(LstTilesOV.Values.ToArray(), out atlasOV, out DictRectsOV);
                    
                    yield return null;
                    
                    this.CacheMap(atlasOV,"OV");
                    
                    DictTileOVInfo = new Dictionary<string, TileInfo>();
                    foreach (var rect in DictRectsOV)
                    {
                        var bitmap = LstTilesOV[int.Parse(rect.Key)];
                        DictTileOVInfo.Add(rect.Key, new TileInfo()
                        {
                            rect = rect.Value,
                            pivot = bitmap.Pivot 
                        });
                    }
                    
                    this.CacheTileInfo(DictTileOVInfo, "OV");
                    
                    if (resultOv)
                    {
                        MainOVText = ImageProcessing.MakeIndexedTexture2D(atlasOV);
                        atlasOV.Dispose();
                    } else
                    {
                        MechCommanderUnity.LogMessage("Error Packing file " + resultOv);
                    }
    //            } catch (Exception e)
    //            {
    //                MechCommanderUnity.LogMessage("Error Packing file " + e.Message);
    //                // throw;
    //            }


                foreach (var tile in LstTiles)
                {
                    tile.Value.Dispose();
                }
                
                foreach (var tile in LstTilesOV)
                {
                    tile.Value.Dispose();
                }


                MechCommanderUnity.LogMessage("Finish Init Terrain");
                
                yield return OnFinish(); 
            }

            
        }

        

        private bool IsCached()
        {
            string MapDirPath = Path.Combine("Assets", "Sprites", "Maps");
            Directory.CreateDirectory(MapDirPath);

            string MapFilePath = Path.Combine(MapDirPath, this.terrainFileName);
            
            if (!File.Exists(MapFilePath + ".bytes"))
                return false;
            
            if (!File.Exists(MapFilePath + "OV.bytes"))
                return false;

            if (!File.Exists(MapFilePath + ".map"))
                return false;
            
            if (!File.Exists(MapFilePath + "OV.map"))
                return false;

            var atlas = MCBitmap.Unserialize(MapFilePath + ".bytes");
            MainText = ImageProcessing.MakeIndexedTexture2D(atlas);
            atlas.Dispose();
            
            var atlasOv = MCBitmap.Unserialize(MapFilePath + "OV.bytes");
            MainOVText = ImageProcessing.MakeIndexedTexture2D(atlasOv);
            atlasOv.Dispose();
            
            DictTileInfo = this.GetCacheTileInfo(MapFilePath + ".map");

            DictTileOVInfo = this.GetCacheTileInfo(MapFilePath + "OV.map");

            if (TerrainTileFile.ToUpper() == "Tiles".ToUpper())
            {
                var pal = new MCPalette(MechCommanderUnity.Instance.FileManager.File(palettePath));
                PalText = pal.ExportPaletteTexture();
            }
            else
            {
                var pal = new MCPalette(MechCommanderUnity.Instance.FileManager.File(GpalettePath));
                PalText = pal.ExportPaletteTexture();
            }

            return true;
        }

        private void CacheMap(MCBitmap bitmap, string suffix="")
        {
            string MapDirPath = Path.Combine("Assets", "Sprites", "Maps");
            Directory.CreateDirectory(MapDirPath);

            string MapFilePath = Path.Combine(MapDirPath, this.terrainFileName)+suffix+".bytes";
            
            bitmap.Serialize(MapFilePath);
        }
        
        private void CacheTileInfo(Dictionary<string, TileInfo> dictTileInfo, string suffix="")
        {
            string MapDirPath = Path.Combine("Assets", "Sprites", "Maps");
            Directory.CreateDirectory(MapDirPath);

            string MapFilePath = Path.Combine(MapDirPath, this.terrainFileName)+suffix+".map";
            
            using (StreamWriter writer = new StreamWriter(MapFilePath, true))
            {
                foreach (var tileInfo in dictTileInfo)
                {
                    // get the destination rectangle
                    Rect destination = tileInfo.Value.rect;
                    Vector2 pivot = tileInfo.Value.pivot;


                    // write out the destination rectangle for this bitmap
                    writer.WriteLine(string.Format(
                        "{0};{1};{2};{3};{4};{5};{6}",
                        tileInfo.Key,
                        destination.x,
                        destination.y,
                        destination.width,
                        destination.height, 
                        pivot.x,
                        pivot.y));
                }
            }
        }

        private Dictionary<string, TileInfo> GetCacheTileInfo(string path, string suffix = "")
        {
            var result=new Dictionary<string, TileInfo>();

            using (StreamReader reader = new StreamReader(path))
            {
                while (reader.Peek() >= 0)
                {
                    var str = reader.ReadLine();
                    string[] strArray = str.Split(';');

                    if (strArray.Length < 7)
                    {
                        continue;
                    }

                    string key = strArray[0];

                    Rect rect= new Rect();

                    rect.x = float.Parse(strArray[1]);
                    rect.y = float.Parse(strArray[2]);
                    rect.width = float.Parse(strArray[3]);
                    rect.height = float.Parse(strArray[4]);
                    
                    var pivot=new Vector2(float.Parse(strArray[5]),float.Parse(strArray[6]));

                    var tileInfo = new TileInfo {rect = rect, pivot = pivot};

                    result.Add(key,tileInfo);
                }
            }
            
            return result;
        }
    }
}
