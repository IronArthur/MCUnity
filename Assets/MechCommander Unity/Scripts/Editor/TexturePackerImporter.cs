
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TexturePackerImporter : AssetPostprocessor
{

    private static SpritesheetCollection spriteSheets = new SpritesheetCollection();


    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {

        List<string> list1 = new List<string>((IEnumerable<string>)deletedAssets);
        list1.AddRange((IEnumerable<string>)movedFromAssetPaths);
        foreach (string str in list1)
        {
            if (Path.GetExtension(str).Equals(".sprites"))
                TexturePackerImporter.spriteSheets.unloadSheetData(str);
        }
        List<string> list2 = new List<string>((IEnumerable<string>)importedAssets);
        list2.AddRange((IEnumerable<string>)movedAssets);
        foreach (string str1 in list2)
        {
            if (Path.GetExtension(str1).Equals(".sprites"))
            {
                TexturePackerImporter.spriteSheets.loadSheetData(str1);
//                Debug.Log(TexturePackerImporter.spriteSheets.spriteFileForDataFile(str1));
                AssetDatabase.ImportAsset(TexturePackerImporter.spriteSheets.spriteFileForDataFile(str1), ImportAssetOptions.ForceUpdate);
                string str2 = TexturePackerImporter.spriteSheets.normalsFileForDataFile(str1);
                if (str2 != null)
                    AssetDatabase.ImportAsset(str2, ImportAssetOptions.ForceUpdate);
            }
        }
    }

    private void OnPreprocessTexture()
    {
        TextureImporter importer = assetImporter as TextureImporter;
        if (TexturePackerImporter.spriteSheets.isSpriteSheet(assetPath))
        {
//            Debug.Log("Sprite "+TexturePackerImporter.spriteSheets.spriteMetaDataForSpriteFile(assetPath));
            TexturePackerImporter.updateSpriteMetaData(importer,
                TexturePackerImporter.spriteSheets.spriteMetaDataForSpriteFile(assetPath));
        }
//        else
//            Debug.Log("NO sprite?¿? " +assetPath);
        
        if (!TexturePackerImporter.spriteSheets.isNormalmapSheet(assetPath))
            return;
        TexturePackerImporter.prepareNormalmapForImport(importer);
    }

    private void OnPostprocessTexture(Texture2D texture)
    {
        if (!TexturePackerImporter.spriteSheets.isNormalmapSheet(assetPath))
            return;
        TexturePackerImporter.createMaterialForNormalmap(assetPath, TexturePackerImporter.spriteSheets.spriteFileForNormalsFile(assetPath));
    }

    private static void updateSpriteMetaData(TextureImporter importer, SpriteMetaData[] metaData)
    {
//        Debug.Log("updateSpriteMetadata");
       // if (importer.textureType != TextureImporterType.Default)
        importer.textureType = (TextureImporterType.Sprite);
        importer.maxTextureSize = (8192);
        importer.spriteImportMode = (SpriteImportMode.Multiple);
        importer.filterMode = FilterMode.Point;
        Dictionary<string, SpriteMetaData> dictionary = new Dictionary<string, SpriteMetaData>();
        foreach (SpriteMetaData spriteMetaData in importer.spritesheet)
            dictionary.Add((string)spriteMetaData.name, spriteMetaData);
        for (int index = 0; index < metaData.Length; ++index)
        {
            string key = (string)metaData[index].name;
            if (dictionary.ContainsKey(key))
            {
                SpriteMetaData spriteMetaData = dictionary[key];

                metaData[index].pivot = spriteMetaData.pivot;
                metaData[index].alignment = spriteMetaData.alignment;

                metaData[index].border = spriteMetaData.border;
            }
        }
        importer.spritesheet = (metaData);
    }

    private static void prepareNormalmapForImport(TextureImporter importer)
    {
        if (importer.textureType == TextureImporterType.Default)
            return;
        importer.textureType = (TextureImporterType.NormalMap);
    }

    private static void createMaterialForNormalmap(string normalSheet, string spriteSheet)
    {
        string path = Path.ChangeExtension(spriteSheet, ".mat");
        if (File.Exists(path))
            return;
        Texture2D texture2D1 = AssetDatabase.LoadAssetAtPath(spriteSheet, typeof(Texture2D)) as Texture2D;
        Texture2D texture2D2 = AssetDatabase.LoadAssetAtPath(normalSheet, typeof(Texture2D)) as Texture2D;
        bool flag = true;
        Shader shader = Shader.Find("Standard");
        if (shader == null)
        {
            shader = Shader.Find("Transparent/Bumped Diffuse");
            flag = false;
        }
        Material material = new Material(shader);
        material.SetTexture("_MainTex", (Texture)texture2D1);
        material.SetTexture("_BumpMap", (Texture)texture2D2);
        if (flag)
        {
            material.SetFloat("_Mode", 2f);
            material.SetInt("_SrcBlend", 5);
            material.SetInt("_DstBlend", 10);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (3000);
        }
        AssetDatabase.CreateAsset((Object)material, path);
        EditorUtility.SetDirty((Object)material);
    }
}
