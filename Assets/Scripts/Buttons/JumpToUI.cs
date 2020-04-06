using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JumpToUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnClick()
    {

//        String path = Application.dataPath + "/Scripts" + "/SavedBoard.txt";

//#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
//        path = Application.persistentDataPath + "/SavedBoard.txt";
//#endif
        if (File.Exists(Save.SavePath))
        {
            //Save save = new Save().LoadBoard();
            //Debug.Log("Rows are now: "+ save.rows);
            //foreach (var item in save.unitVector)
            //{
            //    Debug.Log(item.x+" "+item.y+"  "+item.z);
            //}
        }
        else
        new Save().SaveBoard();

        SceneManager.LoadScene("Demo (Mobile)");
    }
}
