using MechCommanderUnity.MCG.GameObjectTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MechCommanderUnity.MCG.Appearances;
using System.Linq;

namespace MechCommanderUnity.MCG
{
    public class TerrainObjectUnity : BaseObjectUnity
    {
       
        public override ActorStates SelectedState
        {
            get { return ActualState.state; }
            set{
                if (ActualState.state == value)
                    return;

                if ((baseObject.objectType).subType == (int)ObjectTypes.TerrainObjectType.TerrainObjectSubType.TERROBJ_TREE)
                {
                    ((TreeAppearance)baseObject.appearance).currentShapeTypeId = value;

                    ActualState = ((TreeAppearance)baseObject.appearance).ActualState;
                } else
                {
                    ((BuildingAppearance)baseObject.appearance).currentShapeTypeId = value;

                    ActualState = ((BuildingAppearance)baseObject.appearance).ActualState;
                }
            }
        }

        public override void Start()
        {
            base.Start();
            NameSuffix = " (TerrObj)";
        }

        protected override void Update()
        {
            base.Update();
            
            if (!isSetup && baseObject != null)
            {
                this.InitObject();
            }
        }
        
        protected new void InitObject()
        { 
            base.InitObject();

            Data = baseObject.Appearance;

            if (Data != null && Data.vertices!=null)//&& Data.currentRect != null&& Data.currentPivot != null
            {
                if ((baseObject.objectType).subType == (int)ObjectTypes.TerrainObjectType.TerrainObjectSubType.TERROBJ_TREE)
                {
                    //  SelectedState = ActorStates.STATE_BLOWING_UP1;
                    ActualState = ((TreeAppearance)baseObject.appearance).ActualState;
                } else
                {
                    //  SelectedState = ActorStates.STATE_BLOWING_UP1;
                    ActualState = ((BuildingAppearance)baseObject.appearance).ActualState;
                }
                
                this.floorGo.SetActive((ActualState.delta == 1));

                isSetup = true;
                fps = ActualStateFramerate;
                
                Shader shader = Shader.Find("MechCommanderUnity/PaletteSwapLookup");
                Material material = new Material(shader);
                material.mainTexture = Data.currentTexture;
                material.SetTexture("_PaletteTex", PalTexture);
                
                meshRenderer.sortingLayerName = "Objs";
                meshRenderer.sortingOrder = baseSortingIndex+1;
                meshRenderer.sharedMaterial = material;
                
                floorMeshRenderer.sortingLayerName = "Objs";
                floorMeshRenderer.sortingOrder = baseSortingIndex;
                floorMeshRenderer.sharedMaterial = material;

                // Create mesh
                Mesh mesh = new Mesh();
                mesh.name = string.Format("sprite");
                
                // Indices
                int[] triangles = new int[6]
                    {
                                    0, 1, 2,
                                    3, 2, 1,
                    };

                // Normals
                Vector3 normal = Vector3.Normalize(Vector3.up + Vector3.forward);
                Vector3[] normals = new Vector3[4];
                normals[0] = normal;
                normals[1] = normal;
                normals[2] = normal;
                normals[3] = normal;

                mesh.vertices = Data.vertices.Take(4).ToArray();
                mesh.triangles = triangles;
                mesh.normals = normals;
                mesh.uv = Data.uvs.Take(4).ToArray();
                // Assign mesh
                meshFilter.sharedMesh = mesh;
                
                isPlaying= Data.isAnim && Data.numFrames>1;
                restartAnims = (Data.isAnim && Data.numFrames>1);
                
                if (ActualState.delta == 1 && Data.uvsFloor!=null && Data.uvsFloor.Length>3)
                {
//                    Debug.Log("Delta in terrain",this);
                    // Create mesh
                    Mesh floorMesh = new Mesh();
                    floorMesh.name = "spriteFloor";

                    floorMesh.vertices = Data.vertices.Take(4).ToArray();
                    floorMesh.triangles = triangles;
                    floorMesh.normals = normals;
                
                    floorMesh.uv = Data.uvsFloor.Take(4).ToArray();
                    // Assign mesh
                    floorMeshFilter.sharedMesh = floorMesh;
                }
                    
            } else
            {
//                MechCommanderUnity.LogMessage("Something null");
            }
        }
    }
}