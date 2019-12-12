using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MechCommanderUnity.API;
using MechCommanderUnity.Utility;
using UnityEngine;

namespace MechCommanderUnity.MCG.AppearanceTypes
{
    public class VFXAppearanceType : AppearanceType
    {
        #region Class Variables
        int numStates;
        int scaled;
        int subStates;

        public bool treeBuilding = false; //is a Building with animation (substates)


        public List<ActorData> AppearStates = new List<ActorData>();

        #endregion



        #region Class Structures

        #endregion
        #region Constructors
        public VFXAppearanceType(FITFile spriteFile) : base(spriteFile)
        {

            if (!spriteFile.SeekSection("States"))
                return;
            
            if (!spriteFile.GetInt("NumStates", out numStates))
                return;
            
            if (!spriteFile.GetInt("Scaled", out scaled))
                scaled = 0;

            int ScaledOffset = scaled == 1 ? -1 : 0;

            for (int i = 0; i < numStates; i++)
            {
                if (spriteFile.SeekSection("State" + i))
                {
                    if (!spriteFile.GetInt("SubStates", out subStates))
                        subStates = 0;
                    var state = new ActorData();
                    
                    if (!spriteFile.GetInt("State", out var stat))
                        return;

                    state.state = (ActorStates)stat;

                    if (!spriteFile.GetInt("NumFrames", out state.numFrames))
                        return;
                    if (!spriteFile.GetFloat("FrameRate", out state.frameRate))
                        return;
                    if (!spriteFile.GetInt("BasePacketNumber", out state.basePacketNumber))
                        return;
                    if (!spriteFile.GetInt("NumRotations", out state.numRotations))
                        return;
                    if (!spriteFile.GetInt("Symmetrical", out state.symmetrical))
                        return;
                    if (!spriteFile.GetInt("Loop", out int loop))
                        loop=0;

                    //delta ==1 First is Shadow?¿ && state.numFrames>1 && loop==1
                    state.delta = (delta==1 && state.numFrames>1) ? 1: 0 ;
                   // state.numFrames += state.delta;

                    state.shapeId = (state.basePacketNumber + ((1 + ScaledOffset) * 1));

                    AppearStates.Add(state);


                    //Do something with the state data

                    int numFrames, basePacketNumber;
                    float frameRate;
                    int numRotations, symmetrical;
                    for (int j = 0; j < subStates; j++)
                    {
                        treeBuilding = true;
                        if (spriteFile.SeekSection("Sub" + j + "State" + i))
                        {
                            if (!spriteFile.GetInt("NumFrames", out numFrames))
                                return;
                            if (!spriteFile.GetFloat("FrameRate", out frameRate))
                                return;
                            if (!spriteFile.GetInt("BasePacketNumber", out basePacketNumber))
                                return;
                            if (!spriteFile.GetInt("NumRotations", out numRotations))
                                return;
                            if (!spriteFile.GetInt("Symmetrical", out symmetrical))
                                return;
                            if (!spriteFile.GetInt("Loop", out loop))
                                return;

                            //Do something with the substate data

                        }
                    }


                }
            }

        }


        #endregion

        #region Public Functions

        public void InitSprites()
        {
            List<int> shpIds = new List<int>();
            foreach (var state in AppearStates)
            {
                if (!shpIds.Contains((int)state.shapeId))
                {
                    shpIds.Add((int)state.shapeId);
                }
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
