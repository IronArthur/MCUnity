using MechCommanderUnity.MCG.Appearances;
using MechCommanderUnity.Utility;
using System.Collections;
using System.Collections.Generic;
using MechCommander_Unity.Scripts.MCG.GameObjects;
using UnityEngine;

namespace MechCommanderUnity.MCG.GameObjectTypes
{
    public class MCGameObject : MonoBehaviour
    {
        #region Class Variables

        public ObjectClass objectClass;        //What kind of object is this.
        public ObjectType objectType;
        //GameObjectHandle handle;                //Used to reference into master obj table
        public long partId;                //What is my unique part number.
        public int appearanceId;
        //GameObjectTypeHandle typeHandle;            //Who made me?
        public Vector3 position;           //Where am I?
        public short block; //Cell RC position
        public short vertex;
        public short pixelOffsetX;
        public short pixelOffsetY;
        public long d_vertexNum;       //Physical Vertex in mapData array that I'm lower right from
        public long flags;                //See GAMEOBJECT_FLAGS_ defines
        public short debugFlags;          // use ONLY for debugging purposes...
        public char status;               //Am I normal, disabled, destroyed, etc..?

        public float tonnage;          //How hefty am I?
        public float rotation;         //everything's base facing
        public Appearance appearance;

        public float collisionFreeTime;
        //Stuff::Vector4D screenPos;          //Actual Screen position
        public long windowsVisible;        //Which Windows can see me.
        public float explRadius;           //How big is my explosion.
        public float explDamage;           //How much damage does it do?
        public short maxCV;
        public short curCV;
        public short threatRating;
        public float lastFrameTime;        //Time elapsed since last frame was drawn.  (Replaces HEAT.  No net gain in size!)
        public byte blipFrame;
        public byte numAttackers;

        public long drawFlags;         // bars, text, brackets, and highlight colors

        //public static long spanMask;          //Used to preserve tile's LOS
        //public static float blockCaptureRange;
        //public static bool initialize;

        #endregion



        #region Class Structures



        #endregion

        #region Constructors
        public void Init(ObjectType _type)
        {
            objectClass = ObjectClass.GAMEOBJECT;
            objectType = _type;
        }

        #endregion

        #region Public Functions

        public short OffsetPixelX
        {
            get { return pixelOffsetX; }
            set { pixelOffsetX += value; }
        }

        public short OffsetPixelY
        {
            get { return pixelOffsetY; }
            set { pixelOffsetY += value; }
        }

        public string Name
        {
            get { return objectType.Name; }
        }

        public Appearance.AppearanceData Appearance
        {
            get
            {
                switch (objectClass)
                {
                    case ObjectClass.TERRAINOBJECT:
                    case ObjectClass.TREE:
                        return ((TerrainObject)this).AppearanceData;
                    case ObjectClass.BUILDING:
                    case ObjectClass.TREEBUILDING:
                        return ((BuildingObject)this).AppearanceData;
                    case ObjectClass.GATE:
//                        Debug.Log("Gate "+this.appearance.type);
                        return ((GateObject)this).AppearanceData;
                    case ObjectClass.TURRET:
                        return ((TurretObject)this).AppearanceData;
                    default:
                        return null;
                }

            }
        }
        #endregion
    }
}
