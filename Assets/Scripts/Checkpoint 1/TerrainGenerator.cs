// using UnityEngine;

// public class TerrainGenerator : MonoBehaviour
// {
//     public int depth = 20;
//     public int width = 256;
//     public int height = 256;
//     public float scale = 20f;
//     public float offsetX = 100f;
//     public float offsetY = 100f;
//     public int octaves = 4;
//     public float persistence = 0.5f;
//     public float lacunarity = 2f;

//     public GameObject elementPrefab;
//     public float elementScale = 10f;
//     public float elementThreshold = 0.5f;

//     private void Start()
//     {
//         offsetX = Random.Range(0f, 99999f);
//         offsetY = Random.Range(0f, 99999f);
//     }

//     private void Update()
//     {
//         Terrain terrain = GetComponent<Terrain>();
//         terrain.terrainData = GenerateTerrain(terrain.terrainData);
//         AddMeshCollider(terrain);
//         GenerateElements(terrain);
//     }

//     private TerrainData GenerateTerrain( TerrainData terrainData)
//     {
//         terrainData.heightmapResolution = width + 1;
//         terrainData.size = new Vector3(width, depth, height);
//         terrainData.SetHeights(0, 0, GenerateHeights());
//         return terrainData;
//     }

//     private float[,] GenerateHeights()
//     {
//         float[,] heights = new float[width, height];

//         for (int x = 0; x < width; x++)
//         {
//             for (int y = 0; y < height; y++)
//             {
//                 heights[x, y] = CalculateHeight(x, y);
//             }
//         }

//         return heights;
//     }

//     private float CalculateHeight(int x, int y)
//     {
//         float amplitude = 1;
//         float frequency = 1;
//         float noiseHeight = 0;

//         for (int i = 0; i < octaves; i++)
//         {
//             float xCoord = (float)x / width * scale * frequency + offsetX;
//             float yCoord = (float)y / height * scale * frequency + offsetY;

//             float perlinValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
//             noiseHeight += perlinValue * amplitude;

//             amplitude *= persistence;
//             frequency *= lacunarity;
//         }

//         return noiseHeight;
//     }

//     private void AddMeshCollider(Terrain terrain)
//     {
//         TerrainCollider terrainCollider = terrain.gameObject.GetComponent<TerrainCollider>();
//         if (terrainCollider == null)
//         {
//             terrainCollider = terrain.gameObject.AddComponent<TerrainCollider>();
//         }
//         terrainCollider.terrainData = terrain.terrainData;
//     }

//     private void GenerateElements(Terrain terrain)
//     {
//         float[,] heights = terrain.terrainData.GetHeights(0, 0, width, height);

//         for (int x = 0; x < width; x++)
//         {
//             for (int z = 0; z < height; z++)
//             {
//                 float xCoord = (float)x / width * elementScale + offsetX;
//                 float zCoord = (float)z / height * elementScale + offsetY;
//                 float perlinValue = Mathf.PerlinNoise(xCoord, zCoord);

//                 if (perlinValue > elementThreshold)
//                 {
//                     float y = heights[x, z] * depth;
//                     Vector3 position = new Vector3(x, y, z);
//                     Vector3 worldPosition = terrain.transform.TransformPoint(position);
//                     Instantiate(elementPrefab, worldPosition, Quaternion.identity);
//                 }
//             }
//         }
//     }
// }
