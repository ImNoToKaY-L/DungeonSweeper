using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;

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


    void Start()
    {
        if (initiating)
        {
            Destroy(GameObject.Find("PathPainting"));

            Path = new GameObject("PathPainting").transform;
            initiating = false;
            startPos = this.transform.position;
            endPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            obstacle = GameManager.instance.boardScript.obstacles;
            GameManager.instance.AddEnemyToList(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void MoveEnemy()
    {
        //GameObject[] paintedGrid = GameObject.FindGameObjectsWithTag("Soda");
        //foreach (var item in paintedGrid)
        //{
        //    item.SetActive(false);
        //    Destroy(item);
            
        //}
        AStarSearchPath();
        AttemptMove();

    }

    void AttemptMove()
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        Dictionary<Vector3, GameObject> Unitmap = GameManager.instance.boardScript.UnitMap;

        skipMove = true;

        if (Unitmap.ContainsKey(targetGrid))
        {
            Vector3 alternativeTarget =-2*(targetGrid - this.transform.position);
            Debug.Log("Multiple enemy collision triggered, target now:"+targetGrid);
            targetGrid = targetGrid + alternativeTarget;
            Debug.Log("Target after" + targetGrid);



        }

        Unitmap.Add(targetGrid,Unitmap[this.transform.position]);
        Unitmap.Remove(this.transform.position);

        this.transform.position = targetGrid;
        
    }
    public void AStarSearchPath()
    {
        
        startPos = this.transform.position;
        //Currently use the player as the target
        endPos = GameObject.FindGameObjectWithTag("Player").transform.position;
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
