using System;
using System.Linq;
using MechCommanderUnity.MCG.Appearances;
using UnityEngine;

namespace MechCommanderUnity.MCG
{
    public class BuildingObjectUnity: BaseObjectUnity
    {
        public override ActorStates SelectedState
        {
            set{
                if (ActualState.state == value)
                    return;

                ((BuildingAppearance)baseObject.appearance).currentShapeTypeId = value;
                ActualState = ((BuildingAppearance)baseObject.appearance).ActualState;
            }
        }
        
        public override void Start()
        {
            base.Start();
            NameSuffix = " (BuildObj)";
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
                SelectedState = ((BuildingAppearance)baseObject.appearance).currentShapeTypeId;
                //  SelectedState = ActorStates.STATE_BLOWING_UP1;

                if (SelectedState == null)
                {
                    Debug.Log("No State",this);
                    return ;
                }
                
                var states = ((BuildingAppearance) baseObject.appearance).AppearStates;

                if (states == null || states.Count == 0)
                {
                    Debug.Log("No List States " +baseObject.appearance.GetType().ToString(),this);
                    return ;
                }

                ActualState = states.Where(x => x.state == SelectedState).First();

                if (ActualState.numFrames != (Data.uvs.Length / 4))
                {
                    Debug.LogError("Error no match Numframes vs numberofUVs "+ActualState.numFrames +">"+(Data.uvs.Length / 4),this);
                }
                
                this.floorGo.SetActive((ActualState.delta == 1));
                
                isSetup = true;
                
                fps = ActualStateFramerate;

                Shader shader = Shader.Find("MechCommanderUnity/PaletteSwapLookup");
                Material material = new Material(shader);
                material.SetTexture("_PaletteTex", PalTexture);
                
                material.mainTexture = Data.currentTexture;
                
                
                
                meshRenderer.sortingLayerName = "Objs";
                meshRenderer.sortingOrder = baseSortingIndex+1;
                meshRenderer.sharedMaterial = material;

                floorMeshRenderer.sortingLayerName = "Objs";
                floorMeshRenderer.sortingOrder = baseSortingIndex;
                floorMeshRenderer.sharedMaterial = material;
                
                // Create mesh
                Mesh mesh = new Mesh();
                mesh.name = "sprite";
                
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