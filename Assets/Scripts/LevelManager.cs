using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public enum Tile
{
    FLOOR,
    WALL,
    CRATE,
    WORKER,
    FINISH,
    EMPTY
}

public enum Direction
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}

public class LevelManager : MonoBehaviour
{
    public GameObject floor;
    public GameObject wall;
    public GameObject crate;
    public GameObject worker;
    public GameObject finish;
    public string filename;

    public TMPro.TextMeshProUGUI message; 
    public TMPro.TextMeshProUGUI scoreText; 

    private int Width;
    private int Depth;
    private Tile[,] map;
    private Tile[,] originalMap;
    private Vector2Int workerPos = new Vector2Int();
    private int score = 100;

    // Start is called before the first frame update
    void Start()
    {
        message.text = "Welcome!";
        scoreText.text = score.ToString();
        ReadFromFile();
        Draw();
    }

    public void Draw()
    {
       GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
       foreach(GameObject go in tiles)
       {
           Destroy(go);
       }

       for(int x = 0; x < Width; x++)
       {
           for(int z = 0; z < Depth; z++)
           {
                if(map[x,z] == Tile.WALL)
                {
                    Instantiate(wall, new Vector3(x, 0f, z), Quaternion.identity);
                }
                else if(map[x,z] == Tile.FLOOR)
                {
                    Instantiate(floor, new Vector3(x, 0f, z), Quaternion.identity);
                }
                else if(map[x,z] == Tile.CRATE)
                {
                    Instantiate(crate, new Vector3(x, 0f, z), Quaternion.identity);
                }
                else if(map[x,z] == Tile.WORKER)
                {
                    Instantiate(worker, new Vector3(x, 0f, z), Quaternion.identity);
                }
                else if(map[x,z] == Tile.FINISH)
                {
                    Instantiate(finish, new Vector3(x, 0f, z), Quaternion.identity);
                }
           }
       }

    }

    public void ReadFromFile()
    {
       string[] temp =  File.ReadAllLines("Assets//" + filename);
       Width = temp[0].Length;
       Depth = temp.Length;

       map = new Tile[Width, Depth];
       originalMap = new Tile[Width, Depth];
        

       for(int x = 0; x < Width; x++)
       {
           int d = Depth - 1;

           for(int z = 0; z < Depth; z++)
           {
                char c = temp[d--][x];

                switch(c)
                {
                    case 'W': map[x, z] = Tile.WALL; originalMap[x, z] = Tile.WALL; break;
                    case 'F': map[x, z] = Tile.FLOOR; originalMap[x, z] = Tile.FLOOR; break;
                    case 'C': map[x, z] = Tile.CRATE; break;
                    case 'I': map[x, z] = Tile.FINISH; originalMap[x, z] = Tile.FINISH; break;
                    case 'H':
                        {
                            workerPos.x = x;
                            workerPos.y = z;
                            map[x, z] = Tile.WORKER; break;
                        }
                    default : map[x, z] = Tile.EMPTY; originalMap[x, z] = Tile.EMPTY; break;
                }
           }
       }


    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.W))
       {
            Move(Direction.NORTH);
       }
       else if(Input.GetKeyDown(KeyCode.S))
       {
            Move(Direction.SOUTH);
       }
       else if(Input.GetKeyDown(KeyCode.A))
       {
            Move(Direction.WEST);
       }
       else if(Input.GetKeyDown(KeyCode.D))
       {
            Move(Direction.EAST);
       }

       scoreText.text = score.ToString();
    }

    void Move(Direction d)
    {
        Vector2Int newPos = workerPos;
        Vector2Int cratePos = workerPos;

        switch(d)
        {
            case Direction.NORTH: newPos.y++; cratePos.y += 2;  break;
            case Direction.EAST: newPos.x++; cratePos.x += 2;  break;
            case Direction.SOUTH: newPos.y--; cratePos.y -= 2;  break;
            case Direction.WEST: newPos.x--; cratePos.x -= 2;  break;
        }

        if (Check(d, 1, Tile.FLOOR) || Check(d, 1, Tile.FINISH)) // Check if we can move into a floor space
        {
            map[workerPos.x, workerPos.y] = originalMap[workerPos.x, workerPos.y];
            map[newPos.x, newPos.y] = Tile.WORKER;
            workerPos = newPos;
            score--;
        }
        else if (Check(d, 1, Tile.CRATE)) // Check if we can move into a cratespace
        {
            if(Check(d, 2, Tile.FLOOR) || Check(d, 2, Tile.FINISH)) // Check if we have an open spot behind the crate
            {
                map[workerPos.x, workerPos.y] = originalMap[workerPos.x, workerPos.y];
                map[newPos.x, newPos.y] = Tile.WORKER;
                map[cratePos.x, cratePos.y] = Tile.CRATE;
                workerPos = newPos;
                score--;
            }
        }

        if(hasWon())
        {
            message.text = "You WON!";
        }
        Draw();
    }

    bool Check(Direction dir, int distance, Tile check)
    {
        bool tileMatches = false;

        switch(dir)
        {
            case Direction.NORTH: tileMatches = map[workerPos.x, workerPos.y + distance] == check ? true : false; break;
            case Direction.EAST: tileMatches = map[workerPos.x + distance, workerPos.y] == check ? true : false; break;
            case Direction.SOUTH: tileMatches = map[workerPos.x, workerPos.y - distance] == check ? true : false; break;
            case Direction.WEST: tileMatches = map[workerPos.x - distance, workerPos.y] == check ? true : false; break;
        }

        return tileMatches;
    }

    bool hasWon()
    {
       int numFinish = 0;

       for(int x = 0; x < Width; x++)
       {
           for(int z = 0; z < Depth; z++)
           {
               if(map[x,z] == Tile.FINISH)
               {  
                    numFinish++;
               }
           }
       }

       return numFinish == 0;
    }

}
