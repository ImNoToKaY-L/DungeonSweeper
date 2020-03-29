using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PlayMenuControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

//        String path = Application.dataPath + "/Scripts" + "/SavedBoard.txt";

//#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
//        path = Application.persistentDataPath + "/SavedBoard.txt";
//#endif
        if (!File.Exists(Save.SavePath))
        {
            GameObject.FindGameObjectWithTag("Loadgame").SetActive(false);
            GameObject.FindGameObjectWithTag("Newgame").GetComponent<RectTransform>().anchoredPosition=new Vector3(0,0,0);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Loadgame").SetActive(true);
            GameObject.FindGameObjectWithTag("Newgame").GetComponent<RectTransform>().anchoredPosition = new Vector3(-200, 0, 0);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
