
using System.Linq;
using UnityEngine;

public class HiddenEdgeWireframe : MonoBehaviour
{
    public Material mat;

    void Start()
    {
        Mesh oldMesh = GetComponent<MeshFilter>().mesh;

        Mesh newMesh = new Mesh();

        newMesh.vertices = oldMesh.vertices;

        int[] oldTris = oldMesh.triangles;
        int[] indexes = new int[oldTris.Length * 2];

        for (int i = 0, a = 0; i < oldTris.Length; i += 3)
        {
            indexes[a++] = oldTris[i];
            indexes[a++] = oldTris[i + 1];
            indexes[a++] = oldTris[i + 1];
            indexes[a++] = oldTris[i + 2];
            indexes[a++] = oldTris[i + 2];
            indexes[a++] = oldTris[i];
        }

        newMesh.SetIndices(indexes, MeshTopology.Lines, 0);

        GameObject newGo = new GameObject();
        newGo.transform.position = transform.position;
        MeshFilter newMeshFilter = newGo.AddComponent<MeshFilter>();
        newMeshFilter.mesh = newMesh;
        newGo.AddComponent<MeshRenderer>();
        newGo.transform.parent = transform;

        AddMaterial();
    }

    void AddMaterial()
    {
        GetComponent<MeshRenderer>().sharedMaterial = mat;
    }
}

