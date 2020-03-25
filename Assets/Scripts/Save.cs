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

    public int rows, columns, EnemyNumber, maxWallSpawnCount,hardLevel;
    public List<Vector3S> unitVector = new List<Vector3S>();

    public Save()
    {
        rows = board.rows;
        columns = board.columns;
        foreach (var item in GameManager.instance.boardScript.UnitMap)
        {
            Vector3S vector = new Vector3S(item.Key);
            unitVector.Add(vector);
        }
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
            //AssetDatabase.Refresh();

            return savedBoard;
        }
        return null;
    }
}
