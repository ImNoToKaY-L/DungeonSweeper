using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Save 
{
    [NonSerialized]
    private BoardManager board = GameManager.instance.boardScript;
    [NonSerialized]
    public static String Evaluationpath = Application.persistentDataPath + "/Evaluation.txt";
    [NonSerialized]
    public static String SavePath = Application.persistentDataPath + "/SavedBoard.txt";
    [NonSerialized]
    Player player = GameManager.instance.boardScript.player;
    [NonSerialized]
    public static bool LoadGame = false;

    public Record record;
    public int rows, columns, EnemyNumber, maxWallSpawnCount,hardLevel;
    public List<UnitPair> unitVector = new List<UnitPair>();
    public List<Vector3S> obstacles = new List<Vector3S>();
    public bool playerHasKey, playerHasCase;
    public Vector3S playerPos;
    public int playerFOV, playerDistanceDetection;
    public bool KeyFound, CaseFound, ExitFound;


    public Save()
    {
        record = board.record;
        rows = board.rows;
        columns = board.columns;
        foreach (var item in board.UnitMap)
        {
            unitVector.Add(new UnitPair(item.Key, item.Value));
        }
        foreach (var item in board.obstacles)
        {
            Vector3S vector = new Vector3S(item);
            obstacles.Add(vector);
        }
        playerHasKey = GameManager.instance.playerHasKey;
        playerHasCase = GameManager.instance.playerHasCase;

        playerFOV = player.PlayerFOV;
        playerDistanceDetection = player.DistanceDetection;
        playerPos = new Vector3S(GameObject.FindGameObjectWithTag("Player").transform.position);

        EnemyNumber = board.EnemyNumber;
        maxWallSpawnCount = board.maxWallSpawnCount;
        hardLevel = board.hardLevel;
        KeyFound = GameManager.instance.KeyFound;
        CaseFound = GameManager.instance.CaseFound;
        ExitFound = GameManager.instance.ExitFound;


    }
    public Save(bool NO)
    {

    }

    [Serializable]

    public class Vector3S
    {
        public float x;
        public float y;
        public float z;

        public Vector3S(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
        public Vector3 toVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public class UnitPair
    {
        public float x;
        public float y;
        public float z;
        public String unitTag;
         public UnitPair(Vector3 pos,GameObject unit)
        {
            x = pos.x;
            y = pos.y;
            z = pos.z;
            unitTag = unit.tag;
        }
        public Vector3 toVector3()
        {
            return new Vector3(x, y, z);
        }
    }


    public void SaveBoard()
    {
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        FileStream fs = File.Create(SavePath);
        bf.Serialize(fs, this);
        fs.Close();
        Debug.Log("Board saved successfully to " + SavePath);

        //AssetDatabase.Refresh();

    }

    public Save LoadBoard()
    {

        if (File.Exists(SavePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(SavePath, FileMode.Open);
            Save savedBoard = bf.Deserialize(fs) as Save;
            fs.Close();
            File.Delete(SavePath);
            Save.LoadGame = false;

            return savedBoard;
        }
        return null;
    }
}
