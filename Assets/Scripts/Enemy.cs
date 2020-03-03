using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    const int CHASING = 0;
    const int COOPERATING = 1;
    const int INTERCEPTING = 2;
    private int state;
    private bool initiating = true;
    private Dictionary<Vector3, int> search = new Dictionary<Vector3, int>();
    private Dictionary<Vector3, int> cost = new Dictionary<Vector3, int>();
    private Dictionary<Vector3, Vector3> pathSave = new Dictionary<Vector3, Vector3>();
    private List<Vector3> hadSearch = new List<Vector3>();
    private List<Vector3> obstacle;
    public GameObject[] pathPainting;
    Transform Path;
    //The grid that this unit will go to in the next movement
    private Vector3 targetGrid;
    bool skipMove = true;
    bool playerSpotted = false;
    Vector3 playerPreviousPos;
    Vector3 playerCurrentPos;
    Vector3 playerOffset;
    Transform player;

    void Start()
    {
        if (initiating)
        {
            Destroy(GameObject.Find("PathPainting"));

            Path = new GameObject("PathPainting").transform;
            initiating = false;
            startPos = this.transform.position;
            //endPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            obstacle = GameManager.instance.boardScript.obstacles;
            GameManager.instance.AddEnemyToList(this);
            player = GameObject.FindGameObjectWithTag("Player").transform;
            state = CHASING;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    void StateJudging()
    {

        Vector3 currentPos = this.transform.position;
        Vector3 playerPos = GameManager.instance.boardScript.player.transform.position;
        Vector3 nearestItemPos;


    }
    
    public void MoveEnemy()
    {
        //GameObject[] paintedGrid = GameObject.FindGameObjectsWithTag("Soda");
        //foreach (var item in paintedGrid)
        //{
        //    item.SetActive(false);
        //    Destroy(item);

        //}

        startPos = this.transform.position;
        //Currently use the player as the target



        if (PathCheck.GetDistance(this.transform.position,player.position)<=50&&!playerSpotted)
        {

            playerSpotted = true;
            playerPreviousPos = GameManager.instance.boardScript.player.transform.position;
        }

        if (playerSpotted)
        {
            switch (state)
            {
                case CHASING:
                    endPos = GameObject.FindGameObjectWithTag("Player").transform.position;
                    break;
                case INTERCEPTING:
                    break;
                case COOPERATING:
                    //if (playerOffset == Vector3.zero)
                    //    state = CHASING;
                    //else
                    //{
                        endPos = playerCurrentPos + 2*playerOffset;
                    if (GetDistance(this.transform.position,player.position)<=3)
                    {
                        state = CHASING;
                    }
                    if (!ValidGrid(endPos))
                    {
                        Vector3 newTarget = new Vector3(0,0,-1);
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                Vector3 alternativeTarget = endPos + new Vector3(i, j, 0);

                                if (ValidGrid(alternativeTarget))
                                {
                                    newTarget = alternativeTarget;
                                    
                                }
                            }
                        }

                        if (newTarget.z == -1)
                        {
                            endPos = this.transform.position;

                        }
                        else
                            endPos = newTarget;

                    }


                    Debug.Log("Now cooperating:"+playerOffset);

                    //}

                    break;
            }


            AStarSearchPath();
            AttemptMove();
        }


        


    }

    private bool ValidGrid(Vector3 target)
    {
        if (target.x < 0 || target.x > GameManager.instance.boardScript.columns - 2
            || target.y < 0 || target.y > GameManager.instance.boardScript.rows - 2
            || obstacle.Contains(target) || GameManager.instance.boardScript.UnitMap.ContainsKey(target))
        {
            return false;
        }
        else
        {
            return true;
        }
    }



    void AttemptMove()
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        Dictionary<Vector3, GameObject> Unitmap = GameManager.instance.boardScript.UnitMap;
        playerCurrentPos = GameManager.instance.boardScript.player.transform.position;

        skipMove = true;

        if (Unitmap.ContainsKey(targetGrid))
        {
            //Vector3 alternativeTarget =-2*(targetGrid - this.transform.position);
            Debug.Log("Multiple enemy collision triggered, target now");
            targetGrid = this.transform.position;
            state = COOPERATING;
            playerOffset = playerCurrentPos - playerPreviousPos;
            playerPreviousPos = playerCurrentPos;

            return;

        }

        Unitmap.Add(targetGrid,Unitmap[this.transform.position]);
        Unitmap.Remove(this.transform.position);

        //if(!obstacle.Contains(targetGrid))
            this.transform.position = targetGrid;
        playerOffset = playerCurrentPos - playerPreviousPos;
        Debug.Log("Offset " + playerOffset);
        playerPreviousPos = playerCurrentPos;
        
    }
    public void AStarSearchPath()
    {
        
        //startPos = this.transform.position;
        ////Currently use the player as the target
        //endPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        search.Add(startPos, GetDistance(startPos, endPos));
        cost.Add(startPos, 0);
        hadSearch.Add(startPos);
        pathSave.Add(startPos, startPos);

        while (search.Count > 0)
        {
            Vector3 current = GetShortestPos();

            if (current.Equals(endPos))
                break;

            List<Vector3> neighbors = GetNeighbors(current);

            foreach (var next in neighbors)
            {
                if (!hadSearch.Contains(next))
                {
                    cost.Add(next, cost[current] + 1);
                    search.Add(next, cost[next] + GetDistance(next, endPos));
                    pathSave.Add(next, current);
                    hadSearch.Add(next);
                }
            }
        }

        Vector3 backInduction = endPos;

        bool isEnd = true;

        while (backInduction != startPos)
        {
            Vector3 next = pathSave[backInduction];
            if (next==startPos)
            {
                targetGrid = backInduction;
            }

            else
            {
                if (isEnd)
                {
                    Instantiate(pathPainting[1], next, Quaternion.identity).transform.SetParent(Path);
                    isEnd = false;

                }else
                    Instantiate(pathPainting[0], next, Quaternion.identity).transform.SetParent(Path);

            }

            backInduction = next;
        }search.Clear();
        cost.Clear();
        pathSave.Clear();
        hadSearch.Clear();


    }
    private Vector3 GetShortestPos()
    {
        KeyValuePair<Vector3, int> shortest = new KeyValuePair<Vector3, int>(Vector3.zero, int.MaxValue);

        foreach (var item in search)
        {
            if (item.Value < shortest.Value)
            {
                shortest = item;
            }
        }

        search.Remove(shortest.Key);

        return shortest.Key;
    }
    private List<Vector3> GetNeighbors(Vector3 target)
    {
        List<Vector3> neighbors = new List<Vector3>();

        Vector3 up = target + Vector3.up;
        Vector3 right = target + Vector3.right;
        Vector3 left = target - Vector3.right;
        Vector3 down = target - Vector3.up;

        //Up
        if (up.y < GameManager.instance.boardScript.rows && !obstacle.Contains(up))
        {
            neighbors.Add(up);
        }
        //Right
        if (right.x < GameManager.instance.boardScript.columns && !obstacle.Contains(right))
        {
            neighbors.Add(target + Vector3.right);
        }
        //Left
        if (left.x >= 0 && !obstacle.Contains(left))
        {
            neighbors.Add(target - Vector3.right);
        }
        //Down
        if (down.y >= 0 && !obstacle.Contains(down))
        {
            neighbors.Add(target - Vector3.up);
        }

        return neighbors;
    }

    private int GetDistance(Vector3 posA, Vector3 posB)
    {
        return Convert.ToInt32(Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y));
    }



}
