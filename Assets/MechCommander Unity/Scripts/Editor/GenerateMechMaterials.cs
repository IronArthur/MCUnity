using UnityEngine;
using UnityEditor;

using System.IO;

using System.Collections;
using System.Collections.Generic;

using MechCommanderUnity.API;

public class GenerateMechMaterials : MonoBehaviour
{
    private static List<IEnumerator> Coroutines;
    private static void Update()
    {
        if (Coroutines.Count > 0)
        {
            IEnumerator Coroutine = Coroutines[Coroutines.Count - 1];
            if (Coroutine.MoveNext())
            {
                if (Coroutine.Current is IEnumerator)
                    Coroutines.Add(Coroutine.Current as IEnumerator);
            } else
            {
                Coroutines.RemoveAt(Coroutines.Count - 1);
            }
        } else
        {
            EditorApplication.update -= Update;
        }
    }

    #region Menu Items
    [ExecuteInEditMode]
    [MenuItem("Assets/MCUnity/Create Materials")]
    static void CreateMaterials()
    {
        Debug.Log("Loading: ");

        Coroutines = new List<IEnumerator>();
        Coroutines.Add(CreateMaterialLibrary());

        EditorApplication.update += Update;


        //  StartCoroutine(Init());
    }

    #endregion
    [ExecuteInEditMode]
    static IEnumerator CreateMaterialLibrary()
    {
        Debug.Log("Coruotine: ");

        var pal = new MCPalette(@"HB.pal");

        var pak = new PakFile(@"04-Cougar.PAK");

        //   AssetDatabase.StartAssetEditing();

        float progressBar = 0.0f;
        EditorUtility.DisplayProgressBar("Creating Textures",
                                                  "Working...",
                                                  progressBar);

        for (int index = 65; index < 400; index++)
        {
            try
            {
                //var index = 65;


                var shpdata = pak.GetFileInner(index);

                if (shpdata == null)
                    continue;

                var shp = new ShpFile(shpdata);

                var bitmaps = shp.GetBitMaps();
                if (bitmaps.Length == 0)
                    continue;

                List<Texture2D> lstTextures = new List<Texture2D>();

                foreach (var bmp in bitmaps)
                {
                    MCSize sz;
                    Color32[] albedoColors = MCBitmap.GetColor32(bmp, pal, 0, 0, out sz);

                    // Debug.Log(bitmaps.Length);

                    Texture2D albedoMap = null;
                    albedoMap = new Texture2D(sz.Width, sz.Height, TextureFormat.RGBA32, false);
                    albedoMap.SetPixels32(albedoColors);
                    albedoMap.alphaIsTransparency = true;
                    albedoMap.Apply(true, false);

                    lstTextures.Add(albedoMap);

                }

                Texture2D atlas = new Texture2D(1, 1,TextureFormat.ARGB32,false);

                var rects = atlas.PackTextures(lstTextures.ToArray(), 0);
                atlas.alphaIsTransparency = true;
                atlas.filterMode = FilterMode.Point;
                atlas.wrapMode = TextureWrapMode.Clamp;

                atlas.Apply();
                byte[] bytes = (atlas as Texture2D).EncodeToPNG();

                // For testing purposes, also write to a file in the project folder
                File.WriteAllBytes(Application.dataPath + "./Sprites/Tests/" + "04-Cougar.PAK-" + index + ".png", bytes);

                Shader shader = Shader.Find("Sprites/Default");
                Material material = new Material(shader);

                //material.SetFloat("_Mode", (int)0);
                //material.SetFloat("_SmoothnessTextureChannel", (int)1);
                //material.SetFloat("_Metallic", 0);
                //material.SetFloat("_Glossiness", 1);
                //material.SetOverrideTag("RenderType", "TransparentCutout");
                //material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                //material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                //material.SetInt("_ZWrite", 1);
                //material.EnableKeyword("_ALPHATEST_ON");
                //material.DisableKeyword("_ALPHABLEND_ON");
                //material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                //material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;


                material.name = "testmaterial";
                material.mainTexture = atlas;

                material.mainTexture.filterMode = FilterMode.Point;
                //material.mainTextureOffset = new Vector2(rects[0].x, rects[0].y);
                //material.mainTextureScale = new Vector2(rects[0].width, rects[0].height);


                var go = new GameObject();
                var rend = go.AddComponent<MeshRenderer>();
                var meshFilter = go.AddComponent<MeshFilter>();
                rend.material = material;
                rend.sharedMaterial = material;
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                rend.receiveShadows = false;
                rend.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                rend.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

                // Vertices for a 1x1 unit quad
                // This is scaled to correct size depending on facing and orientation
                float hx = 0.5f, hy = 0.5f;
                Vector3[] vertices = new Vector3[4];

                vertices[0] = new Vector3(-hx, hy, 0);
                vertices[1] = new Vector3(hx, hy, 0);
                vertices[2] = new Vector3(-hx, -hy, 0);
                vertices[3] = new Vector3(hx, -hy, 0);

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

                // Create mesh
                Mesh mesh = new Mesh();
                mesh.name = string.Format("MobileEnemyMesh");
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.normals = normals;

                Vector2[] uvs = new Vector2[4];
                var rect = rects[0];
                uvs[0] = new Vector2(rect.x, rect.yMax);
                uvs[1] = new Vector2(rect.xMax, rect.yMax);
                uvs[2] = new Vector2(rect.x, rect.y);
                uvs[3] = new Vector2(rect.xMax, rect.y);

                // Assign mesh
                meshFilter.sharedMesh = mesh;
                meshFilter.sharedMesh.uv = uvs;
                //  AssetDatabase.CreateAsset(material, "Assets/Sprites/Tests/" + "04-Cougar.PAK-" + index + ".mat");
                //AssetDatabase.SaveAssets();
                //AssetDatabase.Refresh();
                //  DestroyImmediate(atlas);
                //foreach (var text in lstTextures)
                //{
                //    DestroyImmediate(text);
                //}



            } catch (System.Exception e)
            {
                Debug.Log(e.Message);
                EditorUtility.ClearProgressBar();
                throw;
            }

            progressBar = (float)(index / 4f);

            Debug.Log(progressBar);
            if ((index % 12f) == 0)
            {
                Debug.Log("Update ->" + progressBar);
                EditorUtility.DisplayProgressBar("Creating Textures",
                                                                  "Working...",
                                                                  progressBar);
            }



            yield return true;

            break;
        }

        //   AssetDatabase.SaveAssets();
        //  AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();
        yield return true;
        Debug.Log("Coruotine2: ");
        yield return null;
    }

}
