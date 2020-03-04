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

        //If it's not the player's turn, exit the function.
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



        //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
        //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
        GameManager.instance.playersTurn = false;
        GameManager.instance.boardScript.player.AttemptMove<Player>(horizontal, vertical);
        //DrawInformation();
    }
}
