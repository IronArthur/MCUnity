// Decompiled with JetBrains decompiler
// Type: SpritesheetCollection
// Assembly: TexturePackerImporter, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 277155B9-C478-4F5B-A6E7-DD5692272411
// Assembly location: C:\Users\dit gestion\Downloads\SpriteSheetPacker\TexturePackerImporter.dll

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SpritesheetCollection
{
  private Dictionary<string, SpriteMetaData[]> spriteSheetData = new Dictionary<string, SpriteMetaData[]>();
  private Dictionary<string, string> spritesForData = new Dictionary<string, string>();
  private Dictionary<string, string> normalsForData = new Dictionary<string, string>();
  private Dictionary<string, string> dataForTexture = new Dictionary<string, string>();

  public void loadSheetData(string dataFile)
  {
    if (this.spriteSheetData.ContainsKey(dataFile))
      this.unloadSheetData(dataFile);
    string[] strArray1 = File.ReadAllLines(dataFile);
    int num1 = 30302;
    string str1 = (string) null;
    string str2 = (string) null;
    foreach (string str3 in strArray1)
    {
      if (str3.StartsWith(":format="))
        num1 = int.Parse(str3.Remove(0, 8));
      if (str3.StartsWith(":normalmap="))
        str2 = str3.Remove(0, 11);
      if (str3.StartsWith(":texture="))
        str1 = str3.Remove(0, 9);
      if (str3.StartsWith("# Sprite sheet: "))
      {
        string str4 = str3.Remove(0, 16);
        str1 = str4.Remove(str4.LastIndexOf("(") - 1);
      }
    }

      List<SpriteMetaData> list = new List<SpriteMetaData>();
      foreach (string str3 in strArray1)
      {
        if (!string.IsNullOrEmpty(str3) && !str3.StartsWith("#") && !str3.StartsWith(":"))
        {
//          Debug.Log(str3);
          string str4 = str3;
          char[] chArray = new char[1];
          int index = 0;
          int num2 = 59;
          chArray[index] = (char) num2;
          string[] strArray2 = str4.Split(chArray);
          if (strArray2.Length < 7)
          {
            EditorUtility.DisplayDialog("File format error", "Failed to import '" + dataFile + "'", "Ok");
            return;
          }
          SpriteMetaData spriteMetaData = new SpriteMetaData();
          spriteMetaData.name = strArray2[0].Replace("/", "-");
          float num3 = float.Parse(strArray2[1]);
          float num4 = float.Parse(strArray2[2]);
          float num5 = float.Parse(strArray2[3]);
          float num6 = float.Parse(strArray2[4]);
          float num7 = float.Parse(strArray2[5]);
          float num8 = float.Parse(strArray2[6]);
          spriteMetaData.rect =  new Rect(num3, num4, num5, num6);
          spriteMetaData.pivot =  new Vector2(num7, num8);
          spriteMetaData.alignment = (double) num7 != 0.0 || (double) num8 != 0.0 ? ((double) num7 != 0.5 || (double) num8 != 0.0 ? ((double) num7 != 1.0 || (double) num8 != 0.0 ? ((double) num7 != 0.0 || (double) num8 != 0.5 ? ((double) num7 != 0.5 || (double) num8 != 0.5 ? ((double) num7 != 1.0 || (double) num8 != 0.5 ? ((double) num7 != 0.0 || (double) num8 != 1.0 ? ((double) num7 != 0.5 || (double) num8 != 1.0 ? ((double) num7 != 1.0 || (double) num8 != 1.0 ?  9 :  3) :  2) :  1) :  5) :  0) :  4) :  8 ):  7) :  6;
          list.Add(spriteMetaData);
        }
      }
      this.spriteSheetData[dataFile] = list.ToArray();
      string index1 = Path.GetDirectoryName(dataFile).Replace(@"\",@"/") + "/" + str1;
//      Debug.Log(index1 +" "+dataFile);
      this.spritesForData[dataFile] = index1;
      this.dataForTexture[index1] = dataFile;
      if (str2 == null)
        return;
      string index2 = Path.GetDirectoryName(dataFile) + "/" + str2;
      this.normalsForData[dataFile] = index2;
      this.dataForTexture[index2] = dataFile;
    
  }

  public void unloadSheetData(string dataFile)
  {
    if (this.spritesForData.ContainsKey(dataFile))
      this.dataForTexture.Remove(this.spritesForData[dataFile]);
    if (this.normalsForData.ContainsKey(dataFile))
      this.dataForTexture.Remove(this.normalsForData[dataFile]);
    this.spriteSheetData.Remove(dataFile);
    this.spritesForData.Remove(dataFile);
    this.normalsForData.Remove(dataFile);
  }

  public string spriteFileForNormalsFile(string normalsFile)
  {
    return this.spritesForData[this.dataForTexture[normalsFile]];
  }

  public string spriteFileForDataFile(string dataFile)
  {
    return this.spritesForData[dataFile];
  }

  public string normalsFileForDataFile(string dataFile)
  {
    if (this.normalsForData.ContainsKey(dataFile))
      return this.normalsForData[dataFile];
    return (string) null;
  }

  public SpriteMetaData[] spriteMetaDataForSpriteFile(string textureFile)
  {
    return this.spriteSheetData[this.dataForTexture[textureFile]];
  }

  public bool isSpriteSheet(string textureFile)
  {
    return this.spritesForData.ContainsValue(textureFile);
  }

  public bool isNormalmapSheet(string textureFile)
  {
    return this.normalsForData.ContainsValue(textureFile);
  }
}
