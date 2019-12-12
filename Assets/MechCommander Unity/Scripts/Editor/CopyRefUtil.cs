using UnityEngine;
using UnityEditor;

public static class CopyRefUtil
{
  [MenuItem ("Assets/Copy Ref To Clipboard %&c")]
  public static void GetReferencesString()
  {
    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
    if(path.IndexOf("Resources") > -1)
    {
      path = path.Substring(path.LastIndexOf("Resources/") + 10);
      path = path.Substring(0, path.LastIndexOf("."));
      EditorGUIUtility.systemCopyBuffer = path;
    }else{
      Debug.LogError("[Copy Ref To Clipboard] Not an object in a resources folder.");
    }
  }
}