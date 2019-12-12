// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    https://github.com/Interkarma/daggerfall-unity
using UnityEngine;
using System.Collections;
using System.IO;
using MechCommanderUnity.API;
using System.Collections.Generic;
using sspack;

namespace MechCommanderUnity.Utility
{

    /// <summary>
    /// Basic image processing for MCBitmap, Color32, and FntFile formats.
    /// </summary>
    public static class ImageProcessing
    {
        #region Helpers

        // Clamps value to range
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        // Inserts a Color32 array into XY position of another Color32 array
        public static void InsertColors(ref Color32[] src, ref Color32[] dst, int xPos, int yPos, int srcWidth, int srcHeight, int dstWidth, int dstHeight)
        {
            for (int y = 0; y < srcHeight; y++)
            {
                for (int x = 0; x < srcWidth; x++)
                {
                    Color32 col = src[y * srcWidth + x];
                    dst[(yPos + y) * dstWidth + (xPos + x)] = col;
                }
            }
        }

        /// <summary>
        /// Converts full colour MCBitmap to Color32 array.
        /// </summary>
        /// <param name="bitmap">Source MCBitmap.</param>
        /// <param name="flipY">Flip Y rows so bottom becomes top.</param>
        /// <returns>Color32 array.</returns>
        public static Color32[] ConvertToColor32(MCBitmap bitmap, bool flipY = true)
        {
            // Must not be indexed
            if (bitmap.Format == MCBitmap.Formats.Indexed)
                return null;

            // Create destination array
            Color32[] colors = new Color32[bitmap.Width * bitmap.Height];

            int index = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                int rowOffset = (flipY) ? (bitmap.Height - 1 - y) * bitmap.Stride : y * bitmap.Stride;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // Get color bytes
                    int pos = rowOffset + x * bitmap.FormatWidth;
                    byte r = bitmap.Data[pos++];
                    byte g = bitmap.Data[pos++];
                    byte b = bitmap.Data[pos++];
                    byte a = bitmap.Data[pos];

                    // Store in array
                    colors[index++] = new Color32(r, g, b, a);
                }
            }

