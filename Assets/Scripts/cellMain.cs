using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellMain : MonoBehaviour
{
    //Block Atlas
    public Dictionary<string, int> atlas;
    //Block Storage
    private Dictionary<string, List<List<int>>> blocks;
    private int[,,] map;
    private int[,,] resMap;
    //Physical map mesh Container
    private GameObject mapHolder;
    //NeighborhoodStyles and Atlas
    [Range(0,1)]
    public int neighborhoodStyle;
    private List<List<List<int>>> neighborhoods;
    private List<List<int>> vonNeumann;
    private List<List<int>> moore;
    //Public map Variables are so they are visible in the editor so i can tweak generation
    //Map Size
    public int mapX;
    public int mapY;
    public int mapZ;
    //Map Density
    public int mapDensity;
    //Map Smoothness
    public bool smoothBool;
    public int smoothBias;
    public int smoothAmt;
    private int smoothCnt;
    //Block References for prefabs
    public GameObject[] blockAtlas;
    //Truncated
    public int refreshRate;
    

    // Start is called before the first frame update
    void Start()
    {
        //clear old map if it exists
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

    //Creates a container for instantianted gameobject clones
    public void NewMapHolder() {
        GameObject.DestroyImmediate(mapHolder);
        mapHolder = new GameObject
        {
            name = "Map"
        };
    }

    //Steps the simulation one tick
    public void nextGeneration(bool isSmooth) {
        if (isSmooth) {
            smoothMap();
        }
    }

    // resets the local map and block list
    public void ResetMapData() {
        blocks = null;
        map = null;
        resMap = null;
        GameObject.DestroyImmediate(mapHolder);
    }

    //add cell to storage
    public void AddCell(int x, int y, int z, string val) {
        //update the map[x,y,z] to val
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

    //Clears the current blocklist 
    public void clearBlocksList() {
        blocks = null;
        InitBlocks();
    }

    //remove cell from map and block list.. reset it to air or 0
    public void removeCell(int x, int y, int z, string val) {
        map[x, y, z] = 0;
        foreach (List<int> elem in blocks[val]) {
            if (elem[0] == x && elem[1] == y) {
                Debug.Log(elem);
                blocks[val].Remove(elem);
            }
        }
    }

    // A wonderful way to grow grass
    // A world without grass would be.. a world without grass
    public void seedGrass() {
        foreach (string elem in blocks.Keys)
        {
            if (elem == "dirt")
            {
                foreach (List<int> listVal in blocks[elem])
                {
                    // if the block above is air... make this dirt block grass
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
        //update map
        updateMap();
    }

    // This function updates the block list and map[x,y,z] values after a calculation is done
    public void updateMap() {
        clearBlocksList();
        pushTempToMap();
        NewMapHolder();
    }

    // Smoothing algorithm for forming coherent noise structures
    // Otherwise the map would look like 3 dimensional static
    public void smoothMap()
    {
        for (int x = 1; x < mapX - 1; x++){
            for (int y = 1; y < mapY - 1; y++){
                for (int z = 1; z < mapZ - 1; z++)
                {
                    int neighbors = 0;
                    // Check neighborhood for neighbors
                    foreach (List<int> elem in neighborhoods[neighborhoodStyle])
                    {
                        if (map[x + elem[0], y + elem[1], z + elem[2]] != 0)
                        {
                            neighbors++;
                        }
                    }
                    //Conditionally change the block type
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
        //track smooth iteration and update map and values
        smoothCnt++;
        updateMap();
    }

    // for tranfering res map to map so i can ensure true Cellular Automaton generations
    // that don't interfere with one another. Ahhhhhhh.. True Science. 
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

    // Generates a border for the map via border blocks
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

    //Randomly fill map with noise biased towards mapDensity
    // Essentially controls map density
    public void FillMap() {
        for (int x = 1; x < mapX - 1; x++) {
            for (int y = 1; y < mapY - 1; y++) {
                for (int z = 1; z < mapZ - 1; z++)
                {
                    int tempRand = Random.Range(0, 100);
                    if (tempRand < mapDensity)
                    {
                        AddCell(x, y, z, "dirt");
                    }
                }
            }
        }
    }

    // neighborHood indexer 
    // for choosing between von neumann and moore neighborhood styles
    public void setNeighborhoodIndex(int index) {
        neighborhoodStyle = index;
    }

    //function to generate a specific cube at a specific location
    public void DrawTile(int x, int y, int z, string val) {
        GameObject clone = Instantiate(blockAtlas[atlas[val]], new Vector3(x - (mapX/2), y - (mapY/2), z - (mapZ/2)), Quaternion.Euler(-90,0,0)) as GameObject;
        clone.transform.parent = mapHolder.transform;
        
    }

    //generate cubes based on map values
    // takes blocklist values and instantiates block prefabs based off of euclidian coordinates
    public void VisMap() {
        foreach (string elem in blocks.Keys) {
            if (elem != "air") {
                foreach (List<int> listVal in blocks[elem]) {
                    DrawTile(listVal[0], listVal[1], listVal[2], elem);
                }
            }
        }
    }

    //For initializing the map and map data
    public void InitCellOS()
    {
        if (blocks == null || map == null)
        {
            smoothCnt = 0;
            // create block atlas
            InitAtlas();
            // create block list based on atlas
            InitBlocks();
            // initialize neighborhood rules
            InitNeighborhoods();
        }
    }

    //For Defining Blocks to avoid magic nums, atlas creation
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

    //initialize blocks structure based on the block atlas
    public void InitBlocks()
    {
        blocks = new Dictionary<string, List<List<int>>>();
        foreach (string elem in atlas.Keys)
        {
            blocks.Add(elem, new List<List<int>>());
        }
    }

    //initialize map data create an ( x * y * z ) sized map filled with air
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

    //initialize Cellular Automaton neighborhoods from relative euclidan position [x,y,z]
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
