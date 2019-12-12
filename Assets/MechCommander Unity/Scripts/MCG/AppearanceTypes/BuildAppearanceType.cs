using System;
using MechCommanderUnity.API;
using MechCommanderUnity.MCG.AppearanceTypes;
using MechCommanderUnity.Utility;
using System.Collections.Generic;

namespace MechCommanderUnity.MCG.AppearanceTypes
{
    public class BuildAppearanceType : AppearanceType
    {
        #region Class Variables
        int numFrames;
        
        public List<ActorData> AppearStates = new List<ActorData>();
        
        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        public BuildAppearanceType(FITFile spriteFile) : base(spriteFile)
        {
            spriteFile.SeekSection("Main Info");


            spriteFile.GetInt("NumFrames", out numFrames);
            
            var state = new ActorData();
            state.state = ActorStates.STATE_NORMAL;
            state.basePacketNumber = 0;
            state.numFrames = 1;
            state.numRotations = 1;
            state.frameRate = 1;
            state.symmetrical=1;
            state.delta = 1;
            AppearStates.Add(state);
            
            state = new ActorData();
            state.state = ActorStates.STATE_BLOWING_UP1;
            state.basePacketNumber = 0;
            state.numFrames = numFrames;
            state.numRotations = 1;
            state.frameRate = 15;
            state.symmetrical=1;
            state.delta = 1;
            AppearStates.Add(state);
            
            state = new ActorData();
            state.state = ActorStates.STATE_DESTROYED;
            state.basePacketNumber = 0;
            state.numFrames = 1;
            state.numRotations = 1;
            state.frameRate = 1;
            state.symmetrical=1;
            state.delta = 1;
            AppearStates.Add(state);

            //0 To 11 is the building "Shadow" normal & destroyed


        }


        #endregion

        #region Public Functions

        public void InitSprites()
        {
            List<int> shpIds = new List<int>();
            for (int i = 12; i < 12+numFrames; i++)
            {
                shpIds.Add(i);
            }
            for (int i = 0; i < 12; i++)
            {
                shpIds.Add(i);
            }
            SpriteManager.Instance.QueueSpriteCreate(appearanceNum, shpIds);
        }

        public SpriteManager.SpriteData SpriteData
        {
            get
            {
                return SpriteManager.Instance.GetSpriteData(appearanceNum);
            }
        }
        #endregion

    }
}