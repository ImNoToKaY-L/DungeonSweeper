using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Detection : MonoBehaviour
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
        GameManager.instance.boardScript.record.scanUsed++;
        GameManager.instance.boardScript.player.DrawInformation();
        GameManager.instance.playersTurn = false;
    }
}
