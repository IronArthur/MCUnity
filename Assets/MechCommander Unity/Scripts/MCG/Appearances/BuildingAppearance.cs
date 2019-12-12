using MechCommanderUnity.MCG.AppearanceTypes;
using MechCommanderUnity.MCG.GameObjectTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MechCommanderUnity.MCG.Appearances
{
    public class BuildingAppearance : ObjectAppearance
    {
        #region Class Variables

        AppearanceType appearType;
        Texture2D currentTexture;       //OK because we make sure each frame before we draw it.
        int currentFrame;
        float currentRotation;

        public ActorStates currentShapeTypeId;

        AppearanceData AppData;

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        public BuildingAppearance(AppearanceType tree = null, MCGameObject obj = null) : base(tree, obj)
        {
            appearType = (AppearanceType)tree;

            currentTexture = null;
            currentFrame = -1;
            currentRotation = 0;

            currentShapeTypeId = ActorStates.STATE_NORMAL;

        }

        #endregion

        #region Public Functions

        public List<ActorData> AppearStates
        {
            get {
                if (type.appearanceClass == AppearanceClass.VFX_APPEAR)
                {
                    return ((VFXAppearanceType)type).AppearStates;
                }else if (type.appearanceClass == AppearanceClass.BUILD_APPEAR)
                {
                    return ((BuildAppearanceType)type).AppearStates;
                }

                Debug.Log("No Class "+ type.appearanceClass.ToString());
                
                return null;
            }
        }

        public ActorData ActualState
        {
            get {
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
                    if (type.appearanceClass == AppearanceClass.VFX_APPEAR)
                    {
                        data = ((VFXAppearanceType)type).SpriteData;
                        AppData = new AppearanceData();
                        if (data == null)
                            return AppData;

                        currentTexture = data.MainText;
                        if (currentTexture != null)
                        {
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
                                
//                                if (state.delta != 0)
//                                {
//                                    Debug.Log("Aqui delta 1 :" +uvsFloor.Count);
////                                    Debug.Log(String.Join(", ",uvs.Take(4).Select(x=>"("+x.x +"-"+x.y+")")));
////                                    Debug.Log(String.Join(", ",uvsFloor.Take(4).Select(x=>"("+x.x +"-"+x.y+")")));
//                                }
                                
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
                                    numFrames = uvs.Count / 4
                                };
                            }
                            else
                            {
                                AppData = new AppearanceData()
                                {
                                    currentTexture = data.MainText,
                                    autoStart = false,
                                    vertices= data.Vertices["0-0"],
                                    uvs= data.uvs["0-0"],
                                    uvsFloor = new Vector2[0],
                                    fps = 1,
                                    isAnim = false,
                                    loop = false,
                                    numFrames = 1
                                };
                            }
                            
                        }
                        

                        return AppData;
                    } else
                    {
                        data = ((BuildAppearanceType)type).SpriteData;
                        AppData = new AppearanceData();
                        if (data == null)
                            return AppData;
                        
                        currentTexture = data.MainText;
                        if (currentTexture != null)
                        {
                            var state = this.ActualState;
                            
                            AppData = new AppearanceData()
                            {
                                currentTexture = data.MainText,
                                autoStart = false,
                                vertices = data.Vertices["12-0"],
                                uvs = data.uvs["12-0"],
                                uvsFloor = data.uvs.ContainsKey("0-0")? data.uvs["0-0"] : new Vector2[0],
                                fps = 1f,
                                isAnim = false,
                                loop = false,
                                numFrames = 1
                            }; 
                        }
                        return AppData;
                    }

                    
                }

                return new AppearanceData();

            }
        }
        #endregion
    }
}
