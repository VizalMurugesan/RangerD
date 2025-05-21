
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;




public class TileManager : MonoBehaviour
{
    [SerializeField] int MapWidth;
    [SerializeField] int MapHeight;
    [SerializeField] float PerlinMapScale;

    [SerializeField] Tilemap BaseGrass;
    [SerializeField] Tilemap BaseGround;
    

    [SerializeField] List<Tile> GrassTiles= new List<Tile>();
    [SerializeField] List<Tile> BushTiles = new List<Tile>();
    [SerializeField] List<Tile> FlowerTiles = new List<Tile>();
    [SerializeField] Tile BaseGrassTile;

    float[,] HeightMap;
    void Start()
    {
        GenerateBaseGrass(MapWidth,MapHeight);
        GenerateBaseGround(MapWidth,MapHeight);
    }

    private float[,] GenerateHeightMap(int width, int height)
    {
        float[,] heightMap = new float[width, height];
        for (int i = 0; i < width; i+= 1)
        {
            for (int j = 0; j < height; j++)
            {
                heightMap[i,j]= Mathf.PerlinNoise((float)i/PerlinMapScale,(float)j/PerlinMapScale);
            }
        }
        return heightMap;
    }
    private string[,] AssignTerrainTypes(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        string[,] mapType = new string[width, height];

        for (int i = 0; i < width; i += 1)
        {
            for (int j = 0; j < height; j++)
            {
                if (heightMap[i, j] <= 0.2f)
                    mapType[i, j] = "G";
                else if (heightMap[i, j] <= 0.6f)
                    mapType[i, j] = "B";
                else
                    mapType[i, j] = "F";

            }
        }

        return mapType;
    }

    public void GenerateBaseGround(int width, int height)
    {
        HeightMap = GenerateHeightMap(width, height);
        string[,] terrainMap = AssignTerrainTypes(HeightMap);
        int Width = terrainMap.GetLength(0);
        int Height = terrainMap.GetLength(1);

        for (int i = 0; i < Width; i += 1)
        {

            for (int j = 0; j < Height; j++)
            {
                string terrainType = terrainMap[i, j];
                int random = 0;

                switch (terrainType)
                {
                    case ("G"):
                        random = UnityEngine.Random.Range(0, GrassTiles.Count);
                        BaseGround.SetTile(new Vector3Int(i,j,0),GrassTiles[random]);
                        break;
                    case ("B"):
                        random = UnityEngine.Random.Range(0, BushTiles.Count);
                        BaseGround.SetTile(new Vector3Int(i, j, 0), BushTiles[random]);
                        break;
                    case ("F"):
                        random = UnityEngine.Random.Range(0, FlowerTiles.Count);
                        BaseGround.SetTile(new Vector3Int(i, j, 0), FlowerTiles[random]);
                        break;
                    default:
                        random = UnityEngine.Random.Range(0, GrassTiles.Count);
                        BaseGround.SetTile(new Vector3Int(i, j, 0), GrassTiles[0]);
                        break;
                }
            }
        }

    }

    private void GenerateBaseGrass(int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                BaseGrass.SetTile(new Vector3Int(i, j, 0), BaseGrassTile);
            }
        }
    }
}