            return colors;
        }

        // Helper to create and apply Texture2D from single call
        public static Texture2D MakeTexture2D(Color32[] src, int width, int height, TextureFormat textureFormat, bool mipMaps)
        {

            Texture2D texture = new Texture2D(width, height, textureFormat, mipMaps);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels32(src);
            texture.Apply(mipMaps);

            return texture;
        }

        // Helper to create and apply Texture2D from single call
        public static Texture2D MakeTexture2D(MCBitmap src, MCPalette myPalette)
        {
            Texture2D texture = new Texture2D(src.Width, src.Height, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels32(GetColors32(src, 0, 0, myPalette));
            //texture.Compress(false);
            texture.Apply(false);

            return texture;
        }

        public static Texture2D MakeIndexedTexture2D(MCBitmap src)
        {
            Texture2D texture = new Texture2D(src.Width, src.Height, TextureFormat.Alpha8, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.alphaIsTransparency = true;
            texture.LoadRawTextureData(src.RawData);
            texture.Apply(false);

            return texture;
        }

        public static Texture2D MakeTexture2D(ref MCBitmap src)
        {
            Texture2D texture = new Texture2D(src.Width, src.Height, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels32(GetColors32(src, 0, 0));
            //texture.Compress(false);
            texture.Apply(false);

            return texture;
        }

        public static Texture2D MakeAtlas(Texture2D[] atlasTextures, out Rect[] rects, string name = "atlas")
        {
            Texture2D atlas = new Texture2D(1, 1);
            rects = atlas.PackTextures(atlasTextures, 0, 2048, false);
            atlas.name = name;
            return atlas;
        }

        // Helper to create and apply Sprite from single call
        public static Sprite MakeSprite(Color32[] src, int width, int height, TextureFormat textureFormat, bool mipMaps)
        {
            var text = MakeTexture2D(src, width, height, textureFormat, mipMaps);

            return Sprite.Create(text, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }

        // Helper to create and apply Sprite from single call
        public static Sprite MakeSprite(MCBitmap src)
        {
            var text = MakeTexture2D(GetColors32(src, 0, 0), src.Width, src.Height, TextureFormat.ARGB32, true);

            var x = 0.5f;
            var y = 0.5f;
            if (src.PivotX > 0 && src.PivotY > 0)
            {
                x = (float)((float)src.PivotX / (float)src.Width);
                y = (float)((float)src.PivotY / (float)src.Height);

            }
            return Sprite.Create(text, new Rect(0, 0, src.Width, src.Height), new Vector2(x, y));
        }

        // Helper to create and apply Sprite from single call
        public static Sprite MakeSprite(Texture2D src)
        {

            var x = 0.5f;
            var y = 0.5f;

            return Sprite.Create(src, new Rect(0, 0, src.width, src.height), new Vector2(x, y));
        }

        // Helper to create and apply Sprite from single call
        public static List<Sprite> MakeSprite(Texture2D src, Rect[] rects, Vector2[] pivots = null)
        {

            var x = 0.5f;
            var y = 0.5f;
            var pivot = new Vector2(x, y);


            List<Sprite> lstSprites = new List<Sprite>();

            for (int i = 0; i < rects.Length; i++)
            {
                if (src == null || rects[i] == null)
                    continue;

                if (pivots[i] != null)
                {
                    lstSprites.Add(
                                        Sprite.Create(src,
                                        new Rect(rects[i].xMin * src.width,
                                            rects[i].yMin * src.height,
                                            rects[i].width * src.width,
                                            rects[i].height * src.height),
                                        pivots[i]));
                } else
                {
                    lstSprites.Add(
                                        Sprite.Create(src,
                                        new Rect(rects[i].xMin * src.width,
                                            rects[i].yMin * src.height,
                                            rects[i].width * src.width,
                                            rects[i].height * src.height),
                                        pivot));
                }

                lstSprites[i].name = i.ToString();
            }

            return lstSprites;
        }

        // Helper to create and apply Sprite from single call
        public static List<Sprite> MakeSprite(Texture2D src, Rect[] rects, Vector2 pivot)
        {

            List<Sprite> lstSprites = new List<Sprite>();

            for (int i = 0; i < rects.Length; i++)
            {
                if (src == null || rects[i] == null)
                    continue;


                lstSprites.Add(
                    Sprite.Create(src,
                    new Rect(rects[i].xMin * src.width,
                        rects[i].yMin * src.height,
                        rects[i].width * src.width,
                        rects[i].height * src.height),
                    pivot));
                lstSprites[i].name = i.ToString();
            }

            return lstSprites;
        }

        // Helper to create and apply Sprite from single call
        public static Sprite MakeSprite(MCBitmap src, MCPalette myPalette)
        {
            var text = MakeTexture2D(GetColors32(src, 0, 0, myPalette), src.Width, src.Height, TextureFormat.ARGB32, true);


            var x = 0.5f;
            var y = 0.5f;

            //MechCommanderUnity.LogMessage("P: "+src.PivotX.ToString() + " "+ src.PivotY.ToString() );


            if (src.PivotX > 0 && src.PivotY > 0)
            {
                x = (float)(1 - ((float)src.PivotX / (float)src.Width));
                y = (float)(1 - ((float)src.PivotY / (float)src.Height));

            }
            return Sprite.Create(text, new Rect(0, 0, src.Width, src.Height), new Vector2(x, y));
        }

        // Helper to save Texture2D to a PNG file
        public static void SaveTextureAsPng(Texture2D texture, string path, Dictionary<string, Rect> rects = null, List<Vector2> pivots = null)
        {
            // Path must end with PNG
            if (!path.EndsWith(".png", System.StringComparison.InvariantCultureIgnoreCase))
                path += ".png";

            // Encode and save file
            byte[] buffer = texture.EncodeToPNG();
            File.WriteAllBytes(path, buffer);

            if (rects != null && pivots != null)
            {
                var mappath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)) +".sprites";
                using (StreamWriter writer = new StreamWriter(mappath, false))
                {
                    writer.WriteLine(string.Format(@"# Sprite sheet: {0} ({1} x {2})",
                        Path.GetFileName(path),texture.width,texture.height));

                    var i = 0;
                    foreach (var item in rects)
                    {
                        var rect = item.Value;
                        var pivot = pivots[i];

                        writer.WriteLine(string.Format(
                        "{0};{1};{2};{3};{4};{5};{6}",
                       item.Key,
                        rect.x,
                        rect.y,
                        rect.width,
                        rect.height, pivot.x, pivot.y));
                        i++;
                    }

                    //for (int i = 0; i < rects.Values.Count; i++)
                    //{
                    //    var rect = rects[i.ToString()];
                    //    var pivot = pivots[i];

                    //    writer.WriteLine(string.Format(
                    //    "{0};{1};{2};{3};{4};{5};{6}",
                    //   i.ToString(),
                    //    rect.x,
                    //    rect.y,
                    //    rect.width,
                    //    rect.height, pivot.x, pivot.y ));
                    //}
                }
            }
        }

        // Helper to save Texture2D to a PNG file
        public static void SaveSpriteAsPng(Sprite sprite, string path)
        {
            //var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

            //var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
            //                                        (int)sprite.textureRect.y,
            //                                        (int)sprite.textureRect.width,
            //                                        (int)sprite.textureRect.height);


            //croppedTexture.SetPixels(pixels);
            //croppedTexture.Apply();
            Texture2D texture;
            if (sprite.rect.width != sprite.texture.width)
            {
                Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                             (int)sprite.textureRect.y,
                                                             (int)sprite.textureRect.width,
                                                             (int)sprite.textureRect.height);
                newText.SetPixels(newColors);
                newText.Apply();
                texture = newText;
            } else
                texture = sprite.texture;

            // Path must end with PNG
            if (!path.EndsWith(".png", System.StringComparison.InvariantCultureIgnoreCase))
                path += ".png";

            // Encode and save file
            byte[] buffer = texture.EncodeToPNG();
            File.WriteAllBytes(path, buffer);
        }
        /// <summary>
        /// Gets a Color32 array for engine with a border.
        /// </summary>
        /// <param name="srcBitmap">Source DFBitmap.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <param name="border">Number of pixels border to add around image.</param>
        /// <param name="sizeOut">Receives image dimensions with borders included.</param>
        /// <returns>Color32 array.</returns>
        public static Color32[] GetColors32(MCBitmap srcBitmap, int alphaIndex, int border)
        {
            // Calculate dimensions
            int srcWidth = srcBitmap.Width;
            int srcHeight = srcBitmap.Height;
            int dstWidth = srcWidth + border * 2;
            int dstHeight = srcHeight + border * 2;

            // Create target array
            Color32[] colors = new Color32[dstWidth * dstHeight];
            byte a;
            int index, srcRow, dstRow;
            for (int y = 0; y < srcHeight; y++)
            {
                // Get row position
                srcRow = y * srcWidth;
                dstRow = (dstHeight - 1 - border - y) * dstWidth;

                // Write data for this row
                for (int x = 0; x < srcWidth; x++)
                {
                    index = srcBitmap.Data[srcRow + x];

                    if (alphaIndex == index) a = 0x00; else a = 0xff;

                    colors[dstRow + border + x] = new Color32(srcBitmap.Data[srcRow + x], srcBitmap.Data[srcRow + x + 1], srcBitmap.Data[srcRow + x + 2], a);
                }
            }

            return colors;
        }

        /// <summary>
        /// Gets a Color32 array for engine with a border.
        /// </summary>
        /// <param name="srcBitmap">Source DFBitmap.</param>
        /// <param name="alphaIndex">Index to receive transparent alpha.</param>
        /// <param name="border">Number of pixels border to add around image.</param>
        /// <param name="palette">Palette</param>
        /// <returns>Color32 array.</returns>
        public static Color32[] GetColors32(MCBitmap srcBitmap, int alphaIndex, int border, MCPalette palette)
        {
            // Calculate dimensions
            int srcWidth = srcBitmap.Width;
            int srcHeight = srcBitmap.Height;
            int dstWidth = srcWidth + border * 2;
            int dstHeight = srcHeight + border * 2;

            // Create target array
            Color32[] colors = new Color32[dstWidth * dstHeight];

            MCPalette pal = palette;

            MCColor c;
            byte a;
            int index, srcRow, dstRow;
            for (int y = 0; y < srcHeight; y++)
            {
                // Get row position
                srcRow = y * srcWidth;
                dstRow = (dstHeight - 1 - border - y) * dstWidth;

                // Write data for this row
                for (int x = 0; x < srcWidth; x++)
                {
                    index = srcBitmap.Data[srcRow + x];


                    if (alphaIndex == index)
                    {
                        colors[dstRow + border + x] = new Color32(0, 0, 0, 0x00);
                    } else
                    {
                        c = pal.Get(index, true);
                        if (c.A != 0xff)
                        {
                            a = c.A;
                        } else
                        {
                            a = 0xff;
                        }
                        colors[dstRow + border + x] = new Color32(c.R, c.G, c.B, a);
                    }



                }
            }

            return colors;
        }


        public static byte[] GetBytes(MCBitmap srcBitmap, int alphaIndex, int border, MCPalette palette)
        {
            // Calculate dimensions
            int srcWidth = srcBitmap.Width;
            int srcHeight = srcBitmap.Height;
            int dstWidth = srcWidth + border * 2;
            int dstHeight = srcHeight + border * 2;

            // Create target array
            byte[] colors = new byte[dstWidth * dstHeight * 4];

            MCPalette pal = palette;

            MCColor c;
            byte a;
            int index, srcRow, dstRow;
            for (int y = 0; y < srcHeight; y++)
            {
                // Get row position
                srcRow = y * srcWidth;
                dstRow = (dstHeight - 1 - border - y) * dstWidth * 4;

                // Write data for this row
                for (int x = 0; x < srcWidth; x++)
                {
                    index = srcBitmap.Data[srcRow + x];


                    if (alphaIndex == index)
                    {
                        colors[dstRow + border + x] = 0;
                        colors[dstRow + border + x + 1] = 0;
                        colors[dstRow + border + x + 2] = 0;
                        colors[dstRow + border + x + 3] = 0;

                    } else
                    {
                        c = pal.Get(index, true);
                        if (c.A != 0xff)
                        {
                            a = c.A;
                        } else
                        {
                            a = 0xff;
                        }
                        colors[dstRow + border + x] = c.R;
                        colors[dstRow + border + x + 1] = c.G;
                        colors[dstRow + border + x + 2] = c.B;
                        colors[dstRow + border + x + 3] = a;
                    }



                }
            }

            return colors;
        }


        #endregion

        #region Atlas

        public static bool CreateAtlas(MCBitmap[] lstBmps, out MCBitmap atlas, out Dictionary<string, Rect> outputMap)
        {
            if (lstBmps.Length == 0)
            {
                atlas = new MCBitmap();
                outputMap = new Dictionary<string, Rect>();
                return false;
            }
            ImagePacker imagePacker = new ImagePacker();
            try
            {
//                MechCommanderUnity.LogMessage("Create Atlas");
                int result = imagePacker.PackImage(lstBmps, true, false, 8096, 8096, 0, 
                    false,true, out atlas, out outputMap);
//                MechCommanderUnity.LogMessage("Finish Atlas");
                Dictionary<string, Rect> newMap = new Dictionary<string, Rect>();
                foreach (var map in outputMap)
                {
                    newMap.Add(map.Key, ImageProcessing.TransformRectToUnityFormat(map.Value, atlas.Height));
                }
                outputMap = newMap;

                //MechCommanderUnity.LogMessage("Pack result: " + result);
                return (result == 0);
            } catch (System.Exception e)
            {
                Debug.LogError("Problem with packer!! " + e.Message);
                throw;
            }

        }

        public static Rect TransformRectToUnityFormat(Rect rect, int Height)
        {
            rect.y = Height - (rect.height + rect.y);
            return rect;
        }

        #endregion

        #region Scaling

        public static MCBitmap ResizeBitmap(MCBitmap bitmap, Rect rect)
        {
            // Get start size
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Create new bitmap
            MCBitmap newBitmap = new MCBitmap();
            newBitmap.Width = (int)rect.width;
            newBitmap.Height = (int)rect.height;
            newBitmap.Format = bitmap.Format;
            newBitmap.Stride = newBitmap.Width * bitmap.FormatWidth;
            newBitmap.Data = new byte[newBitmap.Stride * newBitmap.Height];



            for (int i = 0; i < newBitmap.Height; i++)
            {
                var x = (bitmap.PivotX + (int)rect.xMin) + i;
                for (int j = 0; j < newBitmap.Width; j++)
                {
                    var y = (bitmap.PivotY + (int)rect.yMin) + j;

                    //MechCommanderUnity.LogMessage(" Color> x: " + x + " y: " + y+ " >> x: " + i+ "y: " + j);
                    //var color=MCBitmap.GetPixel(bitmap,y , x );

                    //MCBitmap.SetPixel(newBitmap,j,i, color);

                    newBitmap.Data[(j * newBitmap.Stride) + i] = bitmap.Data[(y * bitmap.Stride) + x];

                }
            }

            newBitmap.PivotX = (int)rect.xMin * -1;
            newBitmap.PivotY = (int)rect.yMin * -1;


            return newBitmap;
        }
        /// <summary>
        /// Resize MCBitmap with a bicubic kernel.
        /// </summary>
        /// <param name="bitmap">MCBitmap source.</param>
        /// <param name="level">MipMap level.</param>
        /// <returns>SnapBitmap mipmap.</returns>
        public static MCBitmap ResizeBicubic(MCBitmap bitmap, int newWidth, int newHeight)
        {
            // Must be a colour format
            if (bitmap.Format == MCBitmap.Formats.Indexed)
                return null;

            // Get start size
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Create new bitmap
            MCBitmap newBitmap = new MCBitmap();
            newBitmap.Width = newWidth;
            newBitmap.Height = newHeight;
            newBitmap.Format = bitmap.Format;
            newBitmap.Stride = newBitmap.Width * bitmap.FormatWidth;
            newBitmap.Data = new byte[newBitmap.Stride * newBitmap.Height];

            // Scale factors
            double xFactor = (double)width / newWidth;
            double yFactor = (double)height / newHeight;

            // Coordinates of source points and coefficients
            double ox, oy, dx, dy, k1, k2;
            int ox1, oy1, ox2, oy2;

            // Destination pixel values
            double r, g, b, a;

            // Width and height decreased by 1
            int ymax = height - 1;
            int xmax = width - 1;

            // Bicubic resize
            for (int y = 0; y < newHeight; y++)
            {
                // Y coordinates
                oy = (double)y * yFactor - 0.5f;
                oy1 = (int)oy;
                dy = oy - (double)oy1;

                for (int x = 0; x < newWidth; x++)
                {
                    // X coordinates
                    ox = (double)x * xFactor - 0.5f;
                    ox1 = (int)ox;
                    dx = ox - (double)ox1;

                    // Initial pixel value
                    r = g = b = a = 0;

                    for (int n = -1; n < 3; n++)
                    {
                        // Get Y coefficient
                        k1 = BiCubicKernel(dy - (double)n);

                        oy2 = oy1 + n;
                        if (oy2 < 0)
                            oy2 = 0;
                        if (oy2 > ymax)
                            oy2 = ymax;

                        for (int m = -1; m < 3; m++)
                        {
                            // Get X coefficient
                            k2 = k1 * BiCubicKernel((double)m - dx);

                            ox2 = ox1 + m;
                            if (ox2 < 0)
                                ox2 = 0;
                            if (ox2 > xmax)
                                ox2 = xmax;

                            // Get pixel of original image
                            int srcPos = (ox2 * bitmap.FormatWidth) + (bitmap.Stride * oy2);
                            r += k2 * bitmap.Data[srcPos++];
                            g += k2 * bitmap.Data[srcPos++];
                            b += k2 * bitmap.Data[srcPos++];
                            a += k2 * bitmap.Data[srcPos];
                        }
                    }

                    // Set destination pixel
                    int dstPos = (x * newBitmap.FormatWidth) + (newBitmap.Stride * y);
                    newBitmap.Data[dstPos++] = (byte)r;
                    newBitmap.Data[dstPos++] = (byte)g;
                    newBitmap.Data[dstPos++] = (byte)b;
                    newBitmap.Data[dstPos] = (byte)a;
                }
            }

            return newBitmap;
        }

        /// <summary>
        /// BiCubic calculations.
        /// </summary>
        /// <param name="x">Value.</param>
        /// <returns>Double.</returns>
        private static double BiCubicKernel(double x)
        {
            if (x > 2.0)
                return 0.0;

            double a, b, c, d;
            double xm1 = x - 1.0;
            double xp1 = x + 1.0;
            double xp2 = x + 2.0;

            a = (xp2 <= 0.0) ? 0.0 : xp2 * xp2 * xp2;
            b = (xp1 <= 0.0) ? 0.0 : xp1 * xp1 * xp1;
            c = (x <= 0.0) ? 0.0 : x * x * x;
            d = (xm1 <= 0.0) ? 0.0 : xm1 * xm1 * xm1;

            return (0.16666666666666666667 * (a - (4.0 * b) + (6.0 * c) - (4.0 * d)));
        }

        #endregion

        #region Grayscale

        /// <summary>
        /// Creates a grayscale version of bitmap based on colour theory.
        /// </summary>
        /// <param name="bitmap">Source MCBitmap.</param>
        /// <returns>Grayscale MCBitmap image.</returns>
        public static MCBitmap MakeGrayscale(MCBitmap bitmap)
        {
            // Must be a colour format
            if (bitmap.Format == MCBitmap.Formats.Indexed)
                return null;

            MCBitmap newBitmap = new MCBitmap();
            newBitmap.Format = bitmap.Format;
            newBitmap.Initialise(bitmap.Width, bitmap.Height);

            // Make each pixel grayscale
            int srcPos = 0, dstPos = 0;
            for (int i = 0; i < bitmap.Width * bitmap.Height; i++)
            {
                // Get source color
                byte r = bitmap.Data[srcPos++];
                byte g = bitmap.Data[srcPos++];
                byte b = bitmap.Data[srcPos++];
                byte a = bitmap.Data[srcPos++];

                // Create grayscale color
                int grayscale = (int)(r * 0.3f + g * 0.59f + b * 0.11f);
                MCColor dstColor = MCColor.FromRGBA((byte)grayscale, (byte)grayscale, (byte)grayscale, a);

                // Write destination pixel
                newBitmap.Data[dstPos++] = dstColor.R;
                newBitmap.Data[dstPos++] = dstColor.G;
                newBitmap.Data[dstPos++] = dstColor.B;
                newBitmap.Data[dstPos++] = dstColor.A;
            }

            return newBitmap;
        }

        /// <summary>
        /// Creates average intensity bitmap.
        /// </summary>
        /// <param name="bitmap">Source MCBitmap.</param>
        /// <returns>Average intensity MCBitmap.</returns>
        public static MCBitmap MakeAverageIntensityBitmap(MCBitmap bitmap)
        {
            // Must be a colour format
            if (bitmap.Format == MCBitmap.Formats.Indexed)
                return null;

            MCBitmap newBitmap = new MCBitmap();
            newBitmap.Format = bitmap.Format;
            newBitmap.Initialise(bitmap.Width, bitmap.Height);

            // Make each pixel grayscale based on average intensity
            int srcPos = 0, dstPos = 0;
            for (int i = 0; i < bitmap.Width * bitmap.Height; i++)
            {
                // Get source color
                byte r = bitmap.Data[srcPos++];
                byte g = bitmap.Data[srcPos++];
                byte b = bitmap.Data[srcPos++];
                byte a = bitmap.Data[srcPos++];

                // Get average intensity
                int intensity = (r + g + b) / 3;
                MCColor dstColor = MCColor.FromRGBA((byte)intensity, (byte)intensity, (byte)intensity, a);

                // Write destination pixel
                newBitmap.Data[dstPos++] = dstColor.R;
                newBitmap.Data[dstPos++] = dstColor.G;
                newBitmap.Data[dstPos++] = dstColor.B;
                newBitmap.Data[dstPos++] = dstColor.A;
            }

            return newBitmap;
        }

        #endregion

        #region PreMultiply Alpha

        /// <summary>
        /// Pre-multiply bitmap alpha.
        /// </summary>
        /// <param name="bitmap">MCBitmap.</param>
        public static void PreMultiplyAlpha(MCBitmap bitmap)
        {
            // Must be a colour format
            if (bitmap.Format == MCBitmap.Formats.Indexed)
                return;

            // Pre-multiply alpha for each pixel
            int pos;
            float multiplier;
            for (int y = 0; y < bitmap.Height; y++)
            {
                pos = y * bitmap.Stride;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    multiplier = bitmap.Data[pos + 3] / 256f;
                    bitmap.Data[pos] = (byte)(bitmap.Data[pos] * multiplier);
                    bitmap.Data[pos + 1] = (byte)(bitmap.Data[pos + 1] * multiplier);
                    bitmap.Data[pos + 2] = (byte)(bitmap.Data[pos + 2] * multiplier);
                    pos += bitmap.FormatWidth;
                }
            }
        }

        #endregion

        #region Rotate & Flip

        // Rotates a Color32 array 90 degrees counter-clockwise
        public static Color32[] RotateColors(ref Color32[] src, int width, int height)
        {
            Color32[] dst = new Color32[src.Length];

            // Rotate image data
            int srcPos = 0;
            for (int x = width - 1; x >= 0; x--)
            {
                for (int y = 0; y < height; y++)
                {
                    Color32 col = src[srcPos++];
                    dst[y * width + x] = col;
                }
            }

            return dst;
        }

        // Flips a Color32 array horizontally and vertically
        public static Color32[] FlipColors(ref Color32[] src, int width, int height)
        {
            Color32[] dst = new Color32[src.Length];

            // Flip image data
            int srcPos = 0;
            int dstPos = dst.Length - 1;
            for (int i = 0; i < width * height; i++)
            {
                Color32 col = src[srcPos++];
                dst[dstPos--] = col;
            }

            return dst;
        }

        #endregion

        #region Dilate

        /// <summary>
        /// Creates a blended border around transparent textures.
        /// Removes dark edges from billboards.
        /// </summary>
        /// <param name="colors">Source image.</param>
        /// <param name="size">Image size.</param>
        public static void DilateColors(ref Color32[] colors, MCSize size)
        {
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    Color32 color = ReadColor(ref colors, ref size, x, y);
                    if (color.a != 0)
                    {
                        MixColor(ref colors, ref size, color, x - 1, y - 1);
                        MixColor(ref colors, ref size, color, x, y - 1);
                        MixColor(ref colors, ref size, color, x + 1, y - 1);
                        MixColor(ref colors, ref size, color, x - 1, y);
                        MixColor(ref colors, ref size, color, x + 1, y);
                        MixColor(ref colors, ref size, color, x - 1, y + 1);
                        MixColor(ref colors, ref size, color, x, y + 1);
                        MixColor(ref colors, ref size, color, x + 1, y + 1);
                    }
                }
            }
        }

        #endregion

        #region Wrap & Clamp

        /// <summary>
        /// Wraps texture into emtpty border area on opposite side.
        /// </summary>
        /// <param name="colors">Source image.</param>
        /// <param name="size">Image size.</param>
        /// <param name="border">Border width.</param>
        public static void WrapBorder(ref Color32[] colors, MCSize size, int border, bool leftRight = true, bool topBottom = true)
        {
            // Wrap left-right
            if (leftRight)
            {
                for (int y = border; y < size.Height - border; y++)
                {
                    int ypos = y * size.Width;
                    int il = ypos + border;
                    int ir = ypos + size.Width - border * 2;
                    for (int x = 0; x < border; x++)
                    {
                        colors[ypos + x] = colors[ir + x];
                        colors[ypos + size.Width - border + x] = colors[il + x];
                    }
                }
            }

            // Wrap top-bottom
            if (topBottom)
            {
                for (int y = 0; y < border; y++)
                {
                    int ypos1 = y * size.Width;
                    int ypos2 = (y + size.Height - border) * size.Width;

                    int it = (border + y) * size.Width;
                    int ib = (y + size.Height - border * 2) * size.Width;
                    for (int x = 0; x < size.Width; x++)
                    {
                        colors[ypos1 + x] = colors[ib + x];
                        colors[ypos2 + x] = colors[it + x];
                    }
                }
            }
        }

        /// <summary>
        /// Clamps texture into empty border area.
        /// </summary>
        /// <param name="colors">Source image.</param>
        /// <param name="size">Image size.</param>
        /// <param name="border">Border width.</param>
        public static void ClampBorder(
            ref Color32[] colors,
            MCSize size,
            int border,
            bool leftRight = true,
            bool topBottom = true,
            bool topLeft = true,
            bool topRight = true,
            bool bottomLeft = true,
            bool bottomRight = true)
        {
            if (leftRight)
            {
                // Clamp left-right
                for (int y = border; y < size.Height - border; y++)
                {
                    int ypos = y * size.Width;
                    Color32 leftColor = colors[ypos + border];
                    Color32 rightColor = colors[ypos + size.Width - border - 1];

                    int il = ypos;
                    int ir = ypos + size.Width - border;
                    for (int x = 0; x < border; x++)
                    {
                        colors[il + x] = leftColor;
                        colors[ir + x] = rightColor;
                    }
                }
            }

            if (topBottom)
            {
                // Clamp top-bottom
                for (int x = border; x < size.Width - border; x++)
                {
                    Color32 topColor = colors[((border) * size.Width) + x];
                    Color32 bottomColor = colors[(size.Height - border - 1) * size.Width + x];

                    for (int y = 0; y < border; y++)
                    {
                        int it = y * size.Width + x;
                        int ib = (size.Height - y - 1) * size.Width + x;

                        colors[it] = topColor;
                        colors[ib] = bottomColor;
                    }
                }
            }

            if (topLeft)
            {
                // Clamp top-left
                Color32 topLeftColor = colors[(border + 1) * size.Width + border];
                for (int y = 0; y < border; y++)
                {
                    for (int x = 0; x < border; x++)
                    {
                        colors[y * size.Width + x] = topLeftColor;
                    }
                }
            }

            if (topRight)
            {
                // Clamp top-right
                Color32 topRightColor = colors[(border + 1) * size.Width + size.Width - border - 1];
                for (int y = 0; y < border; y++)
                {
                    for (int x = size.Width - border; x < size.Width; x++)
                    {
                        colors[y * size.Width + x] = topRightColor;
                    }
                }
            }

            if (bottomLeft)
            {
                // Clamp bottom-left
                Color32 bottomLeftColor = colors[(size.Height - border - 1) * size.Width + border];
                for (int y = size.Height - border; y < size.Height; y++)
                {
                    for (int x = 0; x < border; x++)
                    {
                        colors[y * size.Width + x] = bottomLeftColor;
                    }
                }
            }

            if (bottomRight)
            {
                // Clamp bottom-right
                Color32 bottomRightColor = colors[(size.Height - border - 1) * size.Width + size.Width - border - 1];
                for (int y = size.Height - border; y < size.Height; y++)
                {
                    for (int x = size.Width - border; x < size.Width; x++)
                    {
                        colors[y * size.Width + x] = bottomRightColor;
                    }
                }
            }
        }

        #endregion


        #region Edge Detection

        /*
         * Based on Craig's Utility Library (CUL) by James Craig.
         * http://www.gutgames.com/post/Edge-detection-in-C.aspx
         * MIT License (http://www.opensource.org/licenses/mit-license.php)
         */

        /// <summary>
        /// Gets a new bitmap containing edges detected in source bitmap.
        /// The source bitmap is unchanged.
        /// </summary>
        /// <param name="bitmap">MCBitmap source.</param>
        /// <param name="threshold">Edge detection threshold.</param>
        /// <param name="edgeColor">Edge colour to write.</param>
        /// <returns>MCBitmap containing edges.</returns>
        private static MCBitmap FindEdges(MCBitmap bitmap, float threshold, MCColor edgeColor)
        {
            // Must be a colour format
            if (bitmap.Format == MCBitmap.Formats.Indexed)
                return null;

            // Clone bitmap settings
            MCBitmap newBitmap = MCBitmap.CloneMCBitmap(bitmap);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    MCColor currentColor = MCBitmap.GetPixel(bitmap, x, y);
                    if (y < newBitmap.Height - 1 && x < newBitmap.Width - 1)
                    {
                        MCColor tempColor = MCBitmap.GetPixel(bitmap, x + 1, y + 1);
                        if (ColorDistance(currentColor, tempColor) > threshold)
                            MCBitmap.SetPixel(newBitmap, x, y, edgeColor);
                    } else if (y < newBitmap.Height - 1)
                    {
                        MCColor tempColor = MCBitmap.GetPixel(bitmap, x, y + 1);
                        if (ColorDistance(currentColor, tempColor) > threshold)
                            MCBitmap.SetPixel(newBitmap, x, y, edgeColor);
                    } else if (x < newBitmap.Width - 1)
                    {
                        MCColor tempColor = MCBitmap.GetPixel(bitmap, x + 1, y);
                        if (ColorDistance(currentColor, tempColor) > threshold)
                            MCBitmap.SetPixel(newBitmap, x, y, edgeColor);
                    }
                }
            }

            return newBitmap;
        }

        /// <summary>
        /// Gets distance between two colours using Euclidean distance function.
        /// Distance = SQRT( (R1-R2)2 + (G1-G2)2 + (B1-B2)2 )
        /// </summary>
        /// <param name="color1">First Color.</param>
        /// <param name="color2">Second Color.</param>
        /// <returns>Distance between colours.</returns>
        private static float ColorDistance(MCColor color1, MCColor color2)
        {
            float r = color1.R - color2.R;
            float g = color1.G - color2.G;
            float b = color1.B - color2.B;

            return (float)Mathf.Sqrt((r * r) + (g * g) + (b * b));
        }

        #endregion

        #region Matrix Convolution Filter

        /*
         * Based on Craig's Utility Library (CUL) by James Craig.
         * http://www.gutgames.com/post/Matrix-Convolution-Filters-in-C.aspx
         * MIT License (http://www.opensource.org/licenses/mit-license.php)
         */

        /// <summary>
        /// Used when applying convolution filters to an image.
        /// </summary>
        public class Filter
        {
            #region Constructors
            /// <summary>
            /// Constructor
            /// </summary>
            public Filter()
            {
                MyFilter = new int[3, 3];
                Width = 3;
                Height = 3;
                Offset = 0;
                Absolute = false;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="Width">Width</param>
            /// <param name="Height">Height</param>
            public Filter(int Width, int Height)
            {
                MyFilter = new int[Width, Height];
                this.Width = Width;
                this.Height = Height;
                Offset = 0;
                Absolute = false;
            }
            #endregion

            #region Public Properties
            /// <summary>
            /// The actual filter array
            /// </summary>
            public int[,] MyFilter { get; set; }

            /// <summary>
            /// Width of the filter box
            /// </summary>
            public int Width { get; set; }

            /// <summary>
            /// Height of the filter box
            /// </summary>
            public int Height { get; set; }

            /// <summary>
            /// Amount to add to the red, blue, and green values
            /// </summary>
            public int Offset { get; set; }

            /// <summary>
            /// Determines if we should take the absolute value prior to clamping
            /// </summary>
            public bool Absolute { get; set; }
            #endregion

            #region Public Methods
            /// <summary>
            /// Applies the filter to the input image
            /// </summary>
            /// <param name="Input">input image</param>
            /// <returns>Returns a separate image with the filter applied</returns>
            public MCBitmap ApplyFilter(MCBitmap Input)
            {
                // Must be a colour format
                if (Input.Format == MCBitmap.Formats.Indexed)
                    return null;

                MCBitmap NewBitmap = MCBitmap.CloneMCBitmap(Input);
                for (int x = 0; x < Input.Width; ++x)
                {
                    for (int y = 0; y < Input.Height; ++y)
                    {
                        int RValue = 0;
                        int GValue = 0;
                        int BValue = 0;
                        int AValue = 0;
                        int Weight = 0;
                        int XCurrent = -Width / 2;
                        for (int x2 = 0; x2 < Width; ++x2)
                        {
                            if (XCurrent + x < Input.Width && XCurrent + x >= 0)
                            {
                                int YCurrent = -Height / 2;
                                for (int y2 = 0; y2 < Height; ++y2)
                                {
                                    if (YCurrent + y < Input.Height && YCurrent + y >= 0)
                                    {
                                        MCColor Pixel = MCBitmap.GetPixel(Input, XCurrent + x, YCurrent + y);
                                        RValue += MyFilter[x2, y2] * Pixel.R;
                                        GValue += MyFilter[x2, y2] * Pixel.G;
                                        BValue += MyFilter[x2, y2] * Pixel.B;
                                        AValue = Pixel.A;
                                        Weight += MyFilter[x2, y2];
                                    }
                                    ++YCurrent;
                                }
                            }
                            ++XCurrent;
                        }

                        MCColor MeanPixel = MCBitmap.GetPixel(Input, x, y);
                        if (Weight == 0)
                            Weight = 1;
                        if (Weight > 0)
                        {
                            if (Absolute)
                            {
                                RValue = System.Math.Abs(RValue);
                                GValue = System.Math.Abs(GValue);
                                BValue = System.Math.Abs(BValue);
                            }

                            RValue = (RValue / Weight) + Offset;
                            RValue = Clamp(RValue, 0, 255);
                            GValue = (GValue / Weight) + Offset;
                            GValue = Clamp(GValue, 0, 255);
                            BValue = (BValue / Weight) + Offset;
                            BValue = Clamp(BValue, 0, 255);
                            MeanPixel = MCColor.FromRGBA((byte)RValue, (byte)GValue, (byte)BValue, (byte)AValue);
                        }

                        MCBitmap.SetPixel(NewBitmap, x, y, MeanPixel);
                    }
                }

                return NewBitmap;
            }
            #endregion
        }

        #endregion

        #region Sharpening

        /// <summary>
        /// Sharpen a MCBitmap.
        /// </summary>
        /// <param name="bitmap">Source MCBitmap.</param>
        /// <param name="passes">Number of sharpen passes.</param>
        /// <returns>Sharpened MCBitmap.</returns>
        public static MCBitmap Sharpen(MCBitmap bitmap, int passes = 1)
        {
            // Must be a colour format
            if (bitmap.Format == MCBitmap.Formats.Indexed)
                return null;

            MCBitmap newBitmap = MCBitmap.CloneMCBitmap(bitmap);

            for (int i = 0; i < passes; i++)
            {
                // Create horizontal sobel matrix
                Filter sharpenMatrix = new Filter(3, 3);
                sharpenMatrix.MyFilter[0, 0] = -1;
                sharpenMatrix.MyFilter[1, 0] = -1;
                sharpenMatrix.MyFilter[2, 0] = -1;
                sharpenMatrix.MyFilter[0, 1] = 1;
                sharpenMatrix.MyFilter[1, 1] = 12;
                sharpenMatrix.MyFilter[2, 1] = 1;
                sharpenMatrix.MyFilter[0, 2] = -1;
                sharpenMatrix.MyFilter[1, 2] = -1;
                sharpenMatrix.MyFilter[2, 2] = -1;
                newBitmap = sharpenMatrix.ApplyFilter(newBitmap);
            }

            return newBitmap;
        }

        #endregion

        #region Bump Map

        /// <summary>
        /// Gets a bump map from the source MCBitmap.
        /// </summary>
        /// <param name="bitmap">Source colour image.</param>
        /// <returns>MCBitmap bump image.</returns>
        public static MCBitmap GetBumpMap(MCBitmap bitmap)
        {
            // Must be a colour format
            if (bitmap.Format == MCBitmap.Formats.Indexed)
                return null;

            // Create horizontal sobel matrix
            Filter horizontalMatrix = new Filter(3, 3);
            horizontalMatrix.MyFilter[0, 0] = -1;
            horizontalMatrix.MyFilter[1, 0] = 0;
            horizontalMatrix.MyFilter[2, 0] = 1;
            horizontalMatrix.MyFilter[0, 1] = -2;
            horizontalMatrix.MyFilter[1, 1] = 0;
            horizontalMatrix.MyFilter[2, 1] = 2;
            horizontalMatrix.MyFilter[0, 2] = -1;
            horizontalMatrix.MyFilter[1, 2] = 0;
            horizontalMatrix.MyFilter[2, 2] = 1;

            // Create vertical sobel matrix
            Filter verticalMatrix = new Filter(3, 3);
            verticalMatrix.MyFilter[0, 0] = 1;
            verticalMatrix.MyFilter[1, 0] = 2;
            verticalMatrix.MyFilter[2, 0] = 1;
            verticalMatrix.MyFilter[0, 1] = 0;
            verticalMatrix.MyFilter[1, 1] = 0;
            verticalMatrix.MyFilter[2, 1] = 0;
            verticalMatrix.MyFilter[0, 2] = -1;
            verticalMatrix.MyFilter[1, 2] = -2;
            verticalMatrix.MyFilter[2, 2] = -1;

            // Get filtered images
            MCBitmap horz = MakeAverageIntensityBitmap(horizontalMatrix.ApplyFilter(bitmap));
            MCBitmap vert = MakeAverageIntensityBitmap(verticalMatrix.ApplyFilter(bitmap));

            // Create target bitmap
            MCBitmap result = new MCBitmap();
            result.Format = bitmap.Format;
            result.Initialise(horz.Width, horz.Height);

            // Merge
            int pos = 0;
            for (int i = 0; i < bitmap.Width * bitmap.Height; i++)
            {
                // Merge average intensity
                int r = (horz.Data[pos + 0] + vert.Data[pos + 0]) / 2;
                int g = (horz.Data[pos + 1] + vert.Data[pos + 1]) / 2;
                int b = (horz.Data[pos + 2] + vert.Data[pos + 2]) / 2;

                // Write destination pixel
                result.Data[pos + 0] = (byte)((r > 255) ? 255 : r);
                result.Data[pos + 1] = (byte)((g > 255) ? 255 : g);
                result.Data[pos + 2] = (byte)((b > 255) ? 255 : b);
                result.Data[pos + 3] = 255;

                pos += bitmap.FormatWidth;
            }

            return result;
        }

        #endregion

        #region Normal Map

        /// <summary>
        /// Converts a bump map to a normal map.
        /// </summary>
        /// <param name="bitmap">Source bump map image.</param>
        /// <param name="strength">Normal strength.</param>
        /// <returns>MCBitmap normal image.</returns>
        public static MCBitmap ConvertBumpToNormals(MCBitmap bitmap, float strength = 1)
        {
            // Must be a colour format
            if (bitmap.Format == MCBitmap.Formats.Indexed)
                return null;

            MCBitmap newBitmap = new MCBitmap(bitmap.Width, bitmap.Height);
            newBitmap.Format = bitmap.Format;
            newBitmap.Initialise(bitmap.Width, bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    // Look up the heights to either side of this pixel
                    float left = GetIntensity(bitmap, x - 1, y);
                    float right = GetIntensity(bitmap, x + 1, y);
                    float top = GetIntensity(bitmap, x, y - 1);
                    float bottom = GetIntensity(bitmap, x, y + 1);

                    // Compute gradient vectors, then cross them to get the normal
                    Vector3 dx = new Vector3(1, 0, (right - left) * strength);
                    Vector3 dy = new Vector3(0, 1, (bottom - top) * strength);
                    Vector3 normal = Vector3.Cross(dx, dy);
                    normal.Normalize();

                    // Store result
                    int pos = y * bitmap.Stride + x * bitmap.FormatWidth;
                    newBitmap.Data[pos + 0] = (byte)((normal.x + 1.0f) * 127.5f);
                    newBitmap.Data[pos + 1] = (byte)((normal.y + 1.0f) * 127.5f);
                    newBitmap.Data[pos + 2] = (byte)((normal.z + 1.0f) * 127.5f);
                    newBitmap.Data[pos + 3] = 0xff;
                }
            }

            return newBitmap;
        }

        #endregion

        #region Fonts

        //// Gets a glyph from FntFile as Color32 array
        //public static Color32[] GetGlyphColors(FntFile fntFile, int index, Color backColor, Color textColor, out Rect sizeOut)
        //{
        //    // Get actual glyph rect
        //    sizeOut = new Rect(0, 0, fntFile.GetGlyphWidth(index), fntFile.FixedHeight);

        //    // Get glyph byte data as color array
        //    byte[] data = fntFile.GetGlyphPixels(index);
        //    Color32[] colors = new Color32[data.Length];
        //    for (int y = 0; y < FntFile.GlyphFixedDimension; y++)
        //    {
        //        for (int x = 0; x < FntFile.GlyphFixedDimension; x++)
        //        {
        //            int pos = y * FntFile.GlyphFixedDimension + x;
        //            if (data[pos] > 0)
        //                colors[pos] = textColor;
        //            else
        //                colors[pos] = backColor;
        //        }
        //    }

        //    return colors;
        //}

        //// Creates a font atlas from FntFile
        //public static void CreateFontAtlas(FntFile fntFile, Color backColor, Color textColor, out Texture2D atlasTextureOut, out Rect[] atlasRectsOut)
        //{
        //    const int atlasDim = 256;

        //    // Create atlas colors array
        //    Color32[] atlasColors = new Color32[atlasDim * atlasDim];

        //    // Add Daggerfall glyphs
        //    int xpos = 0, ypos = 0;
        //    Rect[] rects = new Rect[FntFile.MaxGlyphCount];
        //    for (int i = 0; i < FntFile.MaxGlyphCount; i++)
        //    {
        //        // Get glyph colors
        //        Rect rect;
        //        Color32[] glyphColors = GetGlyphColors(fntFile, i, backColor, textColor, out rect);

        //        // Offset pixel rect
        //        rect.x += xpos;
        //        rect.y += ypos;

        //        // Flip pixel rect top-bottom
        //        float top = rect.yMin;
        //        float bottom = rect.yMax;
        //        rect.yMin = bottom;
        //        rect.yMax = top;

        //        // Convert to UV coords and store
        //        rect.xMin /= atlasDim;
        //        rect.yMin /= atlasDim;
        //        rect.xMax /= atlasDim;
        //        rect.yMax /= atlasDim;
        //        rects[i] = rect;

        //        // Insert into atlas
        //        InsertColors(
        //            ref glyphColors,
        //            ref atlasColors,
        //            xpos,
        //            ypos,
        //            FntFile.GlyphFixedDimension,
        //            FntFile.GlyphFixedDimension,
        //            atlasDim,
        //            atlasDim);

        //        // Offset position
        //        xpos += FntFile.GlyphFixedDimension;
        //        if (xpos >= atlasDim)
        //        {
        //            xpos = 0;
        //            ypos += FntFile.GlyphFixedDimension;
        //        }
        //    }

        //    // Create texture from colors array
        //    atlasTextureOut = MakeTexture2D(ref atlasColors, atlasDim, atlasDim, TextureFormat.ARGB32, false);
        //    atlasRectsOut = rects;
        //}

        #endregion

        #region Private Methods

        private static void MixColor(ref Color32[] colors, ref MCSize size, Color32 src, int x, int y)
        {
            // Handle outside of bounds
            if (x < 0 || y < 0 || x > size.Width - 1 || y > size.Height - 1)
                return;

            // Get destination pixel colour and ensure it has empty alpha
            Color32 dst = ReadColor(ref colors, ref size, x, y);
            if (dst.a != 0)
                return;

            // Get count for averaging
            int count = 1;
            if (dst != Color.clear)
                count = 2;

            // Mix source colour with destination
            Vector3 avg = new Vector3(
                src.r + dst.r,
                src.g + dst.g,
                src.b + dst.b) / count;

            // Assign new colour to destination
            colors[y * size.Width + x] = new Color32((byte)avg.x, (byte)avg.y, (byte)avg.z, 0);
        }

        private static Color32 ReadColor(ref Color32[] colors, ref MCSize size, int x, int y)
        {
            // Handle outside of bounds
            if (x < 0 || y < 0 || x > size.Width - 1 || y > size.Height - 1)
                return Color.clear;

            return colors[y * size.Width + x];
        }

        private static float GetIntensity(MCBitmap bitmap, int x, int y)
        {
            // Clamp X
            if (x < 0)
                x = 0;
            else if (x >= bitmap.Width)
                x = bitmap.Width - 1;

            // Clamp Y
            if (y < 0)
                y = 0;
            else if (y >= bitmap.Height)
                y = bitmap.Height - 1;

            // Get position
            int pos = y * bitmap.Stride + x * bitmap.FormatWidth;

            // Average intensity
            float intensity = ((bitmap.Data[pos + 0] + bitmap.Data[pos + 1] + bitmap.Data[pos + 2]) / 3) / 255f;

            return intensity;
        }

        #endregion


        /// <summary>
        /// Creates a blended border around transparent textures.
        /// Removes dark edges from billboards.
        /// </summary>
        /// <param name="colors">Source image.</param>
        /// <param name="size">Image size.</param>
        public static void DilateColors_OLD(ref Color32[] colors, MCSize size)
        {
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    Color32 color = ReadColor(ref colors, ref size, x, y);
                    if (color.a != 0)
                    {
                        MixColor(ref colors, ref size, color, x - 1, y - 1);
                        MixColor(ref colors, ref size, color, x, y - 1);
                        MixColor(ref colors, ref size, color, x + 1, y - 1);
                        MixColor(ref colors, ref size, color, x - 1, y);
                        MixColor(ref colors, ref size, color, x + 1, y);
                        MixColor(ref colors, ref size, color, x - 1, y + 1);
                        MixColor(ref colors, ref size, color, x, y + 1);
                        MixColor(ref colors, ref size, color, x + 1, y + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Wraps texture into emtpty border area on opposite side.
        /// </summary>
        /// <param name="colors">Source image.</param>
        /// <param name="size">Image size.</param>
        /// <param name="border">Border width.</param>
        public static void WrapBorder_OLD(ref Color32[] colors, MCSize size, int border, bool leftRight = true, bool topBottom = true)
        {
            // Wrap left-right
            if (leftRight)
            {
                for (int y = border; y < size.Height - border; y++)
                {
                    int ypos = y * size.Width;
                    int il = ypos + border;
                    int ir = ypos + size.Width - border * 2;
                    for (int x = 0; x < border; x++)
                    {
                        colors[ypos + x] = colors[ir + x];
                        colors[ypos + size.Width - border + x] = colors[il + x];
                    }
                }
            }

            // Wrap top-bottom
            if (topBottom)
            {
                for (int y = 0; y < border; y++)
                {
                    int ypos1 = y * size.Width;
                    int ypos2 = (y + size.Height - border) * size.Width;

                    int it = (border + y) * size.Width;
                    int ib = (y + size.Height - border * 2) * size.Width;
                    for (int x = 0; x < size.Width; x++)
                    {
                        colors[ypos1 + x] = colors[ib + x];
                        colors[ypos2 + x] = colors[it + x];
                    }
                }
            }
        }

        /// <summary>
        /// Clamps texture into empty border area.
        /// </summary>
        /// <param name="colors">Source image.</param>
        /// <param name="size">Image size.</param>
        /// <param name="border">Border width.</param>
        public static void ClampBorder_OLD(
            ref Color32[] colors,
            MCSize size,
            int border,
            bool leftRight = true,
            bool topBottom = true,
            bool topLeft = true,
            bool topRight = true,
            bool bottomLeft = true,
            bool bottomRight = true)
        {
            if (leftRight)
            {
                // Clamp left-right
                for (int y = border; y < size.Height - border; y++)
                {
                    int ypos = y * size.Width;
                    Color32 leftColor = colors[ypos + border];
                    Color32 rightColor = colors[ypos + size.Width - border - 1];

                    int il = ypos;
                    int ir = ypos + size.Width - border;
                    for (int x = 0; x < border; x++)
                    {
                        colors[il + x] = leftColor;
                        colors[ir + x] = rightColor;
                    }
                }
            }

            if (topBottom)
            {
                // Clamp top-bottom
                for (int x = border; x < size.Width - border; x++)
                {
                    Color32 topColor = colors[((border) * size.Width) + x];
                    Color32 bottomColor = colors[(size.Height - border - 1) * size.Width + x];

                    for (int y = 0; y < border; y++)
                    {
                        int it = y * size.Width + x;
                        int ib = (size.Height - y - 1) * size.Width + x;

                        colors[it] = topColor;
                        colors[ib] = bottomColor;
                    }
                }
            }

            if (topLeft)
            {
                // Clamp top-left
                Color32 topLeftColor = colors[(border + 1) * size.Width + border];
                for (int y = 0; y < border; y++)
                {
                    for (int x = 0; x < border; x++)
                    {
                        colors[y * size.Width + x] = topLeftColor;
                    }
                }
            }

            if (topRight)
            {
                // Clamp top-right
                Color32 topRightColor = colors[(border + 1) * size.Width + size.Width - border - 1];
                for (int y = 0; y < border; y++)
                {
                    for (int x = size.Width - border; x < size.Width; x++)
                    {
                        colors[y * size.Width + x] = topRightColor;
                    }
                }
            }

            if (bottomLeft)
            {
                // Clamp bottom-left
                Color32 bottomLeftColor = colors[(size.Height - border - 1) * size.Width + border];
                for (int y = size.Height - border; y < size.Height; y++)
                {
                    for (int x = 0; x < border; x++)
                    {
                        colors[y * size.Width + x] = bottomLeftColor;
                    }
                }
            }

            if (bottomRight)
            {
                // Clamp bottom-right
                Color32 bottomRightColor = colors[(size.Height - border - 1) * size.Width + size.Width - border - 1];
                for (int y = size.Height - border; y < size.Height; y++)
                {
                    for (int x = size.Width - border; x < size.Width; x++)
                    {
                        colors[y * size.Width + x] = bottomRightColor;
                    }
                }
            }
        }

        // Rotates a Color32 array 90 degrees counter-clockwise
        public static Color32[] RotateColors_OLD(ref Color32[] src, int width, int height)
        {
            Color32[] dst = new Color32[src.Length];

            // Rotate image data
            int srcPos = 0;
            for (int x = width - 1; x >= 0; x--)
            {
                for (int y = 0; y < height; y++)
                {
                    Color32 col = src[srcPos++];
                    dst[y * width + x] = col;
                }
            }

            return dst;
        }

        // Flips a Color32 array horizontally and vertically
        public static Color32[] FlipColors_OLD(ref Color32[] src, int width, int height)
        {
            Color32[] dst = new Color32[src.Length];

            // Flip image data
            int srcPos = 0;
            int dstPos = dst.Length - 1;
            for (int i = 0; i < width * height; i++)
            {
                Color32 col = src[srcPos++];
                dst[dstPos--] = col;
            }

            return dst;
        }

        // Inserts a Color32 array into XY position of another Color32 array
        public static void InsertColors_OLD(ref Color32[] src, ref Color32[] dst, int xPos, int yPos, int srcWidth, int srcHeight, int dstWidth, int dstHeight)
        {
            for (int y = 0; y < srcHeight; y++)
            {
                for (int x = 0; x < srcWidth; x++)
                {
                    Color32 col = src[y * srcWidth + x];
                    dst[(yPos + y) * dstWidth + (xPos + x)] = col;
                }
            }
        }

        // Helper to create and apply Texture2D from single call
        public static Texture2D MakeTexture2D_OLD(ref Color32[] src, int width, int height, TextureFormat textureFormat, bool mipMaps)
        {
            Texture2D texture = new Texture2D(width, height, textureFormat, mipMaps);
            texture.SetPixels32(src);
            texture.Apply(true);

            return texture;
        }





    }
}