using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MechCommanderUnity
{
    class MechAnimClipTool
    {

        static int tCount;
        static int counter;
        static int totalCount;
        static int progressUpdateInterval = 10000;


        #region Menu Items

        [MenuItem("Assets/MCUnity/Create Mech AnimController")]
        public static void CreateAnimController()
        {
            var selectedFiles = Selection.objects;

            foreach (var file in selectedFiles)
            {
                var filepath = AssetDatabase.GetAssetPath(file);
                if (Path.GetExtension(filepath).Equals(".AnimParts"))
                {
                    LoadGesturesToController(filepath);
                }
            }
        }

        [MenuItem("Assets/MCUnity/Create Mech Clips from SpriteSheets")]
        public static void CreateAnimClip()
        {
            var selectedFiles = Selection.objects;

            foreach (var file in selectedFiles)
            {
                var filepath = AssetDatabase.GetAssetPath(file);
                if (Path.GetExtension(filepath).Equals(".AnimParts"))
                {
                    LoadGesturesData(filepath);


                    //AssetDatabase.ImportAsset(TexturePackerImporter.spriteSheets.spriteFileForDataFile(str1), (ImportAssetOptions)1);
                    //string str2 = TexturePackerImporter.spriteSheets.normalsFileForDataFile(str1);
                    //if (str2 != null)
                    //    AssetDatabase.ImportAsset(str2, (ImportAssetOptions)1);
                }
            }

        }
        #endregion

        #region Menu Item Validation
        [MenuItem("Assets/MCUnity/Create Mech Clips from SpriteSheets", true)]
        [MenuItem("Assets/MCUnity/Create Mech AnimController", true)]
        static bool ValidateSelect()
        {
            if (!IsSelectionValidAnimPartFile())
            {
                return false;
            }

            //if (Selection.objects.Length > 1)
            //{
            //    return false;
            //}

            return true;
        }

        static bool IsSelectionValidAnimPartFile()
        {
            if (Selection.activeObject == null)
            {
                return false;
            }

            return Path.GetExtension(AssetDatabase.GetAssetPath(Selection.activeObject)) == ".AnimParts";
        }


        #endregion


        private static void LoadGesturesToController(string dataFile, bool overwrite = false)
        {
            string[] strArray1 = File.ReadAllLines(dataFile);

            var mechName = Path.GetFileNameWithoutExtension(dataFile);

            Dictionary<string, List<BlendTree>> LstGestures = new Dictionary<string, List<BlendTree>>();

            AnimatorController controller = new AnimatorController();
            if (!File.Exists("Assets/Sprites/Mechs/" + mechName + ".controller") || overwrite)
            {
                controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Sprites/Mechs/" + mechName + ".controller");
                UnityEditor.Animations.AnimatorControllerLayer[] layersA = controller.layers;
                //layersA[0].name = "Torso";
                //layersA[0].defaultWeight = 1;


                var layer = layersA[0];// controller.layers[0];// new AnimatorControllerLayer();
               // controller.RemoveLayer(0);

              //  layer.blendingMode = AnimatorLayerBlendingMode.Additive;
                layer.defaultWeight = 1;
                layer.name = "Torso";

                layer.stateMachine = new AnimatorStateMachine();
                layer.stateMachine.name = "Torso";

                controller.layers = layersA;

                layer = new AnimatorControllerLayer();
                layer.blendingMode = AnimatorLayerBlendingMode.Additive;
                layer.defaultWeight = 1;
                layer.name = "Legs";
                layer.syncedLayerIndex = 0;
                layer.stateMachine = new AnimatorStateMachine();
                layer.stateMachine.name = "Legs";
                controller.AddLayer(layer);

                layer = new AnimatorControllerLayer();
                layer.blendingMode = AnimatorLayerBlendingMode.Additive;
                layer.defaultWeight = 1;
                layer.name = "Larm";
                layer.syncedLayerIndex = 0;
                layer.stateMachine = new AnimatorStateMachine();
                layer.stateMachine.name = "Larm";
                controller.AddLayer(layer);

                layer = new AnimatorControllerLayer();
                layer.blendingMode = AnimatorLayerBlendingMode.Additive;
                layer.defaultWeight = 1;
                layer.name = "Rarm";
                layer.syncedLayerIndex = 0;
                layer.stateMachine = new AnimatorStateMachine();
                layer.stateMachine.name = "Rarm";


                


                controller.AddLayer(layer);

                controller.AddParameter("Orientation", AnimatorControllerParameterType.Float);
                controller.AddParameter("Movement", AnimatorControllerParameterType.Int);

                controller.AddParameter("TorsoMirror", AnimatorControllerParameterType.Bool);
                controller.AddParameter("LegsMirror", AnimatorControllerParameterType.Bool);
                controller.AddParameter("ArmsMirror", AnimatorControllerParameterType.Bool);


            } else
            {
                controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Sprites/Mechs/" + mechName + ".controller");

                var layeridx = 0;
                foreach (var layer in controller.layers)
                {

                    foreach (var item in layer.stateMachine.states)
                    {
                        if (((BlendTree)item.state.motion).children.Where(z => z.motion == null).Count() > 0)
                        {
                            Debug.Log("Removing Missing Motions");

                            for (int z = 0; z < ((BlendTree)item.state.motion).children.Length; z++)
                            {
                                ((BlendTree)item.state.motion).RemoveChild(z);
                            }
                        }

                        if (LstGestures.ContainsKey(item.state.name))
                        {
                            //   LstTree = LstGestures[GestureName];

                            LstGestures[item.state.name].Add((BlendTree)item.state.motion);
                        } else
                        {
                            LstGestures.Add(item.state.name, new List<BlendTree>() { (BlendTree)item.state.motion });

                        }

                    }

                    layeridx++;
                }
            }


            string textureDirectory = "";

            string fullClipPath = "";






            counter = 0;
            tCount = 0;
            totalCount = strArray1.Length / progressUpdateInterval;
            foreach (string str3 in strArray1)
            {

                if (!string.IsNullOrEmpty(str3) && !str3.StartsWith("#") && !str3.StartsWith(":"))
                {
                    UpdateProgress();

                    string[] strArray2 = str3.Split(';');
                    if (strArray2.Length != 10)
                    {
                        EditorUtility.DisplayDialog("File format error", "Failed to import '" + dataFile + "'", "Ok");
                        return;
                    }
                    var GestName = strArray2[0];
                    var fps = int.Parse(strArray2[1]);
                    var TorsoPath = strArray2[2];
                    var TorsoReverse = strArray2[3] == "R" ? true : false;
                    var LegsPath = strArray2[4];
                    var LegsReverse = strArray2[5] == "R" ? true : false;
                    var LarmPath = strArray2[6];
                    var LarmReverse = strArray2[7] == "R" ? true : false;
                    var RarmPath = strArray2[8];
                    var RarmReverse = strArray2[9] == "R" ? true : false;

                    var ArrayName = GestName.Split('-');
                    var GestureName = ArrayName[0];
                    var Orientation = ArrayName[1];

                    var LstTree = new List<BlendTree>();

                    if (LstGestures.ContainsKey(GestureName))
                    {
                        LstTree = LstGestures[GestureName];
                    } else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            BlendTree tree = new BlendTree();
                            controller.CreateBlendTreeInController(GestureName, out tree, i);
                            tree.useAutomaticThresholds = false;
                            tree.blendType = BlendTreeType.Simple1D;

                            LstTree.Add(tree);
                        }

                        LstGestures.Add(GestureName, LstTree);

                    }


                    var filename = GestName + ".anim";

                    textureDirectory = System.IO.Path.GetDirectoryName(TorsoPath) + System.IO.Path.DirectorySeparatorChar + "AnimClip" + System.IO.Path.DirectorySeparatorChar;
                    Debug.Log(textureDirectory + filename.Replace("-", "-" + 0 + "-"));
                    var Clip = new AnimationClip();// AssetDatabase.LoadAssetAtPath<AnimationClip>(textureDirectory + filename.Replace("-", "-" + 0 + "-")); //CreateAnimClipFromGesture(GestName, "Torso", TorsoPath, fps, TorsoReverse);
                    Clip.name = filename.Replace("-", "-" + 0 + "-");

                    if (LstTree[0].children.Where(z => z.motion.name == Clip.name).Count() == 0)
                    {
                        LstTree[0].AddChild(Clip, float.Parse(Orientation));
                    }
                    //else
                    //{
                    //    var motions = LstTree[0].children;
                    //    for (int i = 0; i < motions.Length; i++)
                    //    {
                    //        if (motions[i].motion.name== Clip.name)
                    //        {
                    //            motions[i].motion = Clip;
                    //        }

                    //    }

                    //    LstTree[0].children = motions;



                    //    var stop = 0;
                    //}


                    textureDirectory = System.IO.Path.GetDirectoryName(LegsPath) + System.IO.Path.DirectorySeparatorChar + "AnimClip" + System.IO.Path.DirectorySeparatorChar;
                    Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(textureDirectory + filename.Replace("-", "-" + 1 + "-")); //CreateAnimClipFromGesture(GestName, "Legs", LegsPath, fps, LegsReverse);
                    Clip = new AnimationClip();
                    Clip.name = filename.Replace("-", "-" + 1 + "-");
                    if (LstTree[1].children.Where(z => z.motion.name == Clip.name).Count() == 0)
                    {
                        LstTree[1].AddChild(Clip, float.Parse(Orientation));
                    }

                    textureDirectory = System.IO.Path.GetDirectoryName(LarmPath) + System.IO.Path.DirectorySeparatorChar + "AnimClip" + System.IO.Path.DirectorySeparatorChar;
                    Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(textureDirectory + filename.Replace("-", "-" + 2 + "-")); //CreateAnimClipFromGesture(GestName, "Larm", LarmPath, fps, LarmReverse);
                    Clip = new AnimationClip();
                    Clip.name = filename.Replace("-", "-" + 2 + "-");
                    if (LstTree[2].children.Where(z => z.motion.name == Clip.name).Count() == 0)
                    {
                        LstTree[2].AddChild(Clip, float.Parse(Orientation));
                    }

                    textureDirectory = System.IO.Path.GetDirectoryName(RarmPath) + System.IO.Path.DirectorySeparatorChar + "AnimClip" + System.IO.Path.DirectorySeparatorChar;
                    Clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(textureDirectory + filename.Replace("-", "-" + 3 + "-")); //CreateAnimClipFromGesture(GestName, "Rarm", RarmPath, fps, RarmReverse);
                    Clip = new AnimationClip();
                    Clip.name = filename.Replace("-", "-" + 3 + "-");
                    if (LstTree[3].children.Where(z => z.motion.name == Clip.name).Count() == 0)
                    {
                        LstTree[3].AddChild(Clip, float.Parse(Orientation));
                    }

                    // break;
                }
            }

            var layers = controller.layers;
            for (int i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                for (int z = 0; z < layer.stateMachine.states.Length; z++)
                {
                    var state = layer.stateMachine.states[z];
                    if (layer.syncedLayerIndex >= 0)
                    {

                        layer.SetOverrideMotion(controller.layers[layer.syncedLayerIndex].stateMachine.states[z].state, state.state.motion);
                        //     layer.SetOverrideMotion(state.state, state.state.motion);

                        //if (i > 1)
                        //{
                        //    var mot = layer.GetOverrideMotion(state.state);
                        //    var mot2 = layer.GetOverrideMotion(controller.layers[layer.syncedLayerIndex].stateMachine.states[z].state);
                        //    var zuu = 0;
                        //}
                    }

                }

            }

            controller.layers = layers;

        }

        private static void LoadGesturesData(string dataFile)
        {
            string[] strArray1 = File.ReadAllLines(dataFile);

            List<SpriteMetaData> list = new List<SpriteMetaData>();

            counter = 0;
            tCount = 0;
            totalCount = strArray1.Length / progressUpdateInterval;
            foreach (string str3 in strArray1)
            {
                if (!string.IsNullOrEmpty(str3) && !str3.StartsWith("#") && !str3.StartsWith(":"))
                {
                    UpdateProgress();

                    string[] strArray2 = str3.Split(';');
                    if (strArray2.Length != 10)
                    {
                        EditorUtility.DisplayDialog("File format error", "Failed to import '" + dataFile + "'", "Ok");
                        return;
                    }
                    var GestName = strArray2[0];
                    var fps = int.Parse(strArray2[1]);
                    var TorsoPath = strArray2[2];
                    var TorsoReverse = strArray2[3] == "R" ? true : false;
                    var LegsPath = strArray2[4];
                    var LegsReverse = strArray2[5] == "R" ? true : false;
                    var LarmPath = strArray2[6];
                    var LarmReverse = strArray2[7] == "R" ? true : false;
                    var RarmPath = strArray2[8];
                    var RarmReverse = strArray2[9] == "R" ? true : false;

                    var filename = GestName + ".anim";

                    var Clip = CreateAnimClipFromGesture(GestName, "Torso", TorsoPath, fps, TorsoReverse);
                    TryToSaveClip(Clip, TorsoPath, filename.Replace("-", "-" + 0 + "-"));

                    Clip = CreateAnimClipFromGesture(GestName, "Legs", LegsPath, fps, LegsReverse);
                    TryToSaveClip(Clip, LegsPath, filename.Replace("-", "-" + 1 + "-"));

                    Clip = CreateAnimClipFromGesture(GestName, "Larm", LarmPath, fps, LarmReverse);
                    TryToSaveClip(Clip, LarmPath, filename.Replace("-", "-" + 2 + "-"));

                    Clip = CreateAnimClipFromGesture(GestName, "Rarm", RarmPath, fps, RarmReverse);
                    TryToSaveClip(Clip, RarmPath, filename.Replace("-", "-" + 3 + "-"));

                    //return;
                }
            }

        }

        private static AnimationClip CreateAnimClipFromGesture(string GestName, string IntPath, string TextPath, int fps, bool Reverse)
        {
            // Create a new Clip
            AnimationClip clip = new AnimationClip();

            // Apply the name and framerate
            clip.name = GestName;
            clip.frameRate = fps;

            // Apply Looping Settings
            AnimationClipSettings clipSettings = new AnimationClipSettings();
            clipSettings.loopTime = true;
            clipSettings.cycleOffset = Reverse ? 0.5f : 0f;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

            // Initialize the curve property for the animation clip
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.propertyName = "m_Sprite";
            // Assumes user wants to apply the sprite property to the root element
            curveBinding.path = IntPath;
            curveBinding.type = typeof(SpriteRenderer);

            //Extract Sprites From TargetTexture

            Sprite[] sprites = GetSpritesFromTexture(AssetDatabase.LoadAssetAtPath<Texture2D>(TextPath));

            // Build keyframes for the property using the supplied Sprites
            ObjectReferenceKeyframe[] keys = CreateKeysForSprites(sprites, fps);

            // Build the clip if valid
            if (keys.Length > 0)
            {
                // Set the keyframes to the animation
                AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keys);
            }

            return clip;


        }

        static Sprite[] GetSpritesFromTexture(Texture2D texture)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Can't find sprites from Texture at path: " + path);
                return null;
            }

            // Load all the sprites from the texture path
            // (Note, this does not load them in the order they appear in editor so they must be sorted)
            Sprite[] spriteArray = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

            // Sort the spritelist in editor order
            List<Sprite> spriteList = new List<Sprite>(spriteArray);
            spriteList.Sort(delegate (Sprite x, Sprite y)
            {
                return EditorUtility.NaturalCompare(x.name, y.name);
            });

            return spriteList.ToArray();
        }

        static ObjectReferenceKeyframe[] CreateKeysForSprites(Sprite[] sprites, int samplesPerSecond)
        {
            List<ObjectReferenceKeyframe> keys = new List<ObjectReferenceKeyframe>();
            float timePerFrame = 1.0f / samplesPerSecond;
            float currentTime = 0.0f;
            foreach (Sprite sprite in sprites)
            {
                ObjectReferenceKeyframe keyframe = new ObjectReferenceKeyframe();
                keyframe.time = currentTime;
                keyframe.value = sprite;
                keys.Add(keyframe);

                currentTime += timePerFrame;
            }

            return keys.ToArray();
        }

        static void TryToSaveClip(AnimationClip clip, string TargetPath, string FileName)
        {
            string textureDirectory = System.IO.Path.GetDirectoryName(TargetPath) + System.IO.Path.DirectorySeparatorChar + "AnimClip" + System.IO.Path.DirectorySeparatorChar;

            string fullClipPath = textureDirectory + FileName;

            try
            {
                SaveClip(clip, fullClipPath, true);
            } catch (System.IO.IOException)
            {
                if (EditorUtility.DisplayDialog("Warning: File Exists",
                        "This will overwrite the existing clip, " + FileName +
                        ". Are you sure you want to create the clip?", "Yes", "No"))
                {
                    SaveClip(clip, fullClipPath, true);
                }
            }
        }

        static void SaveClip(AnimationClip clip, string path, bool allowOverride)
        {
            if (!File.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            if (!allowOverride && System.IO.File.Exists(path))
            {
                throw new System.IO.IOException("Clip already exists at path");
            } else
            {
                AssetDatabase.CreateAsset(clip, path);

                Debug.Log("Saved: " + path);
            }
        }

        static void UpdateProgress()
        {
            if (counter++ == progressUpdateInterval)
            {
                counter = 0;
                EditorUtility.DisplayProgressBar("Creating Anims...", "", Mathf.InverseLerp(0, totalCount, ++tCount));
            }
        }
    }
}
