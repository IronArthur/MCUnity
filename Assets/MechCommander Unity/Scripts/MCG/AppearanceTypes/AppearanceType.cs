using MechCommanderUnity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechCommanderUnity.MCG.AppearanceTypes
{
    public class AppearanceType
    {

        #region Class Variables

        public int numUsers;         //Number of users using this appearanceType.
        public int appearanceNum;        //What kind am I.
        public int delta =0;

        public string name;             //Appearance Base FileName.

        public int boundsUpperLeftX;
        public int boundsUpperLeftY;

        public int boundsLowerRightX;
        public int boundsLowerRightY;

        public bool designerTypeBounds = false;

        public AppearanceClass appearanceClass;

        #endregion



        #region Class Structures
        #endregion

        #region Constructors
        public AppearanceType(FITFile spriteFile)
        {
            spriteFile.SeekSection("Main Info");

            spriteFile.GetString("Name", out name);

            if (!spriteFile.GetInt("Delta", out delta))
                delta = 0;

            if (spriteFile.SeekSection("Bounds"))
            {
                designerTypeBounds = true;

                spriteFile.GetInt("UpperLeftX", out boundsUpperLeftX);
                spriteFile.GetInt("UpperLeftY", out boundsUpperLeftY);
                spriteFile.GetInt("LowerRightX", out boundsLowerRightX);
                spriteFile.GetInt("LowerRightY", out boundsLowerRightY);

            }
        }

        

        #endregion

        #region Public Functions
        #endregion


    }
}
