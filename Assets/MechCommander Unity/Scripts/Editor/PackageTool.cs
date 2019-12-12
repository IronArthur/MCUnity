using UnityEngine;
using UnityEditor;

public class PackageTool
{
    [MenuItem("Package/Update Package")]
    static void UpdatePackage()
    {
        AssetDatabase.ExportPackage( new string[] {"Assets/MechCommander Unity/Scripts/Editor", "Assets/Sprites/Mechs" }, "MechUnityEditor.unitypackage", ExportPackageOptions.Recurse);
        Debug.Log("Package Exported");
    }
}
