using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellMain : MonoBehaviour
{
    //Block Atlas
    public Dictionary<string, int> atlas;
    //Block Storage
    public Dictionary<string, List<List<int>>> blocks;
    public int[,,] map;
    public int[,,] resMap;
    //Physical map mesh
    public GameObject mapHolder;
    //NeighborhoodStyles
    public int neighborhoodStyle;
    public List<List<List<int>>> neighborhoods;
    public List<List<int>> vonNeumann;
    public List<List<int>> moore;
    public int mapX;
    public int mapY;
    public int mapZ;
    public int fillPercent;
    public bool smoothBool;
    public int smoothBias;
    public int smoothAmt;
    public GameObject[] blockAtlas;
    public int refreshRate;
    public int smoothCnt;

    // Start is called before the first frame update
    void Start()
    {
        ResetMapData();
        InitCellOS();
        InitMap();
        FillMap();
        for (int i = 0; i < smoothAmt; i++) {
            smoothMap();
        }
        seedGrass();
        VisMap();
    }

    // the event for pressing the generation button in the editor
    public void GenButtonPress()
    {
        ResetMapData();
        InitCellOS();
        InitMap();
        FillMap();
        //GenBorder();
        VisMap();
    }    

    public void NewMapHolder() {
        GameObject.DestroyImmediate(mapHolder);
        mapHolder = new GameObject
        {
            name = "Map"
        };
    }

    

    public void nextGeneration(bool isSmooth) {
        if (isSmooth) {
            smoothMap();
        }
    }

    // resets the map
    public void ResetMapData() {
        blocks = null;
        map = null;
        resMap = null;
        GameObject.DestroyImmediate(mapHolder);
    }

    //add cell to storage
    public void AddCell(int x, int y, int z, string val) {
        //update the map array map[x,y]
        atlas.TryGetValue(val, out map[x, y, z]);
        //update dictionary
        List<int> cell = new List<int>
        {
            x,
            y,
            z
        };
        blocks[val].Add(cell);
    }

    public void clearBlocksList() {
        blocks = null;
        InitBlocks();
    }

    //remove cell from storage
    public void removeCell(int x, int y, int z, string val) {
        map[x, y, z] = 0;
        foreach (List<int> elem in blocks[val]) {
            if (elem[0] == x && elem[1] == y) {
                Debug.Log(elem);
                blocks[val].Remove(elem);
            }
        }
    }

    public void seedGrass() {
        foreach (string elem in blocks.Keys)
        {
            if (elem == "dirt")
            {
                foreach (List<int> listVal in blocks[elem])
                {
                    if (map[listVal[0], listVal[1] + 1, listVal[2]] == 0)
                    {
                        resMap[listVal[0], listVal[1], listVal[2]] = 3;
                    }
                    else {
                        resMap[listVal[0], listVal[1], listVal[2]] = 2;
                    }
                }
            }
        }
        clearBlocksList();
        pushTempToMap();
        NewMapHolder();
    }

    public void smoothMap()
    {
        for (int x = 1; x < mapX - 1; x++){
            for (int y = 1; y < mapY - 1; y++){
                for (int z = 1; z < mapZ - 1; z++)
                {
                    int neighbors = 0;
                    foreach (List<int> elem in neighborhoods[neighborhoodStyle])
                    {
                        if (map[x + elem[0], y + elem[1], z + elem[2]] != 0)
                        {
                            neighbors++;
                        }
                    }
                    if (neighbors > smoothBias)
                    {
                        resMap[x, y, z] = 2;
                    }
                    else
                    {
                        resMap[x, y, z] = 0;
                    }
                }
            }
        }
        smoothCnt++;
        clearBlocksList();
        pushTempToMap();
        NewMapHolder();
    }

    public void pushTempToMap()
    {
        for (int x = 1; x < mapX - 1; x++){
            for (int y = 1; y < mapY - 1; y++){
                for (int z = 0; z < mapZ - 1; z++)
                {
                    map[x, y, z] = resMap[x, y, z];
                    if (map[x, y, z] != 0)
                    {
                        if (map[x, y, z] == 2)
                        {
                            AddCell(x, y, z, "dirt");
                        }
                        else if (map[x, y, z] == 3) {
                            AddCell(x, y, z, "grass");
                        }
                    }
                }
            }
        }
    }

    // Generates a border for the map
    public void GenBorder() {
        for (int x = 0; x < mapX; x++) {
            for (int y = 0; y < mapY; y++) {
                for (int z = 0; z < mapZ; z++)
                {
                    if (x == 0 || y == 0 || x == mapX - 1 || y == mapY - 1 || z == 0 || z == mapZ - 1)
                    {
                        AddCell(x, y, z, "border");
                    }
                }
            }
        }
    }

    //Randomly fill map based on fill percentage
    public void FillMap() {
        for (int x = 1; x < mapX - 1; x++) {
            for (int y = 1; y < mapY - 1; y++) {
                for (int z = 1; z < mapZ - 1; z++)
                {
                    int tempRand = Random.Range(0, 100);
                    if (tempRand < fillPercent)
                    {
                        AddCell(x, y, z, "dirt");
                    }
                }
            }
        }
    }

    // neighborHood indexer
    public void setNeighborhoodIndex(int index) {
        neighborhoodStyle = index;
    }

    //create mesh for individual tile
    public void DrawTile(int x, int y, int z, string val) {
        GameObject clone = Instantiate(blockAtlas[atlas[val]], new Vector3(x - (mapX/2), y - (mapY/2), z - (mapZ/2)), Quaternion.Euler(-90,0,0)) as GameObject;
        clone.transform.parent = mapHolder.transform;
        
    }

    //generate tiles based on storage
    public void VisMap() {
        foreach (string elem in blocks.Keys) {
            if (elem != "air") {
                foreach (List<int> listVal in blocks[elem]) {
                    DrawTile(listVal[0], listVal[1], listVal[2], elem);
                }
            }
        }
    }

    //Main
    public void InitCellOS()
    {
        if (blocks == null || map == null)
        {
            smoothCnt = 0;
            InitAtlas();
            InitBlocks();
            InitNeighborhoods();
        }
    }

    //For Defining Blocks to avoid magic nums
    public void InitAtlas()
    {
        atlas = new Dictionary<string, int>
        {
            { "air", 0 },
            { "border", 1 },
            { "dirt", 2},
            { "grass", 3 }
        };
    }

    //initialize blocks structure
    public void InitBlocks()
    {
        blocks = new Dictionary<string, List<List<int>>>();
        foreach (string elem in atlas.Keys)
        {
            blocks.Add(elem, new List<List<int>>());
        }
    }

    //initialize map data
    public void InitMap()
    {
        map = new int[mapX, mapY, mapZ];
        resMap = new int[mapX, mapY, mapZ];
        NewMapHolder();
        for (int x = 0; x < mapX; x++)
        {
            for (int y = 0; y < mapY; y++)
            {
                for (int z = 0; z < mapZ; z++)
                {
                    atlas.TryGetValue("air", out map[x, y, z]);
                }
            }
        }
    }

    //initialize ca neighborhoods
    public void InitNeighborhoods()
    {
        //neighborhood x - y format
        vonNeumann = new List<List<int>> {
            //left
            new List<int>{
                -1,0,0
            },
            //down
            new List<int>{
                0,-1,0
            },
            //right
            new List<int>{
                1,0,0
            },
            //up
            new List<int>{
                0,1,0
            },
            //forward
            new List<int>{
                0,0,1
            },
            //back
            new List<int>{
                0,0,-1
            }
        };
        moore = new List<List<int>>
        {
             //left down back
            new List<int>{
                -1,-1,-1
            },
            // left down
            new List<int>{
                -1,-1,0
            },
            //left down forward
            new List<int>{
                -1,-1,1
            },
            //down back
            new List<int>{
                0,-1,-1
            },
            //down 
            new List<int>{
                0,-1,0
            },
            //down forward
            new List<int>{
                0,-1,1
            },
             //right down back
            new List<int>{
                1,-1,-1
            },
            // right down
            new List<int>{
                1,-1,0
            },
            //right down forward
            new List<int>{
                1,-1,1
            },
            //left back
            new List<int>{
                -1,0,-1
            },
            //left 
            new List<int>{
                -1,0,0
            },
            //left forward
            new List<int>{
                -1,0,1
            },
            //back
            new List<int>{
                0,0,-1
            },
            //forward
            new List<int>{
                0,0,1
            }, 
            //right back
            new List<int>{
                1,0,-1
            },
            // right 
            new List<int>{
                1,0,0
            },
            //right  forward
            new List<int>{
                1,0,1
            },
             //left up back
            new List<int>{
                -1,1,-1
            },
            // left up
            new List<int>{
                -1,1,0
            },
            //left up forward
            new List<int>{
                -1,1,1
            },
            //up back
            new List<int>{
                0,1,-1
            },
            //up 
            new List<int>{
                0,1,0
            },
            //up forward
            new List<int>{
                0,1,1
            },
             //right up back
            new List<int>{
                1,1,-1
            },
            // right up
            new List<int>{
                1,1,0
            },
            //right up forward
            new List<int>{
                1,1,1
            },
        };
        neighborhoods = new List<List<List<int>>> {
            vonNeumann,
            moore
        };
    }
}
