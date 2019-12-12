using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MechCommanderUnity.API;
using MechCommanderUnity.MCG;

namespace MechCommanderUnity.MCG.ObjectTypes
{
    [Serializable]
    public class TerrainObjectType : ObjectType
    {
        #region Class Variables

        float damageLevel;
        int collisionOffsetX;
        int collisionOffsetY;
        bool setImpassable;
        int xImpasse;
        int yImpasse;
        float explDmg;
        float explRad;
        int fireTypeHandle;

        #endregion



        #region Class Structures

        public enum TerrainObjectSubType
        {
            TERROBJ_NONE,
            TERROBJ_TREE,
            TERROBJ_BRIDGE,
            TERROBJ_FOREST,
            TERROBJ_WALL_HEAVY,
            TERROBJ_WALL_MEDIUM,
            TERROBJ_WALL_LIGHT,
            NUM_TERROBJ_SUBTYPES
        };


        #endregion

        #region Constructors

        public TerrainObjectType() : base()
        {
            objectTypeClass = (int)ObjectTypeClass.TERRAINOBJECT_TYPE;
            objectClass = ObjectClass.TERRAINOBJECT;

            subType = (int)TerrainObjectSubType.TERROBJ_NONE;
            damageLevel = 0f;
            collisionOffsetX = 0;
            collisionOffsetY = 0;
            setImpassable = false;
            xImpasse = 0;
            yImpasse = 0;
            extentRadius = -1.0f;
            explDmg = 0.0f;
            explRad = 0.0f;
            fireTypeHandle = 0;
        }

        public TerrainObjectType(FITFile objFitFile) : base(objFitFile)
        {
            objectTypeClass = (int)ObjectTypeClass.TERRAINOBJECT_TYPE;
            objectClass = ObjectClass.TERRAINOBJECT;

            subType = (int)TerrainObjectSubType.TERROBJ_NONE;
            damageLevel = 0f;
            collisionOffsetX = 0;
            collisionOffsetY = 0;
            setImpassable = false;
            xImpasse = 0;
            yImpasse = 0;
            extentRadius = -1.0f;
            explDmg = 0.0f;
            explRad = 0.0f;
            fireTypeHandle = 0;

            if (!objFitFile.SeekSection("TerrainObjectData"))
            {
                if (!objFitFile.SeekSection("TreeData"))
                    return;

                subType = (int)TerrainObjectSubType.TERROBJ_TREE;
                objectClass = ObjectClass.TREE;
            }

            objFitFile.GetFloat("DmgLevel", out damageLevel);

            if (!objFitFile.GetInt("CollisionOffsetX", out collisionOffsetX))
                collisionOffsetX =0;

            if (!objFitFile.GetInt("CollisionOffsetY", out collisionOffsetY))
                collisionOffsetY = 0;




            int setImpass;
            objFitFile.GetInt("SetImpassable", out  setImpass);
            setImpassable = setImpass==1 ? true : false;

            if (!objFitFile.GetInt("XImpasse", out xImpasse))
                xImpasse = 0;
            if (!objFitFile.GetInt("YImpasse", out yImpasse))
                yImpasse = 0;

            float realExtent = 0f;
            if (!objFitFile.GetFloat("ExtentRadius", out realExtent))
                realExtent = -1f;


            if (!objFitFile.GetFloat("ExplosionRadius", out explRad))
                explRad = 0f;// if this fails, explosion radius is not set and no splash damage.

            if (!objFitFile.GetFloat("ExplosionDamage", out explDmg))
                explDmg = 0f; // if this fails, explosion damage is not set and no splash damage.

           
            //-------------------------------------------------------
            // Initialize the base object Type from the current file.
            
            extentRadius = realExtent;



        }

        #endregion

        #region Public Functions

        public float DamageLevel
        {
            get { return (damageLevel); }

        }

        #endregion

    }
}
