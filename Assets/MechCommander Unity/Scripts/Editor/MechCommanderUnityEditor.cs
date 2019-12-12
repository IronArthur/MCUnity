using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MechCommanderUnity.Utility;


namespace MechCommanderUnity
{
    [CustomEditor(typeof(MechCommanderUnity))]
    public class MechCommanderUnityEditor : Editor
    {
        private MechCommanderUnity mcgUnity { get { return target as MechCommanderUnity; } }

        private Texture2D cachedPreviewTexture;

        private const string showImportFoldout = "MechCommanderUnity_ShowImportFoldout";
        private static bool ShowImportFoldout
        {
            get { return EditorPrefs.GetBool(showImportFoldout, true); }
            set { EditorPrefs.SetBool(showImportFoldout, value); }
        }

        private const string showOptionsFoldout = "MechCommanderUnity_ShowOptionsFoldout";
        private static bool ShowOptionsFoldout
        {
            get { return EditorPrefs.GetBool(showOptionsFoldout, false); }
            set { EditorPrefs.SetBool(showOptionsFoldout, value); }
        }

        private const string showAssetExportFoldout = "MechCommanderUnity_ShowAssetExportFoldout";
        private static bool ShowAssetExportFoldout
        {
            get { return EditorPrefs.GetBool(showAssetExportFoldout, false); }
            set { EditorPrefs.SetBool(showAssetExportFoldout, value); }
        }
       
        SerializedProperty Prop(string name)
        {
            return serializedObject.FindProperty(name);
        }

