using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Transform boardHolder;

    private const int ENEMY_TILE = 0;
    private const int ITEM_TILE = 1;

    public List<Vector3> obstacles;
    public List<Vector3> hasUnits = new List<Vector3>();
    private int hardLevel = 1;
    public void SetupScene()
    {
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
        Instantiate(playerTile[0], new Vector3(0, 0, 0f), Quaternion.identity);

        RandomMapGenerating();


    }

    private void RandomMapGenerating()
    {
        int maxWallSpawnCount = 40;

        for (int i = 0; i < maxWallSpawnCount; i++)
        {
            int xORy = Random.Range(0, 2);//0 stands for X, 1 stands for Y
            int length = Random.Range(1, 6);
            Vector3 startPos = new Vector3(Random.Range(1, 23), Random.Range(1, 23), 0f);
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
            Vector3 candidate = new Vector3(Random.Range(1, 28), Random.Range(1, 28), 0);
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


