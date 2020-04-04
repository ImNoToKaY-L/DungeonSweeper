using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Evaluation
{
    public float rowFactor,columnFactor,wallNumberFactor, wallLengthFactor,playerDetectionDisFactor,playerFOVFactor;
    [NonSerialized]
    private BoardManager board = GameManager.instance.boardScript;

    public void Evaluate(Record record)
    {
        int idealStepTaken = 0;
        Vector3 Key, Case, Exit;
        foreach (var item in board.UnitMap)
        {
            if (item.Value.tag == "Key")
                Key = item.Key;
            if (item.Value.tag == "Case")
                Case = item.Key;
            if (item.Value.tag == "Exit")
                Exit = item.Key;
        }

        
    }


    private int CalculateStep(Vector3 Key, Vector3 Case, Vector3 Exit)
    {
        PathCheck.AStarSearchPath(new Vector3(0, 0, -1), Key);
    }



}
