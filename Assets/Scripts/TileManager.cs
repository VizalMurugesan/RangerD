
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;




public class TileManager : MonoBehaviour
{
   public Grid grid;

    [SerializeField] public int MapWidth;
    [SerializeField] public int MapHeight;
    [SerializeField] float PerlinMapScale;

    [SerializeField] float BushOccurence;
    [SerializeField] float WhiteFlowerOcurrence;
    [SerializeField] float YeloowFlowerOccurence;
    [SerializeField] float GrassOcurrence;

    [SerializeField] Tilemap BaseGrass;
    [SerializeField] Tilemap BaseGround;
    [SerializeField] Tilemap WhiteFlower;
    [SerializeField] Tilemap YellowFlower;
    [SerializeField] Tilemap path;
    

    [SerializeField] List<Tile> GrassTiles= new List<Tile>();
    [SerializeField] List<Tile> BushTiles = new List<Tile>();
    [SerializeField] List<Tile> WhiteFlowerTiles = new List<Tile>();
    [SerializeField] List<Tile> YellowFlowerTiles = new List<Tile>();
    [SerializeField] List<GameObject> Bushes = new List<GameObject>();
    [SerializeField] List<GameObject> GrassVegetation = new List<GameObject>();
    [SerializeField] Tile BaseGrassTile;

    public Transform BushesParent;
    float[,] HeightMap;
    void Start()
    {
        //GenerateBaseGrass(MapWidth,MapHeight);
        //GenerateBaseGround(MapWidth,MapHeight);
        grid = GetComponent<Grid>();
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
                if (heightMap[i, j] <= GrassOcurrence)
                    mapType[i, j] = "G";
                else if (heightMap[i, j] <= GrassOcurrence+ BushOccurence)
                    mapType[i, j] = "B";
                else if (heightMap[i, j] <= GrassOcurrence + BushOccurence + WhiteFlowerOcurrence)
                    mapType[i, j] = "WF";
                else
                    mapType[i, j] = "YF";

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
                    case ("WF"):
                        random = UnityEngine.Random.Range(0, WhiteFlowerTiles.Count);
                        WhiteFlower.SetTile(new Vector3Int(i, j, 0), WhiteFlowerTiles[random]);
                        break;
                    case ("YF"):
                        random = UnityEngine.Random.Range(0, YellowFlowerTiles.Count);
                        YellowFlower.SetTile(new Vector3Int(i, j, 0), YellowFlowerTiles[random]);
                        break;
                    default:
                        random = UnityEngine.Random.Range(0, GrassTiles.Count);
                        BaseGround.SetTile(new Vector3Int(i, j, 0), GrassTiles[0]);
                        break;
                }
            }
        }

    }

    public void GenerateBushes(int Width, int Height)
    {
        float random = Random.Range(0f, 1f);
        for (int i = 0; i < Width; i += 1)
        {
            for (int j = 0; j < Height; j++)
            {
                Vector3 Worldpos = grid.CellToWorld(new Vector3Int(i, j, 0));
                random = Random.Range(0f, 1f);
                if (random > 0.8f && !path.HasTile(new Vector3Int(i,j,0)))
                {
                    int randnum = Random.Range(0, Bushes.Count);
                    Instantiate(Bushes[randnum], Worldpos, Quaternion.identity, BushesParent);

                }
            }
        }
    }

    public void GenerateGrassVegetation(int Width, int Height)
    {
        float random = Random.Range(0f, 1f);
        for (int i = 0; i < Width; i += 1)
        {
            for (int j = 0; j < Height; j++)
            {
                Vector3 Worldpos = grid.CellToWorld(new Vector3Int(i, j, 0));
                random = Random.Range(0f, 1f);
                if (random > 0.6f && !path.HasTile(new Vector3Int(i, j, 0)))
                {
                    int randnum = Random.Range(0, GrassVegetation.Count);
                    Instantiate(GrassVegetation[randnum], Worldpos, Quaternion.identity, BushesParent);

                }
            }
        }
    }

    public void GenerateBaseGrass(int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                BaseGrass.SetTile(new Vector3Int(i, j, 0), BaseGrassTile);
            }
        }
    }

    public void ClearAllTiles()
    {
        BaseGrass.ClearAllTiles();
        BaseGround.ClearAllTiles();
        WhiteFlower.ClearAllTiles() ;
        YellowFlower.ClearAllTiles();
    }

    public void DestroyVegetation()
    {
        int count = BushesParent.childCount;
        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(BushesParent.GetChild(i).gameObject);
        }
    }
}
