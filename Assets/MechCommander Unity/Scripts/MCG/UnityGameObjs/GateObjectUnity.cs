using System.Linq;
using MechCommanderUnity.MCG.Appearances;
using UnityEngine;

namespace MechCommanderUnity.MCG
{
    public class GateObjectUnity: BaseObjectUnity

    {
        public GVActorData ActualGVState;
        
        public new GVActorStates SelectedState
        {
            get { return ActualGVState.state; }
            set{
                if (ActualGVState.state == value)
                    return;
                
                ((GVAppearance)baseObject.appearance).currentShapeTypeId = value;

                ActualGVState = ((GVAppearance)baseObject.appearance).ActualState;
            }
        }
        
        protected override float ActualStateFramerate
        {
            get { return ActualGVState.frameRate; }
        }
        
        
        public override void Start()
        {
            base.Start();
            NameSuffix = " (GateObj)";
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

            if (Data != null && Data.vertices != null)
            {
                ActualGVState = ((GVAppearance) baseObject.appearance).ActualState;

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
                    Debug.Log("Delta in GATE",this);
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
            }
            
        }
    }
}