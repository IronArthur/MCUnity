#region MIT License

/*
 * Copyright (c) 2009-2010 Nick Gravelyn (nick@gravelyn.com), Markus Ewald (cygon@nuclex.org)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 * 
 */

#endregion

using MechCommanderUnity.API;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace sspack
{
    public enum FailCode
    {
        FailedParsingArguments = 1,
        ImageExporter,
        MapExporter,
        NoImages,
        ImageNameCollision,

        FailedToLoadImage,
        FailedToPackImage,
        FailedToCreateImage,
        FailedToSaveImage,
        FailedToSaveMap
    }

    public class ImagePacker
    {
        // various properties of the resulting image
        private bool requirePow2, requireSquare;
        private int padding;
        private int outputWidth, outputHeight;

        // the input list of image files
//        private List<string> files;
        private List<MCBitmap> filesBMP;

        // some dictionaries to hold the image sizes and destination Rects
        private readonly Dictionary<string, MCSize> imageSizes = new Dictionary<string, MCSize>();
        private readonly Dictionary<string, Rect> imagePlacement = new Dictionary<string, Rect>();

        /// <summary>
        /// Packs a collection of images into a single image.
        /// </summary>
        /// <param name="imageFiles">The list of file paths of the images to be combined.</param>
        /// <param name="requirePowerOfTwo">Whether or not the output image must have a power of two size.</param>
        /// <param name="requireSquareImage">Whether or not the output image must be a square.</param>
        /// <param name="maximumWidth">The maximum width of the output image.</param>
        /// <param name="maximumHeight">The maximum height of the output image.</param>
        /// <param name="imagePadding">The amount of blank space to insert in between individual images.</param>
        /// <param name="generateMap">Whether or not to generate the map dictionary.</param>
        /// <param name="outputImage">The resulting output image.</param>
        /// <param name="outputMap">The resulting output map of placement Rects for the images.</param>
        /// <returns>0 if the packing was successful, error code otherwise.</returns>
        //public int PackImage(
        //	IEnumerable<string> imageFiles, 
        //	bool requirePowerOfTwo, 
        //	bool requireSquareImage, 
        //	int maximumWidth,
        //	int maximumHeight,
        //	int imagePadding,
        //	bool generateMap,
        //	out MCBitmap outputImage, 
        //	out Dictionary<string, Rect> outputMap)
        //{
        //	files = new List<string>(imageFiles);
        //	requirePow2 = requirePowerOfTwo;
        //	requireSquare = requireSquareImage;
        //	outputWidth = maximumWidth;
        //	outputHeight = maximumHeight;
        //	padding = imagePadding;

        //	outputImage = null;
        //	outputMap = null;

        //	// make sure our dictionaries are cleared before starting
        //	imageSizes.Clear();
        //	imagePlacement.Clear();

        //	// get the sizes of all the images
        //	foreach (var image in files)
        //	{
        //		MCBitmap MCBitmap = MCBitmap.FromFile(image) as MCBitmap;
        //		if (MCBitmap == null)
        //			return (int)FailCode.FailedToLoadImage;
        //		imageSizes.Add(image, MCBitmap.Size);
        //	}

        //	// sort our files by file size so we place large sprites first
        //	files.Sort(
        //		(f1, f2) =>
        //		{
        //			Size b1 = imageSizes[f1];
        //			Size b2 = imageSizes[f2];

        //			int c = -b1.Width.CompareTo(b2.Width);
        //			if (c != 0)
        //				return c;

        //			c = -b1.Height.CompareTo(b2.Height);
        //			if (c != 0)
        //				return c;

        //			return f1.CompareTo(f2);
        //		});

        //	// try to pack the images
        //	if (!PackImageRects())
        //		return (int)FailCode.FailedToPackImage;

        //	// make our output image
        //	outputImage = CreateOutputImage();
        //	if (outputImage == null)
        //		return (int)FailCode.FailedToSaveImage;

        //	if (generateMap)
        //	{
        //		// go through our image placements and replace the width/height found in there with
        //		// each image's actual width/height (since the ones in imagePlacement will have padding)
        //		string[] keys = new string[imagePlacement.Keys.Count];
        //		imagePlacement.Keys.CopyTo(keys, 0);
        //		foreach (var k in keys)
        //		{
        //			// get the actual size
        //			Size s = imageSizes[k];

        //			// get the placement Rect
        //			Rect r = imagePlacement[k];

        //			// set the proper size
        //			r.Width = s.Width;
        //			r.Height = s.Height;

        //			// insert back into the dictionary
        //			imagePlacement[k] = r;
        //		}

        //		// copy the placement dictionary to the output
        //		outputMap = new Dictionary<string, Rect>();
        //		foreach (var pair in imagePlacement)
        //		{
        //			outputMap.Add(pair.Key, pair.Value);
        //		}
        //	}

        //	// clear our dictionaries just to free up some memory
        //	imageSizes.Clear();
        //	imagePlacement.Clear();

        //	return 0;
        //}

        /// <summary>
        /// Packs a collection of images into a single image.
        /// </summary>
        /// <param name="imageFiles">The list of file paths of the images to be combined.</param>
        /// <param name="requirePowerOfTwo">Whether or not the output image must have a power of two size.</param>
        /// <param name="requireSquareImage">Whether or not the output image must be a square.</param>
        /// <param name="maximumWidth">The maximum width of the output image.</param>
        /// <param name="maximumHeight">The maximum height of the output image.</param>
        /// <param name="imagePadding">The amount of blank space to insert in between individual images.</param>
        /// <param name="generateMap">Whether or not to generate the map dictionary.</param>
        /// <param name="outputImage">The resulting output image.</param>
        /// <param name="outputMap">The resulting output map of placement Rects for the images.</param>
        /// <returns>0 if the packing was successful, error code otherwise.</returns>
        public int PackImage(
            IEnumerable<MCBitmap> imageFiles,
            bool requirePowerOfTwo,
            bool requireSquareImage,
            int maximumWidth,
            int maximumHeight,
            int imagePadding,
            bool orderByImageSize,
            bool generateMap,
            out MCBitmap outputImage,
            out Dictionary<string, Rect> outputMap)
        {
            filesBMP = new List<MCBitmap>(imageFiles);
            requirePow2 = requirePowerOfTwo;
            requireSquare = requireSquareImage;
            outputWidth = maximumWidth;
            outputHeight = maximumHeight;
            padding = imagePadding;

            outputImage = null;
            outputMap = null;

            // make sure our dictionaries are cleared before starting
            imageSizes.Clear();
            imagePlacement.Clear();

            // get the sizes of all the images
            foreach (var image in filesBMP)
            {
                MCBitmap MCBitmap = image;
                if (MCBitmap == null)
                    return (int)FailCode.FailedToLoadImage;
                if (!imageSizes.ContainsKey(image.Name.ToString()))
                    imageSizes.Add(image.Name.ToString(), MCBitmap.Size);
                else
                {
                    imageSizes.Add(image.Name.ToString() + "_", MCBitmap.Size);
                }
            }

            if (orderByImageSize)
            {
                filesBMP.Sort(
                    (f1, f2) =>
                    {
                        var b1 = imageSizes[f1.Name];
                        var b2 = imageSizes[f2.Name];

                        int c = -b1.Width.CompareTo(b2.Width);
                        if (c != 0)
                            return c;

                        c = -b1.Height.CompareTo(b2.Height);
                        if (c != 0)
                            return c;

                        return f1.Name.CompareTo(f2.Name);
                    });
            }
            

//            MechCommanderUnity.MechCommanderUnity.LogMessage("Create Atlas - AfterSizes , Number: " + imageSizes.Count +" orig:"+filesBMP.Count);

            // try to pack the images
            if (!PackImageRects())
                return (int)FailCode.FailedToPackImage;
            
//            MechCommanderUnity.MechCommanderUnity.LogMessage("Create Atlas - AfterPack");

            // make our output image
            outputImage = CreateOutputImage();
            if (outputImage == null)
                return (int)FailCode.FailedToSaveImage;

            if (generateMap)
            {
                // go through our image placements and replace the width/height found in there with
                // each image's actual width/height (since the ones in imagePlacement will have padding)
                string[] keys = new string[imagePlacement.Keys.Count];
                imagePlacement.Keys.CopyTo(keys, 0);
                foreach (var k in keys)
                {
                    // get the actual size
                    MCSize s = imageSizes[k];

                    // get the placement Rect
                    Rect r = imagePlacement[k];

                    // set the proper size
                    r.width = s.Width;
                    r.height = s.Height;

                    // insert back into the dictionary
                    imagePlacement[k] = r;
                }

                // copy the placement dictionary to the output
                outputMap = new Dictionary<string, Rect>();
                foreach (var pair in imagePlacement)
                {
                    outputMap.Add(pair.Key, pair.Value);
                }
            }

            // clear our dictionaries just to free up some memory
            imageSizes.Clear();
            imagePlacement.Clear();

            return 0;
        }

        // This method does some trickery type stuff where we perform the TestPackingImages method over and over, 
        // trying to reduce the image size until we have found the smallest possible image we can fit.
        private bool PackImageRects()
        {
            // create a dictionary for our test image placements
            Dictionary<string, Rect> testImagePlacement = new Dictionary<string, Rect>();

            // get the size of our smallest image
            int smallestWidth = int.MaxValue;
            int smallestHeight = int.MaxValue;
            foreach (var size in imageSizes)
            {
                smallestWidth = Math.Min(smallestWidth, size.Value.Width);
                smallestHeight = Math.Min(smallestHeight, size.Value.Height);
            }

            // we need a couple values for testing
            int testWidth = outputWidth;
            int testHeight = outputHeight;

            bool shrinkVertical = false;

            // just keep looping...
            while (true)
            {
//                MechCommanderUnity.MechCommanderUnity.LogMessage("Create Atlas - InnewWhile : " +testWidth +" "+testHeight);
                // make sure our test dictionary is empty
                testImagePlacement.Clear();

                // try to pack the images into our current test size
                if (!TestPackingImages(testWidth, testHeight, testImagePlacement))
                {
//                    MechCommanderUnity.MechCommanderUnity.LogMessage("TestPackingImages FAILED");
                    // if that failed...

                    // if we have no images in imagePlacement, i.e. we've never succeeded at PackImages,
                    // show an error and return false since there is no way to fit the images into our
                    // maximum size texture
                    if (imagePlacement.Count == 0)
                        return false;

                    // otherwise return true to use our last good results
                    if (shrinkVertical)
                        return true;

                    shrinkVertical = true;
                    testWidth += smallestWidth + padding + padding;
                    testHeight += smallestHeight + padding + padding;
                    continue;
                }

                // clear the imagePlacement dictionary and add our test results in
                imagePlacement.Clear();
                foreach (var pair in testImagePlacement)
                    imagePlacement.Add(pair.Key, pair.Value);

                // figure out the smallest MCBitmap that will hold all the images
                testWidth = testHeight = 0;
                foreach (var pair in imagePlacement)
                {
                    testWidth = Math.Max(testWidth, (int)pair.Value.xMax);
                    testHeight = Math.Max(testHeight, (int)pair.Value.yMax);
                }

                // subtract the extra padding on the right and bottom
                if (!shrinkVertical)
                    testWidth -= padding;
                testHeight -= padding;

                // if we require a power of two texture, find the next power of two that can fit this image
                if (requirePow2)
                {
                    testWidth = MiscHelper.FindNextPowerOfTwo(testWidth);
                    testHeight = MiscHelper.FindNextPowerOfTwo(testHeight);
                }

                // if we require a square texture, set the width and height to the larger of the two
                if (requireSquare)
                {
                    int max = Math.Max(testWidth, testHeight);
                    testWidth = testHeight = max;
                }

                // if the test results are the same as our last output results, we've reached an optimal size,
                // so we can just be done
                if (testWidth == outputWidth && testHeight == outputHeight)
                {
                    if (shrinkVertical)
                        return true;

                    shrinkVertical = true;
                }

                // save the test results as our last known good results
                outputWidth = testWidth;
                outputHeight = testHeight;

                // subtract the smallest image size out for the next test iteration
                if (!shrinkVertical)
                    testWidth -= smallestWidth;
                testHeight -= smallestHeight;
            }
        }

        private bool TestPackingImages(int testWidth, int testHeight, Dictionary<string, Rect> testImagePlacement)
        {
//            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
//            st.Start();
            
            // create the Rect packer
            ArevaloRectanglePacker RectPacker = new ArevaloRectanglePacker(testWidth, testHeight);

            foreach (var image in filesBMP)
            {
                // get the MCBitmap for this file
                MCSize size = imageSizes[image.Name.ToString()];

                // pack the image
                Vector2 origin;
                if (!RectPacker.TryPack(size.Width + padding, size.Height + padding, out origin))
                {
//                    MechCommanderUnity.MechCommanderUnity.LogMessage("FAlsePAck");
                    return false;
                }

                if (!testImagePlacement.ContainsKey(image.Name.ToString()))
                    // add the destination Rect to our dictionary
                    testImagePlacement.Add(image.Name.ToString(), new Rect(origin.x, origin.y, size.Width + padding, size.Height + padding));
                else
                {
                    // add the destination Rect to our dictionary
                    testImagePlacement.Add(image.Name.ToString() + "_", new Rect(origin.x, origin.y, size.Width + padding, size.Height + padding));
                }
            }

//            st.Stop();
            
//            MechCommanderUnity.MechCommanderUnity.LogMessage("TestPacking elapsed: ("+filesBMP.Count+") ->" +(st.ElapsedMilliseconds/1000) +"s");
            return true;
        }

        private MCBitmap CreateOutputImage()
        {
            //try
            //{
            MCBitmap outputImage;
            if (filesBMP[0].Format != MCBitmap.Formats.ARGB)
            {
                outputImage = new MCBitmap(outputWidth, outputHeight);//, PixelFormat.Format8bppIndexed
                                                                      //outputImage.Palette = filesBMP[0].Palette;

                // draw all the images into the output image

                var BoundsRect = new Rect(0, 0, outputWidth, outputHeight);
                var bmpData = outputImage.Data;

                int bytes = outputImage.Stride * outputImage.Height;
                var rgbValues = new byte[bytes];



                foreach (var image in filesBMP)
                {
                    Rect location = imagePlacement[image.Name.ToString()];
                    MCBitmap MCBitmap = image;// MCBitmap.FromFile(image) as MCBitmap;
                    if (MCBitmap == null)
                        return null;

                    var BoundsRectN = new Rect(0, 0, MCBitmap.Width, MCBitmap.Height);

                    for (int x = 0; x < MCBitmap.Width; x++)
                    {
                        for (int y = 0; y < MCBitmap.Height; y++)
                        {
                            rgbValues[(int)(((location.y + y) * outputImage.Stride) + (location.x + x))] = MCBitmap.Data[((y) * MCBitmap.Stride) + (x)];
                        }
                    }

                    // copy pixels over to avoid antialiasing or any other side effects of drawing
                    // the subimages to the output image using Graphics
                    //for (int x = 0; x < MCBitmap.Width; x++)
                    //    for (int y = 0; y < MCBitmap.Height; y++)
                    //        outputImage.SetPixel(location.X + x, location.Y + y, MCBitmap.GetPixel(x, y));
                }

                outputImage.Data = rgbValues;

            } else
            {
                outputImage = new MCBitmap(outputWidth, outputHeight);//, MCBitmap.Formats.ARGB

                // draw all the images into the output image
                foreach (var image in filesBMP)
                {
                    Rect location = imagePlacement[image.Name.ToString()];
                    MCBitmap MCBitmap = image;// MCBitmap.FromFile(image) as MCBitmap;
                    if (MCBitmap == null)
                        return null;

                    // copy pixels over to avoid antialiasing or any other side effects of drawing
                    // the subimages to the output image using Graphics
                    for (int x = 0; x < MCBitmap.Width; x++)
                        for (int y = 0; y < MCBitmap.Height; y++)
                            MCBitmap.SetPixel(outputImage,(int)location.x + x, (int)location.y + y, MCBitmap.GetPixel(MCBitmap,x, y));
                }
            }




            return outputImage;
            //}
            //catch
            //{
            //	return null;
            //}
        }
    }
}