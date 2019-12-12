using MechCommanderUnity.MCG.Appearances;
using MechCommanderUnity.MCG.AppearanceTypes;
using MechCommanderUnity.MCG.ObjectTypes;
using MechCommanderUnity.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechCommanderUnity.MCG.GameObjectTypes
{
    public class TerrainObject : MCGameObject
    {
        #region Class Variables
        #endregion



        #region Class Structures
        #endregion

        #region Constructors
        public void Init(ObjectType objType,AppearanceTypeManager appearanceTypeManager)
        {
            base.Init(objType);
            if (((TerrainObjectType)objType).subType == (int)TerrainObjectType.TerrainObjectSubType.TERROBJ_TREE)
            {

                var treeAppearType=appearanceTypeManager.GetAppearanceType(objType.Appearance);

                appearance = new TreeAppearance(treeAppearType,this);

            }else
            {
                var terrainObjectAppearType = appearanceTypeManager.GetAppearanceType(objType.Appearance);
                appearance = new BuildingAppearance(terrainObjectAppearType, this);
            }


            this.objectClass = ObjectClass.TERRAINOBJECT;

            switch (((TerrainObjectType)objType).subType)
            {
                case (int)TerrainObjectType.TerrainObjectSubType.TERROBJ_NONE:
                    if (((TerrainObjectType)objType).DamageLevel == 0f)
                    {
                        //--------------------------------------------------------
                        // We are already destroyed.  Used for extraction Markers
                        //setTangible(false);
                        //setStatus(OBJECT_STATUS_DESTROYED);
                    }
                    break;
                case (int)TerrainObjectType.TerrainObjectSubType.TERROBJ_TREE:
                    objectClass = ObjectClass.TREE;
                    //setFlag(OBJECT_FLAG_DAMAGED, false);
                    break;
                case (int)TerrainObjectType.TerrainObjectSubType.TERROBJ_BRIDGE:
                case (int)TerrainObjectType.TerrainObjectSubType.TERROBJ_FOREST:
                case (int)TerrainObjectType.TerrainObjectSubType.TERROBJ_WALL_HEAVY:
                case (int)TerrainObjectType.TerrainObjectSubType.TERROBJ_WALL_MEDIUM:
                case (int)TerrainObjectType.TerrainObjectSubType.TERROBJ_WALL_LIGHT:
                    //setTangible(false);
                    objectClass = ObjectClass.BRIDGE;
                    break;
            }




        }
        #endregion

        #region Public Functions


        public Appearance.AppearanceData AppearanceData
        {
            get
            {
                if (((TerrainObjectType)objectType).subType == (int)TerrainObjectType.TerrainObjectSubType.TERROBJ_TREE)
                    return ((TreeAppearance)appearance).SpriteData;
                else
                    return ((BuildingAppearance)appearance).SpriteData;
                // return ((VFXAppearanceType)appearance.type).SpriteData;
            }
        }

        #endregion
    }
}
