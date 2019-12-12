using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class MeshCombiner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.CombineMeshes();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void CombineMeshes()
    {
        var _meshesAreCombined = true;

        Matrix4x4 myTransform = transform.worldToLocalMatrix;
        Dictionary<Material, List<CombineInstance>> combines = new Dictionary<Material, List<CombineInstance>>();
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        Debug.Log(meshRenderers.Length);
        return;

        foreach (var meshRenderer in meshRenderers)
        {
            foreach (var material in meshRenderer.sharedMaterials)
            {
                if (material != null && !combines.ContainsKey(material))
                {
                    combines.Add(material, new List<CombineInstance>());
                }
            }
        }

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        foreach (var filter in meshFilters)
        {
            if (filter.sharedMesh == null)
                continue;
            CombineInstance ci = new CombineInstance();
            ci.mesh = filter.sharedMesh;
            ci.transform = myTransform * filter.transform.localToWorldMatrix;
            combines[filter.GetComponent<MeshRenderer>().sharedMaterial].Add(ci);
            filter.GetComponent<MeshRenderer>().enabled = false;
        }

        foreach (var m in combines.Keys)
        {
            var go = new GameObject("Combined Mesh");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            var filter = go.AddComponent<MeshFilter>();
            filter.mesh.CombineMeshes(combines[m].ToArray(), true, true);
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.material = m;
            go.AddComponent<NetworkIdentity>();
            


        }

    }
}
