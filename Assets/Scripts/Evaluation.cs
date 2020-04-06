using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[Serializable]
public class Evaluation
{


    public float rowFactor =1f,
        columnFactor =1f,
        wallNumberFactor =1f,
        playerDetectionModifier =1f,
        playerFOVModifier;

    [NonSerialized]
    public int idealStepTaken;
    [NonSerialized]
    BoardManager board;


    public Evaluation()
    {

    }


    public void StepPrediction()
    {
        board = GameManager.instance.boardScript;
        Vector3 Key = Vector3.zero,
    Case = Vector3.zero,
    Exit = Vector3.zero;
        foreach (var item in board.UnitMap)
        {
            if (item.Value.tag == "Key")
                Key = item.Key;
            if (item.Value.tag == "Case")
                Case = item.Key;
            if (item.Value.tag == "Exit")
                Exit = item.Key;
        }

        idealStepTaken = CalculateStep(Key, Case, Exit);
        Debug.Log("Current Board is predicted to use " + idealStepTaken + " steps");
    }
    public void Evaluate(Record record)
    {

        //idealStepTaken = CalculateStep(Key, Case, Exit);


        //This ratio will be used to evaluate how familiar the player is with the game
        //The worst expected ratio should be around 2, which means the player finds the item
        //in a totally reversed order (Exit - Case - Key - Case - Exit), so within this range
        //the player should still be deemed as familar player.
        float stepTakenF = record.stepTaken;
        float idealStepF = idealStepTaken;
        float stepRatio = stepTakenF/idealStepF;
        Debug.Log("Step ratio = " + stepRatio);
        float sizeFactor = 1;
        if (stepRatio >= 3.4)
        {
            sizeFactor = MergeFactor(sizeFactor, 0.6f);
            wallNumberFactor = MergeFactor(wallNumberFactor, 0.6f);
        }
        else if (stepRatio >= 2.7)
        {
            float Sizemodifier =  1- (0.4f * (1 - DataPosition(2.7f, 3.4f, stepRatio)));
            sizeFactor = MergeFactor(sizeFactor, Sizemodifier);
            float Wallmodifier = 1 - (0.4f * (1 - DataPosition(2.7f, 3.4f, stepRatio)));
            wallNumberFactor = MergeFactor(wallNumberFactor, Wallmodifier);

        }
        else if (stepRatio >= 2)
        {
            float Sizemodifier = 1+ (0.3f * (1 - DataPosition(2f, 2.7f, stepRatio)));
            sizeFactor = MergeFactor(sizeFactor, Sizemodifier);
            float Wallmodifier = 1 + (0.35f * (1 - DataPosition(2f, 2.7f, stepRatio)));
            wallNumberFactor = MergeFactor(wallNumberFactor, Wallmodifier);

        }
        else if(stepRatio>=1)
        {
            float Sizemodifier = 1 + (0.4f * (1 - DataPosition(1f, 2f, stepRatio)));
            sizeFactor = MergeFactor(sizeFactor, Sizemodifier);
            float Wallmodifier = 1 + (0.5f * (1 - DataPosition(1f, 2f, stepRatio)));
            wallNumberFactor = MergeFactor(wallNumberFactor, Wallmodifier);
        }

        //Debug.Log("Wall number modifier now is " + wallNumberFactor);
        int totalMapSize = board.rows * board.columns;
        int maxScanSize = 2 * board.player.PlayerFOV + 1;
        maxScanSize *= maxScanSize;
        
        


        SaveEvaluation();
        


    }


    private int CalculateStep(Vector3 Key, Vector3 Case, Vector3 Exit)
    {
        BoardManager board = GameManager.instance.boardScript;
        int totalStep = 0;
        int StepTaken;
        PathCheck.AStarSearchPath(new Vector3(0, 0, 0f),Key,board.obstacles,out StepTaken);
        totalStep += StepTaken;
        PathCheck.AStarSearchPath(Key, Case, board.obstacles, out StepTaken);
        totalStep += StepTaken;
        PathCheck.AStarSearchPath(Case, Key, board.obstacles, out StepTaken);
        totalStep += StepTaken;
        return totalStep;

    }
    private float MergeFactor(float Factor, float Ratio)
    {
        return ((1 + Ratio) / 2) * Factor;
    }

    private float DataPosition(float lowerbound,float upperbound,float inputData)
    {
        if (upperbound<=lowerbound)
        {
            Debug.LogError("INTERNAL ERROR, INVALID BOUND PAIR INPUT");
            return -1;
        }
        float boundDifference = upperbound - lowerbound;
        float position = inputData - lowerbound;
        return position / boundDifference;
    }

    private void SaveEvaluation()
    {
        BinaryFormatter bf = new BinaryFormatter();


        FileStream fs = File.Create(Save.Evaluationpath);
        bf.Serialize(fs, this);
        fs.Close();
        Debug.Log("Evaluation saved successfully to " + Save.Evaluationpath);
    }

    public void DebugLoad()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(Save.Evaluationpath, FileMode.Open);
        Evaluation e = bf.Deserialize(fs) as Evaluation;
        fs.Close();
        Debug.Log(e.wallNumberFactor+" is modified to wall number");
    }

    public static Evaluation LoadEvaluation()
    {
        if (File.Exists(Save.Evaluationpath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(Save.Evaluationpath, FileMode.Open);
            Evaluation e = bf.Deserialize(fs) as Evaluation;
            fs.Close();
            return e;
        }
        return new Evaluation();
    }

    //private int ConstFactor(int lowerbound,int upperbound)
    //{
    //    return UnityEngine.Random.Range(lowerbound, upperbound);
    //}

}