        public override void OnInspectorGUI()
        {
            // Update
            serializedObject.Update();

            // Get properties
            var propMCGPath = Prop("MCGRemotePath");

            // Browse for MCG path
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("MCG Path", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.SelectableLabel(mcgUnity.MCGRemotePath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.Space();
            GUILayoutHelper.Horizontal(() =>
            {
                if (GUILayout.Button("Browse..."))
                {
                    string path = EditorUtility.OpenFilePanel("Locate MCG exe file", "", "exe");
                    if (!string.IsNullOrEmpty(path))
                    {
                        string mcgPath = Path.GetDirectoryName(path);
                        if (!mcgUnity.ValidateRemoteMCGPath(mcgPath))
                        {
                            EditorUtility.DisplayDialog("Invalid Game Path", "The selected MCG path is invalid", "Close");
                        }
                        else
                        {
                            mcgUnity.MCGRemotePath = mcgPath;
//                            propMCGPath.stringValue = path;
                            mcgUnity.CopyMCGAssetsToLocal();
                        }
                    }
                }
                if (GUILayout.Button("Clear"))
                {
                    mcgUnity.EditorClearMCGPath();
                    EditorUtility.SetDirty(target);
                }
                if (GUILayout.Button("Refresh Reader"))
                {
                    mcgUnity.EditorResetMCGPath();
                    EditorUtility.SetDirty(target);
                }
            });


            // Prompt user to set MCG path
            if (string.IsNullOrEmpty(mcgUnity.MCGPath))
            {
                EditorGUILayout.HelpBox("Please set the MCG path of your MechCommander Gold installation.", MessageType.Info);
                return;
            }

            // Display other GUI items
            //DisplayAssetExporterGUI();
            //DisplayOptionsGUI();
            DisplayMapsImporterGUI();

            //DisplayImporterGUI();

            // Save modified properties
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                EditorUtility.SetDirty(target);
            
        }

        private void DisplayOptionsGUI()
        {
            EditorGUILayout.Space();
            ShowOptionsFoldout = GUILayoutHelper.Foldout(ShowOptionsFoldout, new GUIContent("Options"), () =>
            {
            });
             
        }

        private void DisplayMapsImporterGUI()
        {
            EditorGUILayout.Space();
            ShowOptionsFoldout = GUILayoutHelper.Foldout(ShowOptionsFoldout, new GUIContent("Maps Importer"), () =>
            {
               GUILayoutHelper.Indent(() =>
               {
                   var propMapIndex = Prop("Map_ModelIndex");
                   var propMapName = Prop("MapName");

                   EditorGUILayout.Space();

                   if (mcgUnity.ListMaps.Count > 0)
                   {
                       GUILayoutHelper.Horizontal(() =>
                       {
                           GUILayout.Label("Map tools", EditorStyles.boldLabel);
                           if (GUILayout.Button("Select Map to Scene Load"))
                           {
                               GenericMenu menu = new GenericMenu();
                               foreach (var map in mcgUnity.ListMaps)
                               {
                                   menu.AddItem(new GUIContent(map), map==propMapName.stringValue, OnSelectMap, map);
                               }

                               menu.ShowAsContext();

                           }
                       });
                       
                       EditorGUILayout.Space();
                       
                       GUILayoutHelper.Horizontal(() =>
                       {
                           Texture2D texture = new Texture2D(1, 1);

                           if (mcgUnity.PreviewHasChanged && MechCommanderUnity.Instance.ContentReader.GetMapPreview(propMapName.stringValue, out texture))
                           {
                               cachedPreviewTexture = texture;
                               mcgUnity.PreviewHasChanged = false;
                              // EditorGUI.ObjectField(GUILayout.Label(texture));
                           }else if (cachedPreviewTexture == null)
                           {
                               cachedPreviewTexture = texture;
                           }
                           EditorGUILayout.Space();
                           GUILayout.Label("Preview Map:", EditorStyles.boldLabel);
                           //EditorGUI.PrefixLabel(new Rect(30, 75, 100, 15), 0, new GUIContent("Preview Map:"));
                           EditorGUI.DrawPreviewTexture(new Rect(120, 95, cachedPreviewTexture.width *.5f, cachedPreviewTexture.height*.5f), cachedPreviewTexture);

                           GUILayout.Label(cachedPreviewTexture);
                                             
                       });
                       
                   }
               });
            });
        }
        private void DisplayImporterGUI()
        {
            // Hide importer GUI when not active in hierarchy
            if (!mcgUnity.gameObject.activeInHierarchy)
                return;

            EditorGUILayout.Space();
            ShowImportFoldout = GUILayoutHelper.Foldout(ShowImportFoldout, new GUIContent("Importer"), () =>
            {
                GUILayoutHelper.Indent(() =>
                {
                    EditorGUILayout.Space();
                   /* var propModelID = Prop("ModelImporter_ModelID");

                    

                    foreach(var obj in GameObjectHelper.TypeOBJ){

                        if (mcgUnity.ListFullObjs==null || mcgUnity.ListFullObjs[obj.Key] == null)
                        {
                            continue;
                        }
                        EditorGUILayout.LabelField(new GUIContent(obj.Value, ""));
                        GUILayoutHelper.Horizontal(() =>
                        {
                            var ListMech = mcgUnity.ListFullObjs[obj.Key];
                           

                            int[] values = new int[ListMech.Keys.Count];
                            ListMech.Keys.CopyTo(values, 0);


                            string[] options = new string[ListMech.Values.Count];
                            ListMech.Values.CopyTo(options, 0);



                            propModelID.intValue = EditorGUILayout.IntPopup( propModelID.intValue, options, values);

                            //propModelID.intValue = EditorGUILayout.IntField(propModelID.intValue);
                            if (GUILayout.Button("Import"))
                            {
                                //  Debug.Log ("Cargando!");

                                //Debug.Log(propModelID.intValue.ToString());
                             //   GameObjectHelper.CreateMechCommanderMeshGameObject(propModelID.intValue, obj.Key , null);
                            }
                        });
                    }*/
                    
                  /*  EditorGUILayout.LabelField(new GUIContent("Mech", ""));
                    GUILayoutHelper.Horizontal(() =>
                    {
                        var ListMech = mcgUnity.ListMechs;
                        if (ListMech == null)
                        {
                            ListMech = GameObjectHelper.GetMechList();
                        }
                        int[] values = new int[ListMech.Keys.Count];
                        ListMech.Keys.CopyTo(values, 0);

                        string[] options = new string[ListMech.Values.Count];
                        ListMech.Values.CopyTo(options, 0);



                        propModelID.intValue = EditorGUILayout.IntPopup("Select Mech: ", propModelID.intValue, options, values);

                        //propModelID.intValue = EditorGUILayout.IntField(propModelID.intValue);
                        if (GUILayout.Button("Import"))
                        {
                          //  Debug.Log ("Cargando!");

                            Debug.Log(propModelID.intValue.ToString());
                            GameObjectHelper.CreateMechCommanderMeshGameObject(propModelID.intValue, null);
                        }
                    });

                    EditorGUILayout.LabelField(new GUIContent("OBJ", ""));
                    GUILayoutHelper.Horizontal(() =>
                    {
                        var ListObj = mcgUnity.ListObjs;
                        if (ListObj == null)
                        {
                            ListObj = GameObjectHelper.GetObjList();
                        }
                        int[] values = new int[ListObj.Keys.Count];
                        ListObj.Keys.CopyTo(values, 0);

                        string[] options = new string[ListObj.Values.Count];
                        ListObj.Values.CopyTo(options, 0);



                        propModelID.intValue = EditorGUILayout.IntPopup("Select Obj: ", propModelID.intValue, options, values);

                        //propModelID.intValue = EditorGUILayout.IntField(propModelID.intValue);
                        if (GUILayout.Button("Import"))
                        {
                            //  Debug.Log ("Cargando!");

                            Debug.Log(propModelID.intValue.ToString());
                            GameObjectHelper.CreateMechCommanderMeshGameObject(propModelID.intValue, null);
                        }
                    });
                    */
                   
                });
            });
        }

        private void DisplayAssetExporterGUI()
        {
            EditorGUILayout.Space();
            ShowAssetExportFoldout = GUILayoutHelper.Foldout(ShowAssetExportFoldout, new GUIContent("Asset Exporter (Beta)"), () =>
            {
               /* EditorGUILayout.HelpBox("Export pre-built assets to specified Resources folder and subfolder.", MessageType.Info);

                // Parent Resources path
                var propMyResourcesFolder = Prop("Option_MyResourcesFolder");
                propMyResourcesFolder.stringValue = EditorGUILayout.TextField(new GUIContent("My Resources Folder", "Path to Resources folder for asset export."), propMyResourcesFolder.stringValue);

                // Terrain atlases
                GUILayoutHelper.Horizontal(() =>
                {
                    var propTerrainAtlasesSubFolder = Prop("Option_TerrainAtlasesSubFolder");
                    propTerrainAtlasesSubFolder.stringValue = EditorGUILayout.TextField(new GUIContent("Terrain Atlas SubFolder", "Sub-folder for terrain atlas textures."), propTerrainAtlasesSubFolder.stringValue);
                    if (GUILayout.Button("Update"))
                    {
                        //#if UNITY_EDITOR && !UNITY_WEBPLAYER
                        //                        mcgUnity.ExportTerrainTextureAtlases();
                        //#endif
                    }
                });
                * */
            });
        }


        private void OnSelectMap(object o)
        {
            string mapname = (string)o;
            mcgUnity.SelectMapToLoad(mapname);
            
            
            
        }
    }
}
