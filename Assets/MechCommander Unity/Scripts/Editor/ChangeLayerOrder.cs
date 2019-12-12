using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MechCommanderUnity.API
{
    [Serializable]
    class MapElvFile
    {
        [Serializable]
        public struct MCTile
        {
            public byte Heigth;
            public byte algo;
            public byte otro;
            public byte otra;
            public ushort TileId;
            public short OverlayTileId;
        }
    }
}
namespace Assets.MechCommander_Unity.Scripts.Editor
{
    class ChangeLayerOrder
    {
        


        // [MenuItem("Assets/ChangeOrderLayer")]
        public static void ChangeLayer()
        {
            return;
            var selected = (GameObject)Selection.activeObject;


            foreach (var spriteRend in selected.GetComponentsInChildren<SpriteRenderer>())
            {
                if (spriteRend.sortingLayerID == 825352053)
                {
                    spriteRend.sortingLayerName = "Terrain";
                }
                else if (spriteRend.sortingLayerID == 1695910223)
                {
                    spriteRend.sortingLayerName = "Overlays";
                } else
                {
                    Debug.Log(spriteRend.name + " -> " + spriteRend.sortingLayerID + " -> " + spriteRend.sortingLayerName + "  -- " + SortingLayer.GetLayerValueFromName("Terrain"));
                }
                
                //spriteRend.sortingLayerName = "Terrain";
            }

          //  ChangeLayersRecursively(selected.GetComponent<Transform>(), "Terrain");

            Debug.Log(selected);
        }

        [MenuItem("Assets/TestDeser")]
        public static void TestDeser()
        {

            using (var stream = new FileStream("test.serializable", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var formatter = new BinaryFormatter();
                var obj = (MechCommanderUnity.API.MapElvFile.MCTile[])formatter.Deserialize(stream);
            }
        }


        public static void ChangeLayersRecursively(Transform trans, string name)
        {
            trans.gameObject.layer = LayerMask.NameToLayer(name);
            foreach (Transform child in trans)
            {
                ChangeLayersRecursively(child,name);
            }
        }

    }
}
