using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class ResetEvaluation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClick);
        GameObject.FindGameObjectWithTag("ResetInfo").transform.localScale = Vector3.zero;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick()
    {
        if (File.Exists(Save.Evaluationpath))
        {
            File.Delete(Save.Evaluationpath);
            GameObject.FindGameObjectWithTag("ResetInfo").transform.localScale = new Vector3(1,1,1);
            Debug.Log("File exists");
        }
        else
        {
            GameObject.FindGameObjectWithTag("ResetInfo").transform.localScale = new Vector3(1, 1, 1);
            GameObject.FindGameObjectWithTag("ResetInfo").GetComponent<Text>().text = "No evaluation found";
            Debug.Log("File does not exists");

        }
    }
}
