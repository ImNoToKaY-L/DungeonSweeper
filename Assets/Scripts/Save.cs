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
    public static String SavePath = Application.persistentDataPath + "/SavedBoard.txt";
    [NonSerialized]
    Player player = GameManager.instance.boardScript.player;
    [NonSerialized]
    public static bool LoadGame = false;


    public int rows, columns, EnemyNumber, maxWallSpawnCount,hardLevel;
    public List<UnitPair> unitVector = new List<UnitPair>();
    public List<Vector3S> obstacles = new List<Vector3S>();
    public bool playerHasKey, playerHasCase;
    public Vector3S playerPos;
    public int playerFOV, playerDistanceDetection;

    public Save()
    {
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
        String path = Application.dataPath + "/Scripts" + "/SavedBoard.txt";
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        path = Application.persistentDataPath+  "/SavedBoard.txt";
#endif



        FileStream fs = File.Create(path);
        bf.Serialize(fs, this);
        fs.Close();
        //AssetDatabase.Refresh();

    }

    public Save LoadBoard()
    {
        String path = Application.dataPath + "/Scripts" + "/SavedBoard.txt";
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        path = Application.persistentDataPath + "/SavedBoard.txt";
#endif
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);
            Save savedBoard = bf.Deserialize(fs) as Save;
            fs.Close();
            File.Delete(path);

            return savedBoard;
        }
        return null;
    }
}
