using UnityEngine;
using System.Collections;
using MechCommanderUnity.API;
using System.Collections.Generic;
using MechCommanderUnity.Utility;

namespace MechCommanderUnity.MCG.AppearanceTypes
{
    public class GVAppearanceType : AppearanceType
    {

        #region Class Variables

        int numStates;
        int numParts;
        float turretOffset;

        public List<GVActorData> AppearStates = new List<GVActorData>();


        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        public GVAppearanceType(FITFile spriteFile) : base(spriteFile)
        {
            if (!spriteFile.SeekSection("States"))
            {
                return;
            }
            if (!spriteFile.GetInt("NumStates", out numStates))
            {
                return;
            }

            for (int i = 0; i < numStates; i++)
            {
                if (spriteFile.SeekSection("State" + i))
                {

                    var state = new GVActorData();

                    int stat;
                    if (!spriteFile.GetInt("State", out stat))
                        return;

                    state.state = (GVActorStates)stat;

                    if (!spriteFile.GetInt("NumFrames", out state.numFrames))
                        return;
                    if (!spriteFile.GetFloat("FrameRate", out state.frameRate))
                        return;
                    if (!spriteFile.GetInt("BasePacketNumber", out state.basePacketNumber))
                        return;
                    if (!spriteFile.GetInt("NumRotations", out state.numRotations))
                        return;
                   

                    state.shapeId = state.basePacketNumber;

                    AppearStates.Add(state);


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
                for (int i = 0; i < state.numRotations; i++)
                {
                    if (!shpIds.Contains(state.shapeId + i))
                    {
                        shpIds.Add(state.shapeId + i);
                    }
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