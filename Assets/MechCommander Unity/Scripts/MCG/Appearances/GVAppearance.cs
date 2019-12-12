using UnityEngine;
using System.Collections;
using MechCommanderUnity.MCG.AppearanceTypes;
using MechCommanderUnity.MCG.GameObjectTypes;
using System.Collections.Generic;
using System.Linq;

namespace MechCommanderUnity.MCG.Appearances
{
    public class GVAppearance : Appearance
    {

        #region Class Variables
        GVAppearanceType appearType;
        Texture2D currentTexture;
        int currentFrame;
        public float currentRotation;


        public GVActorStates currentShapeTypeId;

        AppearanceData AppData;


        #endregion

        #region Class Structures

        #endregion

        #region Constructors
        public GVAppearance(AppearanceType tree = null, MCGameObject obj = null) : base(tree, obj)
        {
            appearType = (GVAppearanceType)tree;

            currentTexture = null;
            currentFrame = -1;
            currentRotation = 0;

            currentShapeTypeId = GVActorStates.STATE_STOPPED;

        }
        #endregion

        #region Public Functions
        public List<GVActorData> AppearStates
        {
            get
            {
                return ((GVAppearanceType)type).AppearStates;
            }
        }

        public GVActorData ActualState
        {
            get
            {
                return AppearStates.Where(x => x.state == currentShapeTypeId).First();
            }
        }

        public AppearanceData SpriteData
        {
            get
            {
                if (currentTexture == null)
                {
                    Utility.SpriteManager.SpriteData data;

                    data = ((GVAppearanceType)type).SpriteData;
                    AppData = new AppearanceData();

                    if (data == null)
                        return AppData;


                    currentTexture = data.MainText;

                }

                if (currentTexture != null)
                {

                    var data = ((GVAppearanceType)type).SpriteData;

                    var state = ActualState;

                    if (state.numFrames > 1)
                    {
                        int basepacket = state.basePacketNumber + (int)currentRotation;
                        
                        var uvs = new List<Vector2>();
                        
                        for (long i = 0; i < state.numFrames; i++)
                        {
                            if (data.uvs.ContainsKey(basepacket + "-" + i))
                            {
                                uvs.AddRange(data.uvs[basepacket + "-" + i]);
                            }
                        }

                        AppData = new AppearanceData()
                        {
                            currentTexture = data.MainText,
                            autoStart = false,
                            vertices = data.Vertices[basepacket + "-" + 0],
                            uvs = uvs.ToArray(),
                            uvsFloor = new Vector2[0],
                            fps = state.frameRate,
                            isAnim = true,
                            loop = false,
                            numFrames = uvs.Count/4
                        };
                    } else
                    {
                        int basepacket = state.basePacketNumber + (int)currentRotation;

                        AppData = new AppearanceData()
                        {
                            currentTexture = data.MainText,
                            autoStart = false,
                            vertices = data.Vertices[basepacket + "-" + 0],
                            uvs = data.uvs[basepacket + "-" + 0],
                            uvsFloor = new Vector2[0],
                            fps = 15f,
                            isAnim = false,
                            loop = false,
                            numFrames = 1
                        };
                    }
                    
                    return AppData;
                }

                return new AppearanceData();

            }
        }
        #endregion
    }
}