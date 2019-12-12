using System;
using System.Collections;
using System.Linq;
using MechCommanderUnity.MCG.Appearances;
using MechCommanderUnity.MCG.GameObjectTypes;
using UnityEngine;

namespace MechCommanderUnity.MCG
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class BaseObjectUnity : MonoBehaviour
    {
        
        public MCGameObject baseObject { get; set; }

        public Texture2D PalTexture { get; set; }

        protected MeshRenderer meshRenderer;
        protected MeshFilter meshFilter;
        
        protected GameObject floorGo;
        protected MeshRenderer floorMeshRenderer;
        protected MeshFilter floorMeshFilter;
        
        protected Appearance.AppearanceData Data;

        public string Name = "";

        protected string NameSuffix = "";

        public bool isSetup = false;

        public bool isPlaying = true;

        public bool restartAnims = false;

        public int currentFrame = 0;

        [HideInInspector]
        public int baseSortingIndex = 0;
        
        public float fps;

        public Vector2[] currentUvs;

        public ActorData ActualState;

        public virtual ActorStates SelectedState
        {
            get { return ActualState.state; }
            set{}
        }

        protected virtual float ActualStateFramerate
        {
            get { return ActualState.frameRate; }
        }
        
        protected delegate void ChangeState(float _damage);
        protected static event ChangeState OnChangeState;

        protected IEnumerator AnimateCoroutine;

        // Use this for initialization
        public virtual void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();

            floorGo = transform.GetChild(0).gameObject;
            
            floorMeshRenderer = floorGo.GetComponent<MeshRenderer>();
            floorMeshFilter = floorGo.GetComponent<MeshFilter>();
        }
        
        // Update is called once per frame
        protected virtual void Update()
        {
            if (restartAnims)
            {
//                Debug.Log("Restarting Anims on : "+this.Name,this);
                if (AnimateCoroutine != null)
                    StopCoroutine(AnimateCoroutine);

                AnimateCoroutine = Animate();
                StartCoroutine(AnimateCoroutine);
                restartAnims = false;
            }
        }




        protected void InitObject()
        {
//            MechCommanderUnity.LogMessage("Init Object "+Name);
            
            if (Name == "" && baseObject != null)
            {
                Name = baseObject.Name;
                this.transform.parent.name = Name + NameSuffix;
            }
            
            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();

        }
        
        protected virtual IEnumerator Animate()
        {
           // fps = ActualStateFramerate;
            
            currentFrame = 0;
            while (true)
            {
                if (isSetup && isPlaying)
                {
                    UpdateModel();

                    currentFrame++;

                    if (currentFrame >= ActualState.numFrames)
                    {
                        currentFrame = 0;
                    }
                }


                yield return new WaitForSeconds(1f / fps);
            }
        }

        protected void UpdateModel()
        {
            if (Data?.uvs != null)
            {
                meshFilter.sharedMesh.uv = Data.uvs.Skip(currentFrame*4).Take(4).ToArray();
                currentUvs = meshFilter.sharedMesh.uv;
            }
        }

        protected void OnDestroy()
        {
            this.ResetComponent();
        }

        protected void OnDisable()
        {
            restartAnims = true;
        }
        
        
        
        [EditorButton]
        public void ResetComponent()
        {
            if (AnimateCoroutine!=null)
                StopCoroutine(AnimateCoroutine);

            baseObject = null;
            PalTexture = null;
            Name = "";
            isSetup = false;
            restartAnims = true;
        }
    }
}