using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MechCommanderUnity
{
    class ChangeAnimatorBlendTrees
    {

        #region Menu Items
        [MenuItem("Assets/MCUnity/Modify Animator")]
        public static void CreateAnimClip()
        {

            return;
//            var controller = (AnimatorController)Selection.activeObject;
//
//            BlendTree tree = new BlendTree();
//
//
//            controller.CreateBlendTreeInController("t", out tree, 0);
//
//            tree.useAutomaticThresholds = false;
//            tree.blendType = BlendTreeType.Simple1D;
//            tree.maxThreshold = 31f;
//
//            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Sprites/Mechs/3/5/AnimClip/park-00.anim");
//
//            for (int i = 0; i < 32; i++)
//            {
//                var mot = new Motion();
//
//                clip = CreateAnimClipFromGesture("asd", "Torso", "Assets/Sprites/Mechs/3/5/000.png", 13, false);
//
//                tree.AddChild(clip, (float)i);
//
//                var aux2 = 0;
//            }
//
//            var aux = 0;

        }
        #endregion

        #region Menu Item Validation
        [MenuItem("Assets/MCUnity/Modify Animator", true)]
        static bool ValidateSelect()
        {
            return false;

            if (!IsSelectionValidAnimator())
            {
                return false;
            }

            //if (Selection.objects.Length > 1)
            //{
            //    return false;
            //}

            return true;
        }

        static bool IsSelectionValidAnimator()
        {
            if (Selection.activeObject == null)
            {
                return false;
            }

            return Selection.activeObject.GetType() == typeof(AnimatorController);
        }
        #endregion



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
            clipSettings.mirror = Reverse;
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
    }


}
