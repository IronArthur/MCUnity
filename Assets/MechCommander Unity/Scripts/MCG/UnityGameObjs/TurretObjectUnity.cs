using System.Collections;
using System.Linq;
using MechCommanderUnity.MCG.Appearances;
using UnityEngine;

namespace MechCommanderUnity.MCG
{
    public class TurretObjectUnity: BaseObjectUnity
    {
        public float RotationUpdateInterval = 0;
        
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
        
        protected override IEnumerator Animate()
        {
            currentFrame = 0;
            while (true)
            {
                if (isSetup && isPlaying)
                {
                    base.UpdateModel();

                    currentFrame++;

                    if (currentFrame >= ActualState.numFrames)
                    {
                        currentFrame = 0;
                    }
                }


                yield return new WaitForSeconds(1f / fps);
            }
        }

        public override void Start()
        {
            base.Start();
            NameSuffix = " (TurretObj)";
        }

        protected override void Update()
        {
            base.Update();

            if (!isSetup && baseObject != null)
            {
                this.InitObject();

                if (isSetup)
                {
                    ActualGVState = ((GVAppearance) baseObject.appearance).ActualState;

                    if (ActualGVState.numRotations > 1)
                    {
                        StartCoroutine(TurretRotateRandom(ActualGVState.numRotations-1));
                    }
                }
            }
        }

        


        protected new void InitObject()
        {
            base.InitObject();

            Data = baseObject.Appearance;

            if (Data != null && Data.vertices != null)
            {
                ActualGVState = ((GVAppearance) baseObject.appearance).ActualState;

//                foreach (var state in ((GVAppearance) baseObject.appearance).AppearStates)
//                {
//                    Debug.Log(state.state +" "+ state.numFrames +" "+state.frameRate,this);
//                }

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
                    Debug.Log("Delta in Turret",this);
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

        private IEnumerator TurretRotateRandom(int NumberOfRotations)
        {
            yield return new WaitForSeconds(5f);
            RotationUpdateInterval = Random.Range(3f, 15f);
            while (true)
            {
                baseObject.rotation = Random.Range(0, NumberOfRotations);
//                Debug.Log($"Updating {this.Name} rotation to: {baseObject.rotation} with interval: {RotationUpdateInterval} ",this);
                UpdateObject();
                
                yield return new WaitForSeconds(RotationUpdateInterval);
            }
            
        }
        
        protected void UpdateObject()
        {
            Data = baseObject.Appearance;
            
            if (Data != null && meshFilter.sharedMesh!=null && Data.vertices != null)
            {
                ActualGVState = ((GVAppearance) baseObject.appearance).ActualState;
                fps = ActualStateFramerate;
                
                var mesh = meshFilter.sharedMesh;
                mesh.uv = Data.uvs.Take(4).ToArray();
                // Assign mesh
                meshFilter.sharedMesh = mesh;
                
                isPlaying= Data.isAnim && Data.numFrames>1;
                restartAnims = (Data.isAnim && Data.numFrames>1);
                
            }
            
        }
    }
}