using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrows : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(MovePlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void MovePlayer()
    {

        if (!GameManager.instance.playersTurn) return;

        int horizontal=0, vertical=0;

        switch (this.GetComponent<Button>().tag)
        {
            case "UP":
                horizontal = 0;
                vertical = 1;
                break;
            case "LEFT":
                horizontal = -1;
                vertical = 0;
                break;
            case "RIGHT":
                horizontal = 1;
                vertical = 0;
                break;
            case "DOWN":
                horizontal = 0;
                vertical = -1;
                break;
        }
        GameManager.instance.playersTurn = false;
        GameManager.instance.boardScript.record.stepTaken++;
        //Debug.Log("Current step taken " + GameManager.instance.boardScript.record.stepTaken);
        GameManager.instance.boardScript.player.AttemptMove<Player>(horizontal, vertical);
        //DrawInformation();
    }
}
