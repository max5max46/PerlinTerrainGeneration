using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshMaker : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private int dimX;
    [SerializeField] private int dimZ;

    [Header("Properties for First Perlin Layer")]
    [SerializeField] private float perlinScaleMain;
    [SerializeField] private float perlinRangeMain;

    [Header("Properties for Second Perlin Layer")]
    [SerializeField] private float perlinScaleSub;
    [SerializeField] private float perlinRangeSub;

    [Header("References")]
    [SerializeField] private GameObject verticePointPrefab;

    [Header("Debug")]
    [SerializeField] private bool canSeeVertices;

    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    private List<GameObject> verticePoints;

    // Start is called before the first frame update
    void Start()
    {
        verticePoints = new List<GameObject>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateMesh();

        UpdateMesh();
    }

    public void RegenerateMesh()
    {
        vertices = new Vector3[0];
        triangles = new int[0];
        colors = new Color[0];

        CreateMesh();

        if (canSeeVertices)
            ShowVertices();

        UpdateMesh();
    }

    public void ShowVertices(bool destroyVerticePoints = false)
    {
        if (verticePoints.Count > 0)
        {
            foreach (GameObject verticePoint in verticePoints)
                Destroy(verticePoint);

            verticePoints = new List<GameObject>();
        }

        if (!destroyVerticePoints)
        {
            foreach (Vector3 vertice in vertices)
            {
                GameObject verticePoint = Instantiate(verticePointPrefab, vertice, Quaternion.Euler(0, 0, 0));
                verticePoints.Add(verticePoint);
            }
        }
    }

    void CreateMesh()
    {

        // Calculates All Vertices
        vertices = new Vector3[dimX * dimZ];

        int index = 0;

        int offsetMain = Random.Range(0, 99999);
        int offsetSub = Random.Range(0, 99999);

        for (int z = 0; z < dimZ; z++)
            for (int x = 0; x < dimX; x++)
            {
                float perlinX = (float)x / dimX * perlinScaleMain + offsetMain;
                float perlinY = (float)z / dimZ * perlinScaleMain + offsetMain;

                vertices[index] = new Vector3(x, ((Mathf.PerlinNoise(perlinX, perlinY) * 2) - 1) * perlinRangeMain, z);

                perlinX = (float)x / dimX * perlinScaleSub + offsetSub;
                perlinY = (float)z / dimZ * perlinScaleSub + offsetSub;

                vertices[index] = new Vector3(x, vertices[index].y + ((Mathf.PerlinNoise(perlinX, perlinY) * 2) - 1) * perlinRangeSub, z);

                index++;
            }


        // Calculates All Triangles
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


        // Calculates All Colors
        colors = new Color[dimX * dimZ];

        for (int i = 0; i < vertices.Length; i++)
        {
            switch (vertices[i].y)
            {
                case var n when (n >= perlinRangeMain):
                    colors[i] = new Color(0, 0, 1);
                    break;

                case var n when (n < perlinRangeMain && n >= 0):
                    float red = 1 - (vertices[i].y / perlinRangeMain);
                    float blue = 1 - red;

                    colors[i] = new Color(red, 0, blue);
                    break;

                case var n when (n < 0 && n > -perlinRangeMain):
                    float red2 = 1 - (-vertices[i].y / perlinRangeMain);
                    float Green = 1 - red2;

                    colors[i] = new Color(red2, Green, 0);
                    break;

                case var n when (n <= -perlinRangeMain):
                    colors[i] = new Color(0, 1, 0);
                    break;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    public void ToggleVerticePoints()
    {
        if (!canSeeVertices)
        {
            canSeeVertices = true;
            ShowVertices();
        }
        else
        {
            canSeeVertices = false;
            ShowVertices(true);
        }
    }
}
