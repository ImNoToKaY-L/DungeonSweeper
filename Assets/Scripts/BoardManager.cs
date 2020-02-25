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
    private Player player;
    Transform boardHolder;

    private const int ENEMY_TILE = 0;
    private const int ITEM_TILE = 1;

    public List<Vector3> obstacles;
    public List<Vector3> hasUnits = new List<Vector3>();
    public int maxWallSpawnCount = 40;

    private int hardLevel = 1;
    public void SetupScene()
    {
        if (GameObject.Find("Board")!=null)
        {
            Destroy(GameObject.Find("Board"));
            obstacles.Clear();
            hasUnits.Clear();
        }

        boardHolder = new GameObject("Board").transform;
        for (int x = 0; x < columns-1; x++)
        {
            for (int y = 0; y < rows-1; y++)
            {
                GameObject toInstantiate = floorTiles[0];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }


        Instantiate(playerTile[0], new Vector3(0, 0, 0f), Quaternion.identity).transform.SetParent(boardHolder);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        RandomMapGenerating();

        GameObject.FindGameObjectWithTag("ModifierInfo").GetComponent<Text>().text = "Map size: " + rows + "*" + columns + "\n" + "Player fov: " + player.PlayerFOV+"\n"
               +"Player detection: "+player.DistanceDetection;



    }


    private int RandomNumber(int lowerbound, int upperbound)
    {
        return UnityEngine.Random.Range(lowerbound,upperbound);
    }


    private void RandomMapGenerating()
    {
        for (int i = 0; i < maxWallSpawnCount; i++)
        {
            int xORy = RandomNumber(0, 2);//0 stands for X, 1 stands for Y
            int length = RandomNumber(1, 6);
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



    }

    private void InstantiateObstacles(GameObject tile, Vector3 position)
    {

        obstacles.Add(position);
        Instantiate(tile, position, Quaternion.identity).transform.SetParent(boardHolder);
    }

    private bool GridOccupied(Vector3 grid)
    {
        return obstacles.Contains(grid) || hasUnits.Contains(grid);
    }

    private void UnitGenerating(GameObject Tile,int tileType)
    {
        bool unitSpawned = false;
        while (!unitSpawned)
        {
            Vector3 candidate = new Vector3(RandomNumber(1, columns-2), RandomNumber(1, rows-2), 0);
            if (!GridOccupied(candidate)&&PathCheck.AStarSearchPath(candidate,obstacles))
            {
                unitSpawned = true;
                Instantiate(Tile, candidate, Quaternion.identity).transform.SetParent(boardHolder);
                if (tileType!=ENEMY_TILE)
                {
                    hasUnits.Add(candidate);

                }
            }
        }
    }
}


