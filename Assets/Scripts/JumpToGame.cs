﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JumpToGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClick);
        Debug.Log("ITISATEST");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnClick()
    {
        SceneManager.LoadScene("Body");
    }
}
