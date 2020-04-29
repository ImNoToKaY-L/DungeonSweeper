using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Evaluation
{


    public float sizeFactor = 1f,
        wallNumberFactor = 1f;
    public int rowIntFactor,columnIntFactor,playerDetectionModifier = 1,
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
        GameObject.FindGameObjectWithTag("Prediction").GetComponent<Text>().text ="Estimated "+idealStepTaken.ToString();

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
        if (stepRatio >= 2.7||record.isCaught)
        {
            Debug.Log("Player takes too many steps or is caught by enemy");
            sizeFactor = MergeFactor(sizeFactor, 0.4f);
            wallNumberFactor = MergeFactor(wallNumberFactor, 0.4f);
            rowIntFactor = UnityEngine.Random.Range(-5,0);
            columnIntFactor = UnityEngine.Random.Range(-5, 0);
        }
        else if (stepRatio >= 2.2&&!record.isCaught)
        {
            float Sizemodifier =  1- (0.5f * (1 - DataPosition(2.2f, 2.7f, stepRatio)));
            sizeFactor = MergeFactor(sizeFactor, Sizemodifier);
            float Wallmodifier = 1 - (0.5f * (1 - DataPosition(2.2f, 2.7f, stepRatio)));
            wallNumberFactor = MergeFactor(wallNumberFactor, Wallmodifier);
            rowIntFactor = UnityEngine.Random.Range(-5, 0);
            columnIntFactor = UnityEngine.Random.Range(-5, 0);

        }
        else if (stepRatio >= 1.8&& !record.isCaught)
        {
            float Sizemodifier = 1+ (0.15f * (1 - DataPosition(1.8f, 2.2f, stepRatio)));
            sizeFactor = MergeFactor(sizeFactor, Sizemodifier);
            float Wallmodifier = 1 + (0.3f * (1 - DataPosition(1.8f, 2.2f, stepRatio)));
            wallNumberFactor = MergeFactor(wallNumberFactor, Wallmodifier);
            rowIntFactor = UnityEngine.Random.Range(0,5);
            columnIntFactor = UnityEngine.Random.Range(0,5);

        }
        else if(stepRatio>=1&& !record.isCaught)
        {
            float Sizemodifier = 1 + (0.2f * (1 - DataPosition(1f, 1.8f, stepRatio)));
            sizeFactor = MergeFactor(sizeFactor, Sizemodifier);
            float Wallmodifier = 1 + (0.4f * (1 - DataPosition(1f, 1.8f, stepRatio)));
            wallNumberFactor = MergeFactor(wallNumberFactor, Wallmodifier);
            rowIntFactor = UnityEngine.Random.Range(0,10);
            columnIntFactor = UnityEngine.Random.Range(0, 10);
        }

        //Debug.Log("Wall number modifier now is " + wallNumberFactor);
        int usedMapSize;
        float columnmax = 0, rowmax = 0;

        foreach (var item in board.UnitMap)
        {
            if (item.Key.x > columnmax)
                columnmax = item.Key.x;
            if (item.Key.y > rowmax)
                rowmax = item.Key.y;
        }
        usedMapSize = Convert.ToInt32(columnmax * rowmax);
        int maxScanSize = 2 * board.player.PlayerFOV + 1;
        maxScanSize *= maxScanSize;
        int maxScanUsage = usedMapSize / maxScanSize;

        if (record.scanUsed <= maxScanUsage)
            playerDetectionModifier = -1;   
        else
            playerDetectionModifier = 1;


        Debug.Log("wall number factor now is " + wallNumberFactor+"\nsize factor "+sizeFactor);



        SaveEvaluation();
        


    }



    public void EvaluatedModify(BoardManager board)
    {
        board.rows = Convert.ToInt32(board.rows*sizeFactor)+rowIntFactor;
        board.columns = Convert.ToInt32(board.columns * sizeFactor) + columnIntFactor;
        board.maxWallSpawnCount = Convert.ToInt32(board.maxWallSpawnCount * wallNumberFactor);
        int modifiedPlayerDetection = board.player.DistanceDetection + playerDetectionModifier;
        if (modifiedPlayerDetection<=10)
        {
            board.player.DistanceDetection = 10;
            board.player.PlayerFOV = 2;
        }
        else if (modifiedPlayerDetection>=20)
        {
            board.player.DistanceDetection = 20;
            board.player.PlayerFOV = 1;
        }
        else
        {
            board.player.DistanceDetection = modifiedPlayerDetection;
            board.player.PlayerFOV = 2;
        }

        if (board.rows >= 80) board.rows = 80;
        if (board.columns >= 80) board.columns = 80;

        if (board.rows <= 20) board.rows = 20;
        if (board.columns <= 20) board.columns = 20;

        

        if (board.maxWallSpawnCount*5>=(board.rows*board.columns)*0.3)
        {
            board.maxWallSpawnCount =Convert.ToInt32((board.rows * board.columns * 0.3) / 5) ;
        }
        else if (board.maxWallSpawnCount*5<=(board.rows*board.columns)*0.1)
        {
            board.maxWallSpawnCount = Convert.ToInt32((board.rows * board.columns) * 0.1);
        }

        

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
