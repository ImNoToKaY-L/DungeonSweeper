using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCheck : MonoBehaviour
{


    


    void Start()
    {
        //if (initiating)
        //{
        //    initiating = false;
        //    startPos = this.transform.position;
        //    endPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        //    obstacle = GameManager.instance.boardScript.obstacles;
        //    GameManager.instance.AddEnemyToList(this);
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }


    public static bool AStarSearchPath(Vector3 currentPos,Vector3 endPos, List<Vector3>obstacle, out int Length)
    {
        Vector3 startPos;
        Vector3 endPos;
        Length = 0;
        Dictionary<Vector3, int> search = new Dictionary<Vector3, int>();
        Dictionary<Vector3, int> cost = new Dictionary<Vector3, int>();
        Dictionary<Vector3, Vector3> pathSave = new Dictionary<Vector3, Vector3>();
        List<Vector3> hadSearch = new List<Vector3>();
        startPos = currentPos;
        //Currently use the player as the target
        endPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        search.Add(startPos, GetDistance(startPos, endPos));
        cost.Add(startPos, 0);
        hadSearch.Add(startPos);
        pathSave.Add(startPos, startPos);

        while (search.Count > 0)
        {
            Vector3 current = GetShortestPos(search);

            if (current.Equals(endPos))
                break;

            List<Vector3> neighbors = GetNeighbors(current,obstacle);

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


        try
        {
            Vector3 backInduction = endPos;
            while (backInduction != startPos)
            {
                Vector3 next = pathSave[backInduction];
                if (next == startPos)
                {
                    return true;
                }
                Length++;

                backInduction = next;
            }
        }
        catch (Exception)
        {
            return false;
        }



        return false;


    }
    private static Vector3 GetShortestPos(Dictionary<Vector3, int> search)
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
    private static List<Vector3> GetNeighbors(Vector3 target, List<Vector3> obstacle)
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

    public static int GetDistance(Vector3 posA, Vector3 posB)
    {
        return Convert.ToInt32(Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y));
    }



}
