using UnityEngine;
using System.Collections;
using MechCommanderUnity.MCG.Appearances;
using MechCommanderUnity.MCG.ObjectTypes;

namespace MechCommanderUnity.MCG.GameObjectTypes
{
    public class TurretObject : MCGameObject
    {

        #region Class Variables
        static int MAX_TURRET_WEAPONFIRE_CHUNKS = 32;
        static int MAX_TURRET_WEAPONS = 4;

        sbyte teamId;
        float turretRotation;
        bool didReveal;
        //GameObjectWatchID targetWID;
        float[] readyTime;
        float[] lastFireTime;
        float minRange;                                     // current min attack range
        float maxRange;                                     // current max attack range
        long numFunctionalWeapons;                          // takes into account damage, etc.

        long netRosterIndex;
        long[] numWeaponFireChunks;
        ulong[][] weaponFireChunks;//MAX_TURRET_WEAPONFIRE_CHUNKS

        //TG_LightPtr pointLight;
        //DWORD lightId;
        float idleWait;
        //Stuff::Vector3D idlePosition;
        //Stuff::Vector3D oldPosition;
        //DWORD parentId;
        //GameObjectWatchID parent;
        long currentWeaponNode;

        //static bool turretsEnabled[MAX_TEAMS];

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        public void Init(ObjectType objType, AppearanceTypeManager appearanceTypeManager)
        {
            base.Init(objType);
            readyTime = new float[MAX_TURRET_WEAPONS];
            lastFireTime = new float[MAX_TURRET_WEAPONS];
            numWeaponFireChunks = new long[2];
            weaponFireChunks = new ulong[2][];
            weaponFireChunks[0] = new ulong[MAX_TURRET_WEAPONFIRE_CHUNKS];
            weaponFireChunks[1] = new ulong[MAX_TURRET_WEAPONFIRE_CHUNKS];

            //-------------------------------------------
            // Initialize the Building Appearance here.
            //GameObject::init(true, _type);

            //setFlag(OBJECT_FLAG_JUSTCREATED, true);

            var turretAppearanceType = appearanceTypeManager.GetAppearanceType(objType.Appearance);

            appearance = new GVAppearance(turretAppearanceType, this);

            objectClass = ObjectClass.TURRET;
            //setFlag(OBJECT_FLAG_DAMAGED, true);



            teamId = -1;
            readyTime[0] = readyTime[1] = readyTime[2] = readyTime[3] = 0f;

            TurretType type = (TurretType)objType;
            if (type.engageRadius != 0.0)
                type.extentRadius = (type.engageRadius);

            if (type.extentRadius > 0.0) { }
            //setFlag(OBJECT_FLAG_TANGIBLE, true);

            tonnage = type.baseTonnage;
            explDamage = type.explDmg;
            explRadius = type.explRad;

            //setFlag(OBJECT_FLAG_CAPTURABLE, false);

            //setFlag(OBJECT_FLAG_POP_NEUTRALS, false);
        }

        
        #endregion

        #region Public Functions
        public Appearance.AppearanceData AppearanceData
        {
            get
            {
                if (appearance.type == null)
                {
                    Debug.Log("Error appearance type null in turretobject");
                }

                ((GVAppearance)appearance).currentRotation = rotation;

                return ((GVAppearance)appearance).SpriteData;
            }
        }
        #endregion
    }
}