using MechCommanderUnity;
using MechCommanderUnity.MCG;
using MechCommanderUnity.MCG.Appearances;
using MechCommanderUnity.MCG.GameObjectTypes;
using MechCommanderUnity.MCG.ObjectTypes;
using UnityEngine;

namespace MechCommander_Unity.Scripts.MCG.GameObjects
{
    public class GateObject :BuildingObject
    {
	    #region Class Variables
	    long	dmgLevel;
	    
	    long	blownEffectId;
	    long	normalEffectId;
	    long	damageEffectId;
	
	    float			baseTonnage;
		
	    long			basePixelOffsetX;
	    long			basePixelOffsetY;
		
	    float			explDmg;
	    float			explRad;

	    float			openRadius;
		
	    float			littleExtent;
				
	    long			gateTypeName;

	    bool			blocksLineOfFire;

	    long			buildingDescriptionID;
	    
	    #endregion
	    
	    
	    #region Constructors

	    public void Init(ObjectType objType, AppearanceTypeManager appearanceTypeManager)
	    {
		    base.Init(objType,appearanceTypeManager);
		    
		    objectClass = ObjectClass.GATE;
		    
		    var buildingAppearanceType = appearanceTypeManager.GetAppearanceType(objType.Appearance);

		    appearance = new GVAppearance(buildingAppearanceType, this);
		    
		    
	    }
	    #endregion
	    
	    #region Public Functions
	    public Appearance.AppearanceData AppearanceData
	    {
		    get
		    {
			    if (appearance.type == null)
			    {
				    Debug.Log("Error appearance type null in gateobject");
				    // return null;
			    }

			    return ((GVAppearance)appearance).SpriteData;

			    return null;


		    }
	    }
	    #endregion
    }
}