using MechCommanderUnity.API;
using MechCommanderUnity.MCG.AppearanceTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechCommanderUnity.MCG
{
    public class AppearanceTypeManager
    {
        #region Class Variables
        PakFile spritePakFile;
        Dictionary<int, AppearanceType> AppearanceTypeList;

        #endregion



        #region Class Structures

        

        #endregion

        #region Constructors


        public AppearanceTypeManager(string spriteFileName)
        {
            spriteFileName = System.IO.Path.ChangeExtension(spriteFileName, "pak");

            this.spritePakFile = new PakFile(MCGExtensions.PathCombine(new string[] { MechCommanderUnity.Instance.MCGPath, "sprites", spriteFileName }));


            AppearanceTypeList = new Dictionary<int, AppearanceType>();
        }

        #endregion

        #region Public Functions

        public AppearanceType GetAppearanceType(int AppearNumber)
        {
            //----------------------------------------------------------------
            // The type of appearance is stored in the upper 8 bits of the
            // apprNum.  To get the correct packet we mask off the top 8 bits
            // and store the number.  To get the appearance type, we right shift
            // by 24.

            var appearanceClass = (AppearanceClass)(AppearNumber >> 24);
            //----------------------------------------------------
            // If these top bits are wrong, return NULL
            if (appearanceClass == AppearanceClass.BASE_APPEARANCE)
                return null;

            int appearanceNumber = AppearNumber & 0xFFFFFF;

            AppearanceType appearanceType = null;
            if (AppearanceTypeList.ContainsKey(appearanceNumber))
            {
                appearanceType = AppearanceTypeList[appearanceNumber];
                appearanceType.numUsers++;
            } else
            {
                var spriteFitData = spritePakFile.GetFileInner(appearanceNumber);
                if (spriteFitData == null)
                {
                    return null;
                }

                var spriteFitFile = new FITFile(spriteFitData);


                // 1,2,5,9,6,7,8,4
                switch (appearanceClass)
                {
                    case AppearanceClass.SPRITE_TREE://1
                        Debug.Log("SPRITE_TREE"+ AppearNumber + " -> " + appearanceNumber);
                        // appearanceType = new Mech3DAppearanceType;

                        //  appearanceType.appearanceNum = appearanceNumber;
                        //  appearanceType.init(appearFile);

                        //----------------------------------------
                        // We have a new one, add it to the list.				
                        //  appearanceType.numUsers = 1;

                        //  AppearanceTypeList.Add(appearanceNumber, appearanceType);
                        break;
                    case AppearanceClass.VFX_APPEAR://2
                        //Debug.Log("VFX APPEAR"+ AppearNumber + " -> " + appearanceNumber);
                        appearanceType = new VFXAppearanceType(spriteFitFile);
                        appearanceType.appearanceNum = appearanceNumber;
                        appearanceType.appearanceClass = AppearanceClass.VFX_APPEAR;

                        ((VFXAppearanceType)appearanceType).InitSprites();
                        //----------------------------------------
                        // We have a new one, add it to the list.				
                        appearanceType.numUsers = 1;
                        AppearanceTypeList.Add(appearanceNumber, appearanceType);

                        break;
                    case AppearanceClass.GV_TYPE://5
                        appearanceType = new GVAppearanceType(spriteFitFile);
                        appearanceType.appearanceNum = appearanceNumber;
                        appearanceType.appearanceClass = AppearanceClass.GV_TYPE;

                        ((GVAppearanceType)appearanceType).InitSprites();
                        //----------------------------------------
                        // We have a new one, add it to the list.				
                        appearanceType.numUsers = 1;
                        AppearanceTypeList.Add(appearanceNumber, appearanceType);

                        break;
                    case AppearanceClass.PU_TYPE://9
                        //TODO specific appearance type for this one
                        Debug.Log("PU_Type using GV Appearance");
                        appearanceType = new GVAppearanceType(spriteFitFile);
                        appearanceType.appearanceNum = appearanceNumber;
                        appearanceType.appearanceClass = AppearanceClass.GV_TYPE;

                        ((GVAppearanceType)appearanceType).InitSprites();
                        //----------------------------------------
                        // We have a new one, add it to the list.				
                        appearanceType.numUsers = 1;
                        AppearanceTypeList.Add(appearanceNumber, appearanceType);
                        break;
                    case AppearanceClass.ARM_APPEAR://6
                        break;
                    case AppearanceClass.BUILD_APPEAR://7
//                        Debug.Log("BUIDLING " + AppearNumber + " -> " + appearanceNumber);
                        appearanceType = new BuildAppearanceType(spriteFitFile);
                        appearanceType.appearanceNum = appearanceNumber;
                        appearanceType.appearanceClass = AppearanceClass.BUILD_APPEAR;

                        ((BuildAppearanceType)appearanceType).InitSprites();
                        //----------------------------------------
                        // We have a new one, add it to the list.				
                        appearanceType.numUsers = 1;
                        AppearanceTypeList.Add(appearanceNumber, appearanceType);
                        break;
                    case AppearanceClass.ELM_TREE://8
                        break;
                    case AppearanceClass.LINE_APPEAR://4
                        break;

                    default:
                        break;
                }
            }


            return appearanceType;
        }

        #endregion
    }
}
