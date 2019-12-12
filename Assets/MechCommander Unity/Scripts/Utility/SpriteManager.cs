using MechCommanderUnity.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace MechCommanderUnity.Utility
{
    public class SpriteManager : MonoBehaviour
    {
        #region Class Variables
        private static SpriteManager s_Instance = null;
        private Queue m_SpriteQueue = new Queue();
        [SerializeField]
        private bool CoroutineIsWorking = false;
        public bool LimitQueueProcesing = false;
        public float QueueProcessTime = 0.0f;

        public int TaskPending = 0;

        Dictionary<int, SpriteData> Sprites = new Dictionary<int, SpriteData>();

        #endregion

        #region Class Structures
        public class SpriteData
        {
            public Texture2D MainText;
            public Dictionary<string, Rect> Rects;
            public Dictionary<string, Vector2> Pivots;
            public Dictionary<string, Vector3[]> Vertices;
            public Dictionary<string, Vector2[]> uvs;

        }


        #endregion

        #region Constructors
        #endregion

        #region Public Functions

        // override so we don't have the typecast the object
        public static SpriteManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = GameObject.FindObjectOfType(typeof(SpriteManager)) as SpriteManager;
                }
                return s_Instance;
            }
        }

        //Inserts the event into the current queue.
        public bool QueueSpriteCreate(int pakId, List<int> ShpIds)
        {
            if (!Sprites.ContainsKey(pakId))
            {
                string data = pakId + "-" + string.Join(",", ShpIds.Select(x => x.ToString()).ToArray());
                m_SpriteQueue.Enqueue(data);
            }

            return true;
        }

        public SpriteData GetSpriteData(int pakId)
        {

            if (Sprites.ContainsKey(pakId))
            {
                return Sprites[pakId];
            }

            return null;
        }

        #endregion

        private void CreateSpriteTask(int pakId, List<int> ShpIds)
        {
            if (!Sprites.ContainsKey(pakId))
            {

                var reader = MechCommanderUnity.Instance.ContentReader;
//                Debug.Log("SpriteTaks: " + pakId + "-" + string.Join(",", ShpIds.Select(x => x.ToString()).ToArray()));

//                MechCommanderUnity.LogMessage("Sprite Job Start: " + pakId);

                List<MCBitmap> lstBmps;
                if (reader.GetShapes(pakId, out lstBmps, ShpIds))
                {
                    Dictionary<string, Vector2> pivots = new Dictionary<string, Vector2>();

                    for (int j = 0; j < lstBmps.Count; j++)
                    {
                        pivots.Add(lstBmps[j].Name, lstBmps[j].Pivot);
                    }
                    
                    MCBitmap atlas;
                    Dictionary<string, Rect> dict;
                    ImageProcessing.CreateAtlas(lstBmps.ToArray(), out atlas, out dict);
                    for (int i = 0; i < lstBmps.Count; i++)
                    {
                        lstBmps[i].Dispose();
                    }

                    Dictionary<string, Vector3[]> vertices = new Dictionary<string, Vector3[]>();
                    Dictionary<string, Vector2[]> uvs = new Dictionary<string, Vector2[]>();
                    foreach (var rect in dict)
                    {

                        Vector3[] verticesLocal = new Vector3[4];
                        var halfXOV1 = (rect.Value.width / 100) * pivots[rect.Key].x;
                        var halfXOV2 = (rect.Value.width / 100) * (1 - pivots[rect.Key].x);
                        var heightOV1 = (rect.Value.height / 100) * (1 - pivots[rect.Key].y);
                        var heightOV2 = (rect.Value.height / 100) * pivots[rect.Key].y;

                        verticesLocal[0] = new Vector3(-halfXOV1, heightOV1, 0); //top-left
                        verticesLocal[1] = new Vector3(halfXOV2, heightOV1, 0); //top-right
                        verticesLocal[2] = new Vector3(-halfXOV1, -heightOV2, 0); //bottom-left
                        verticesLocal[3] = new Vector3(halfXOV2, -heightOV2, 0); //bottom-right

                        Vector2[] uvsLocal = new Vector2[4];
                        uvsLocal[0] = new Vector2(rect.Value.x / atlas.Width, rect.Value.yMax / atlas.Height);
                        uvsLocal[1] = new Vector2(rect.Value.xMax / atlas.Width, rect.Value.yMax / atlas.Height);
                        uvsLocal[2] = new Vector2(rect.Value.x / atlas.Width, rect.Value.y / atlas.Height);
                        uvsLocal[3] = new Vector2(rect.Value.xMax / atlas.Width, rect.Value.y / atlas.Height);

                        vertices.Add(rect.Key, verticesLocal);
                        uvs.Add(rect.Key, uvsLocal);
                    }
                    
                    var MainText = ImageProcessing.MakeIndexedTexture2D(atlas);
                    atlas.Dispose();
                    var spriteData = new SpriteData()
                    {
                        MainText = MainText,
                        Rects = dict,
                        Pivots = pivots,
                        Vertices= vertices,
                        uvs= uvs
                    };

                    Sprites.Add(pakId, spriteData);
                    //MechCommanderUnity.LogMessage("Sprite Job Finish: " + pakId);

                } else
                {
                    Debug.LogError("Fatal error recovering Shape data");
                }
            }
        }

        //Every update cycle the queue is processed, if the queue processing is limited,
        //a maximum processing time per update can be set after which the events will have
        //to be processed next update loop.
        void Update()
        {
            if (!CoroutineIsWorking && m_SpriteQueue.Count > 0)
            {
                CoroutineIsWorking = true;
                StartCoroutine(ProcessQueue());
            }
            
            TaskPending = m_SpriteQueue.Count;
        }

        private IEnumerator ProcessQueue()
        {
            
            while (m_SpriteQueue.Count > 0)
            {
                var datastr = m_SpriteQueue.Dequeue() as string;

//                Debug.Log("Data to process " +datastr);
                
                var data = (datastr).Split('-');

                if (data.Length != 2)
                {
                    Debug.LogError(data.Length);
                    continue;
                }

                int pakId = int.Parse(data[0]);

                var ids = data[1].Split(',');

                if (ids.Length < 1)
                {
                    Debug.LogError(data[1]);
                    continue;
                }
                var ShpIds = ids.Select(x => int.Parse(x)).ToList();

                CreateSpriteTask(pakId, ShpIds);

                if (LimitQueueProcesing)
                {
                    yield return new WaitForSeconds(QueueProcessTime);
                }else
                    yield return null;
            }

            CoroutineIsWorking = false;
            yield return null;
        }

        public void OnApplicationQuit()
        {
            //RemoveAll();
            //m_eventQueue.Clear();
            s_Instance = null;
        }
    }
}
