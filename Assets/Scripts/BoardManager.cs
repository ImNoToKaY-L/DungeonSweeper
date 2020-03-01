using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class BoardManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int rows = 30;
    public int columns = 30;
    public GameObject[] floorTiles;
    public GameObject[] enemyTiles;
    public GameObject[] wallTiles;
    public GameObject[] itemTiles;
    public GameObject[] playerTile;
    public Player player;
    Transform boardHolder;

    private const int ENEMY_TILE = 0;
    private const int ITEM_TILE = 1;

    public List<Vector3> obstacles;
    //public List<Vector3> hasUnits = new List<Vector3>();
    public int maxWallSpawnCount = 40;
    public Dictionary<Vector3, GameObject> UnitMap = new Dictionary<Vector3, GameObject>();

    private int hardLevel = 1;
    public void SetupScene()
    {
        //if (GameObject.Find("Player")!=null)
        //{
        //    GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        //    foreach (var item in playerList)
        //    {
        //        item.SetActive(false);
        //        Destroy(item);
        //    }
        //}

        if (GameObject.Find("Board")!=null)
        {
            GameObject[] boardList = GameObject.FindGameObjectsWithTag("board");
            Debug.Log("Board is not null");

            foreach (var item in boardList)
            {
                item.SetActive(false);
                Destroy(item);
            }

            Debug.Log("Board successfully destroyed");
            obstacles.Clear();
            //hasUnits.Clear();
            UnitMap.Clear();
        }



        boardHolder = new GameObject("Board").transform;
        boardHolder.tag = "board";

        Instantiate(playerTile[0], new Vector3(0, 0, 0f), Quaternion.identity).transform.SetParent(boardHolder);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        HardLevelModify();


        for (int x = 0; x < columns-1; x++)
        {
            for (int y = 0; y < rows-1; y++)
            {
                GameObject toInstantiate = floorTiles[0];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
        RandomMapGenerating();


        //Instantiate(playerTile[0], new Vector3(0, 0, 0f), Quaternion.identity).transform.SetParent(boardHolder);
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //RandomMapGenerating();

        GameObject.FindGameObjectWithTag("ModifierInfo").GetComponent<Text>().text = "Map size: " + rows + "*" + columns + "\n" + "Player fov: " + player.PlayerFOV+"\n"
               +"Player detection: "+player.DistanceDetection + "\n"+"Max wall spawn: "+maxWallSpawnCount;



    }


    private void HardLevelModify()
    {
        if (hardLevel!=1)
        {

            if (hardLevel<=3)
            {
                player.PlayerFOV = 2;
                player.DistanceDetection = 10;
                double dHardLevel = Convert.ToDouble(hardLevel);

                double multiplier = (((dHardLevel / 10) * RandomNumber(1, 3)) + 1);
                rows = Convert.ToInt32(rows * multiplier);
                columns = Convert.ToInt32(columns * (((dHardLevel / 10) * RandomNumber(1, 3)) + 1));
                maxWallSpawnCount = Convert.ToInt32(maxWallSpawnCount * ((dHardLevel / 3) + 1));
            }
            else
            {
                player.PlayerFOV = 1;
                player.DistanceDetection = 20;
            }

        }
        else
        {
            rows = 30;
            columns = 30;
            maxWallSpawnCount = 40;
            player.PlayerFOV = 2;
            player.DistanceDetection = 20;
        }
    }



    private int RandomNumber(int lowerbound, int upperbound)
    {
        return UnityEngine.Random.Range(lowerbound,upperbound);
    }


    public void NextLevel()
    {
        if (hardLevel<=5)
        {
            hardLevel++;

        }
    }

    public void ResetLevel()
    {
        hardLevel = 1;
    }

    private void RandomMapGenerating()
    {
        for (int i = 0; i < maxWallSpawnCount; i++)
        {
            int xORy = RandomNumber(0, 2);//0 stands for X, 1 stands for Y
            int length = RandomNumber(1, 4+hardLevel);
            Vector3 startPos = new Vector3(RandomNumber(1, Convert.ToInt32(columns * 0.8) ), RandomNumber(1, Convert.ToInt32(rows*0.8)), 0f);
            for (int j = 1; j <= length; j++)
            {
                Vector3 candidate;
                if (xORy == 0)
                    candidate = startPos + new Vector3(j, 0, 0);
                else
                    candidate = startPos + new Vector3(0, j, 0);

                if (!GridOccupied(candidate))
                {
                    InstantiateObstacles(wallTiles[0], candidate);
                }               
            }

        }
        UnitGenerating(enemyTiles[0], ENEMY_TILE);
        UnitGenerating(itemTiles[0], ITEM_TILE);
        UnitGenerating(itemTiles[1], ITEM_TILE);
        UnitGenerating(itemTiles[2], ITEM_TILE);
        //Instantiate(itemTiles[0], new Vector3(1, 0, 0), Quaternion.identity).transform.SetParent(boardHolder);
        //Instantiate(itemTiles[1], new Vector3(0, 1, 0), Quaternion.identity).transform.SetParent(boardHolder);
        //Instantiate(itemTiles[2], new Vector3(1, 1, 0), Quaternion.identity).transform.SetParent(boardHolder);
        //hasUnits.Add(new Vector3(1, 0, 0));
        //hasUnits.Add(new Vector3(0, 1, 0));
        //hasUnits.Add(new Vector3(1, 1, 0));





    }

    private void InstantiateObstacles(GameObject tile, Vector3 position)
    {

        obstacles.Add(position);
        Instantiate(tile, position, Quaternion.identity).transform.SetParent(boardHolder);
    }

    private bool GridOccupied(Vector3 grid)
    {
        return obstacles.Contains(grid) || UnitMap.ContainsKey(grid);
    }

    private void UnitGenerating(GameObject Tile,int tileType)
    {
        bool unitSpawned = false;
        while (!unitSpawned)
        {
            GameObject unit;
            Vector3 candidate = new Vector3(RandomNumber(1, columns-2), RandomNumber(1, rows-2), 0);
            if (!GridOccupied(candidate)&&PathCheck.AStarSearchPath(candidate,obstacles))
            {
                unitSpawned = true;
                unit = Instantiate(Tile, candidate, Quaternion.identity);
                unit.transform.SetParent(boardHolder);
                if (tileType!=ENEMY_TILE)
                {
                    //hasUnits.Add(candidate);
                    UnitMap.Add(candidate, unit);
                    unit.SetActive(false);

                }
            }
        }
    }
}


