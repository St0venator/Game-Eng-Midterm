using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.UIElements;


public class marcher1 : MonoBehaviour
{

    public int width = 10;
    public int height = 10;
    public float noiseScale = 1.0f;
    public float noiseDensity = 1.0f;
    public float noiseCutoff = 1.0f;
    public bool draw;

    public enum terrainType
    {
        Caves,
        RollingHills,
        ModularHills,
        Pit
    }

    public terrainType type;

    public float floorScale;
    private float Xoffset;
    private float Zoffset;

    public float noiseLayers;

    float[,,] vals;

    private List<Vector3> verts = new List<Vector3>();
    private List<int> tris = new List<int>();

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    
    // Start is called before the first frame update
    void Start()
    {
        

        meshFilter = GetComponent<MeshFilter>();

        Xoffset = Random.Range(width, 100000 + width);
        Zoffset = Random.Range(width, 100000 + width);

        vals = Noise();
        march();

        setMesh();

        gameObject.AddComponent<MeshCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void march()
    {
        verts.Clear();
        tris.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    float[] corners = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        corners[i] = vals[corner.x, corner.y, corner.z];
                    }
                    cube(new Vector3(x, y, z), corners);
                }
            }
        }
    }

    private void cube(Vector3 pos, float[] corners)
    {

        int index = getConfigIndex(corners);

        if(index == 0 || index == 255)
        {
            return;
        }

        int edgeIndex = 0;

        for(int E = 0; E < 5; E++)
        {
            for(int M = 0; M < 3; M++)
            {
                int triVal = MarchingTable.Triangles[index, edgeIndex];

                if(triVal == -1)
                {
                    return;
                }

                Vector3 edgeStart = pos + MarchingTable.Edges[triVal, 0];
                Vector3 edgeEnd = pos + MarchingTable.Edges[triVal, 1];

                //Vector3 vert = (edgeStart + edgeEnd) / 2f;

                Vector3 vert = Vector3.Lerp(edgeStart, edgeEnd, (noiseCutoff - corners[getEdgeIndex(MarchingTable.Edges[triVal, 0])]) / (corners[getEdgeIndex(MarchingTable.Edges[triVal, 1])] - corners[getEdgeIndex(MarchingTable.Edges[triVal, 0])]));

                verts.Add(vert);
                tris.Add(verts.Count - 1);

                edgeIndex++;
            }
        }

    }

    private int getEdgeIndex(Vector3 pos)
    {
        for(int i = 0; i < MarchingTable.Corners.Length; i++)
        {
            if(pos == MarchingTable.Corners[i])
            {
                return i;
            }
        }

        return default;
    }

    private int getConfigIndex(float[] corners)
    {
        int index = 0;

        for (int i = 0; i < 8; i++)
        {
            if (corners[i] > noiseCutoff)
            {
                index |= 1 << i;
            }
        }
        //Debug.Log(index);
        return index;
    }

    private void setMesh()
    {
        Mesh mesh = new Mesh();

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private float[,,] Noise()
    {
        float divAmnt = 1f;
        float[,,] vals = new float[width + 1, height + 1, width + 1];
        for(int x = 0; x < width + 1; x++)
        {
            for(int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    float noiseX = x + Xoffset;
                    float noiseZ = z + Zoffset;

                    float heightVal = 0;

                    if(type == terrainType.Caves)
                    {
                        heightVal = Vector3.Distance(new Vector3(noiseX, y, noiseZ), new Vector3(noiseX, 0, noiseZ)) / (float)height;
                        heightVal += PerlinNoise3D(noiseX, y, noiseZ) * noiseDensity;

                        if (y < floorScale)
                        {
                            heightVal = 0f;
                        }
                    }
                    
                    if(type == terrainType.RollingHills)
                    {

                        float floor = floorScale;
                        
                        for(int i = 1; i <= noiseLayers; i++)
                        {
                            float floorCont = 0f;
                            floorCont += (noiseDensity * i) * Mathf.Sin(noiseX / (noiseScale * i));
                            floorCont += (noiseDensity * i) * Mathf.Sin(noiseZ / (noiseScale * i));
                            floorCont += (noiseDensity * i) * Mathf.Sin((noiseX + noiseZ) / 2f / (noiseScale * i));
                            floorCont /= 3f;

                            floor += floorCont;
                        }
                        //floor = Mathf.Abs(floor);
                        if(y < floor)
                        {
                            heightVal = y / floor;
                        }
                        else
                        {
                            heightVal = y;
                        }
                    }

                    if (type == terrainType.ModularHills)
                    {

                        float floor = floorScale;
                        //floor *= floorScale;

                        /*
                        for (int i = 1; i <= noiseLayers; i++)
                        {
                            float floorCont = 0f;
                            floorCont += (noiseDensity * i) * Mathf.Sin(noiseX / (noiseScale * i));
                            floorCont += (noiseDensity * i) * Mathf.Sin(noiseZ / (noiseScale * i));
                            floorCont /= 2f;

                            floor += floorCont;
                        }
                        */


                        //floor = Mathf.Abs(floor);
                        if (y < floor)
                        {
                            heightVal = y / floor;
                        }
                        else
                        {
                            heightVal = y;
                        }

                    }

                    //heightVal = (heightVal - 0.5f) * 2f;

                    //float heightVal = Vector3.Distance(new Vector3(x, y, z), new Vector3(width / 2, height / 2, width / 2));

                    if(y > floorScale + ((2f * Mathf.Sin(x / 8f)) + (2f * Mathf.Cos(z / 8f))) / 2f)
                    {
                        divAmnt = 0;
                        for(int i = 1; i <= noiseLayers; i++)
                        {
                            heightVal += PerlinNoise3D(noiseX * Mathf.Pow(2, i), y * Mathf.Pow(2, i), noiseZ * Mathf.Pow(2, i)) * (noiseDensity / Mathf.Pow(2, i));
                            divAmnt += noiseDensity / (float)i;
                        }
                        
                    }



                    vals[x, y, z] = heightVal;
                }
            }
        }

        return vals;
    }


    private void OnDrawGizmos()
    {

        if (draw)
        {
            vals = Noise();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        float colorVal = vals[x, y, z];
                        Gizmos.color = new Color(colorVal, colorVal, colorVal);

                        if (colorVal <= noiseCutoff)
                        {
                            if (draw)
                            {
                                Gizmos.DrawSphere(new Vector3(x, y, z), 0.1f);
                            }

                        }

                    }
                }
            }
        }
        
    }

    public float PerlinNoise3D(float x, float y, float z)
    {
        x /= 10;
        y /= 10;
        z /= 10;


        x *= noiseScale;
        y *= noiseScale;
        z *= noiseScale;


        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        float product = (xy + xz + yz + yx + zx + zy) / 6;

        return (product - 0.5f) * 2f;
    }



}
