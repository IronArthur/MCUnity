using MechCommanderUnity.MCG.AppearanceTypes;
using MechCommanderUnity.MCG.GameObjectTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechCommanderUnity.MCG.Appearances
{
    public class Appearance
    {
        #region Class Variables
        public AppearanceType type;
        #endregion

        #region Class Structures

        public class AppearanceData
        {
            public Texture2D currentTexture;
            public Vector3[] vertices;
            public Vector3[] verticesFloor;
            public Vector2[] uvs;
            public Vector2[] uvsFloor;
            public Rect[] currentRect;
            public Vector2[] currentPivot;
            public Rect floorRect;
            public Vector2 floorPivot;
            public bool isAnim;
            public int numFrames;
            public float fps;
            public bool autoStart;
            public bool loop;

        }

        #endregion

        #region Constructors
        public Appearance(AppearanceType tree = null, MCGameObject obj = null)
        {
            type = tree;
        }
        #endregion

        #region Public Functions

        public string Name
        {
            get { return type.name; }
        }
        #endregion
    }
}
