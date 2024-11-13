using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshMaker : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private int dimX;
    [SerializeField] private int dimZ;

    [Header("References")]
    [SerializeField] private GameObject verticePointPrefab;

    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateMesh();
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateMesh()
    {
        vertices = new Vector3[dimX * dimZ];

        int index = 0;

        for (int z = 0; z < dimZ; z++)
            for (int x = 0; x < dimX; x++)
            {
                vertices[index] = new Vector3(x, 0, z);
                index++;
            }

        foreach (Vector3 vertice in vertices)
        {
            Instantiate(verticePointPrefab, vertice, Quaternion.Euler(0,0,0));
        }

        triangles = new int[((dimX - 1) * (dimZ - 1)) * 3 * 2];

        index = 0;

        for (int z = 0; z < dimZ - 1; z++)
            for (int x = 0; x < dimX - 1; x++)
            {
                triangles[index] = x + (z * dimX);
                index++;
                triangles[index] = x + ((z + 1) * dimX);
                index++;
                triangles[index] = (x + 1) + (z * dimX);
                index++;

                triangles[index] = (x + 1) + (z * dimX);
                index++;
                triangles[index] = x + ((z + 1) * dimX);
                index++;
                triangles[index] = (x + 1) + ((z + 1) * dimX);
                index++;
            }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
