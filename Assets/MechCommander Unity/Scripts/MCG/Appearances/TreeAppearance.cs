using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MechCommanderUnity.MCG.AppearanceTypes;
using MechCommanderUnity.MCG.GameObjectTypes;
using UnityEngine;

namespace MechCommanderUnity.MCG.Appearances
{
    public class TreeAppearance : ObjectAppearance
    {
        #region Class Variables
        VFXAppearanceType appearType;
        Texture2D currentTexture;       //OK because we make sure each frame before we draw it.
        long currentFrame;
        float currentRotation;

        public ActorStates currentShapeTypeId;

        AppearanceData AppData;


        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        public TreeAppearance(AppearanceType tree = null, MCGameObject obj = null) : base(tree, obj)
        {
            appearType = (VFXAppearanceType)tree;

            currentTexture = null;
            currentFrame = -1;
            currentRotation = 0;

            currentShapeTypeId = ActorStates.STATE_NORMAL;

        }
        #endregion

        #region Public Functions
        public List<ActorData> AppearStates
        {
            get { return appearType.AppearStates; }
        }

        public ActorData ActualState
        {
            get { return AppearStates.Where(x => x.state == currentShapeTypeId).First(); }
        }

        public AppearanceData SpriteData
        {
            get
            {
                if (currentTexture == null)
                {
                    Utility.SpriteManager.SpriteData data;

                    data = ((VFXAppearanceType)type).SpriteData;
                    AppData = new AppearanceData();

                    if (data == null)
                        return AppData;


                    currentTexture = data.MainText;

                }

                if (currentTexture != null)
                {
                    var data = ((VFXAppearanceType)type).SpriteData;

                    var state = this.ActualState;

                    if (state.numFrames > 1)
                    {
                        long basepacket = state.basePacketNumber;

                        var uvs = new List<Vector2>();
                        var uvsFloor = new List<Vector2>();

                        for (long i = state.delta; i < (state.numFrames+state.delta); i++)
                        {
                            if (data.uvs.ContainsKey(basepacket + "-" + i))
                            {
                                uvs.AddRange(data.uvs[basepacket + "-" + i]);
                            }
                        }
                        for (long i = 0; i < state.delta; i++)
                        {
                            if (data.uvs.ContainsKey(basepacket + "-" + i))
                            {
                                uvsFloor.AddRange(data.uvs[basepacket + "-" + i]);
                            }
                        }

                        AppData = new AppearanceData()
                        {
                            currentTexture = data.MainText,
                            autoStart = false,
                            vertices = data.Vertices[basepacket + "-" + 0],
                            uvs = uvs.ToArray(),
                            uvsFloor = uvsFloor.ToArray(),
                            fps = state.frameRate,
                            isAnim = true,
                            loop = false,
                            numFrames = state.numFrames
                        };
                    } else
                    {
                        if (state.delta != 0)
                        {
                            Debug.Log("Aqui delta 1 -" +state.numFrames);
                        }
                        
                        AppData = new AppearanceData()
                        {
                            currentTexture = data.MainText,
                            autoStart = false,
                            vertices = data.Vertices["0-0"],
                            uvs = data.uvs["0-0"],
                            uvsFloor = new Vector2[0],
                            fps = 1f,
                            isAnim = false,
                            loop = false,
                            numFrames = 1
                        };
                    }

                    return AppData;

                }

                return null;

            }
        }
        #endregion

    }
}
