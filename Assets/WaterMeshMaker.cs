using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterMeshMaker : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private int dimX;
    [SerializeField] private int dimZ;
    [SerializeField] private float waveSpeed;
    public float waterHeight;
    [SerializeField] private Color upperColor;
    [SerializeField] private Color lowerColor;

    [Header("Properties for First Perlin Layer")]
    [SerializeField] private float perlinScaleMain;
    [SerializeField] private float perlinRangeMain;

    [Header("Properties for Second Perlin Layer")]
    [SerializeField] private float perlinScaleSub;
    [SerializeField] private float perlinRangeSub;

    [Header("References")]
    [SerializeField] private Slider heightSlider;
    [SerializeField] private Slider waveSpeedSlider;

    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    private int offsetMain;
    private int offsetSub;

    private double perlinOffsetScroll;

    bool showWater;


    // Start is called before the first frame update
    void Start()
    {
        heightSlider.value = waterHeight;
        waveSpeedSlider.value = waveSpeed;

        showWater = true;

        int offsetMain = Random.Range(0, 99999);
        int offsetSub = Random.Range(0, 99999);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateMesh();
        UpdateMesh();
    }

    private void Update()
    {
        waterHeight = heightSlider.value;
        waveSpeed = waveSpeedSlider.value;

        if (showWater)
        {
            ScrollMesh();
            GenerateColors();

            perlinOffsetScroll += waveSpeed * Time.deltaTime;

            UpdateMesh();
        }
    }

    public void RegenerateMesh()
    {
        vertices = new Vector3[0];
        triangles = new int[0];
        colors = new Color[0];

        CreateMesh();

        UpdateMesh();
    }

    void CreateMesh()
    {

        // Calculates All Vertices
        vertices = new Vector3[dimX * dimZ];

        int index = 0;

        for (int z = 0; z < dimZ; z++)
            for (int x = 0; x < dimX; x++)
            {
                float perlinX = (float)x / dimX * perlinScaleMain + offsetMain;
                float perlinY = (float)z / dimZ * perlinScaleMain + offsetMain;

                vertices[index] = new Vector3(x, (((Mathf.PerlinNoise(perlinX, perlinY) * 2) - 1) * perlinRangeMain) + waterHeight, z);

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

        GenerateColors();
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

    void GenerateColors()
    {
        

        // Calculates All Colors
        colors = new Color[dimX * dimZ];

        for (int i = 0; i < vertices.Length; i++)
        {
            float y = (vertices[i].y - waterHeight) / perlinRangeMain;

            switch (y)
            {
                case var n when (n >= 1):
                    colors[i] = upperColor;
                    break;

                case var n when (n < 1 && n > -1):
                    
                    float percent = 1 - ((y + 1) / 2);

                    float red = upperColor.r + percent * (lowerColor.r - upperColor.r);
                    float green = upperColor.g + percent * (lowerColor.g - upperColor.g);
                    float blue = upperColor.b + percent * (lowerColor.b - upperColor.b);
                    float alpha = upperColor.a + percent * (lowerColor.a - upperColor.a);

                    colors[i] = new Color(red, green, blue, alpha);
                    break;

                case var n when (n <= -1):
                    colors[i] = lowerColor;
                    break;
            }
        }
    }

    void ScrollMesh()
    {
        int index = 0;

        for (int z = 0; z < dimZ; z++)
            for (int x = 0; x < dimX; x++)
            {
                float perlinX = (float)x / dimX * perlinScaleMain + (offsetMain + (float)perlinOffsetScroll);
                float perlinY = (float)z / dimZ * perlinScaleMain + (offsetMain + (float)perlinOffsetScroll);

                vertices[index] = new Vector3(x, (((Mathf.PerlinNoise(perlinX, perlinY) * 2) - 1) * perlinRangeMain) + waterHeight, z);

                perlinX = (float)x / dimX * perlinScaleSub + offsetSub;
                perlinY = (float)z / dimZ * perlinScaleSub + offsetSub;

                vertices[index] = new Vector3(x, vertices[index].y + ((Mathf.PerlinNoise(perlinX, perlinY) * 2) - 1) * perlinRangeSub, z);

                index++;
            }
    }

    public void ToggleVisible()
    {
        if (!showWater)
        {
            showWater = true;
            GetComponent<MeshRenderer>().enabled = true;
            heightSlider.interactable = true;
            waveSpeedSlider.interactable = true;
        }
        else
        {
            showWater = false;
            GetComponent<MeshRenderer>().enabled = false;
            heightSlider.interactable = false;
            waveSpeedSlider.interactable = false;
        }
    }
}
