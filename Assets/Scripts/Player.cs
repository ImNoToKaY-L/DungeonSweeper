using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MovingObject
{
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
	private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.
#endif

	private bool hasMoved = false;
	private List<Vector3> existingUnits;
	private List<Vector3> obstacles;
	private int PlayerFOV = 1;
	public GameObject[] NumberTiles;
	protected override void OnCantMove<T>(T component)
    {
    }



	private Animator animator;                  //Used to store a reference to the Player's animator component.

	// Start is called before the first frame update
	void Start()
    {
		animator = GetComponent<Animator>();
		base.Start();

	}


	private bool ValidGrid(Vector3 target)
	{
		if (target.x < 0 || target.x > GameManager.instance.boardScript.columns-2
			||target.y<0||target.y> GameManager.instance.boardScript.rows - 2
			||obstacles.Contains(target)||existingUnits.Contains(target))
		{
			return false;
		}
		else
		{
			return true;
		}
	}


	// Update is called once per frame
	void Update()
    {


		//If it's not the player's turn, exit the function.
		if (!GameManager.instance.playersTurn) return;

		int horizontal = 0;     //Used to store the horizontal move direction.
		int vertical = 0;       //Used to store the vertical move direction.

		//Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER

		//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
		horizontal = (int)(Input.GetAxisRaw("Horizontal"));

		//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
		vertical = (int)(Input.GetAxisRaw("Vertical"));

		//Check if moving horizontally, if so set vertical to zero.
		if (horizontal != 0)
		{
			vertical = 0;
		}
		//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //End of mobile platform dependendent compilation section started above with #elif
		//Check if we have a non-zero value for horizontal or vertical

		if (horizontal==0&&vertical==0)
		{
			hasMoved = false;
		}

		if (hasMoved)
		{
			return;
		}

		if (horizontal != 0 || vertical != 0)
		{
			//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
			//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
			GameManager.instance.playersTurn = false;
			AttemptMove<Player>(horizontal, vertical);
			DrawInformation();
			hasMoved = true;
		}


	}



	private void DrawInformation()
	{
		GameObject[] NumbersToDestroy = GameObject.FindGameObjectsWithTag("Number");
		foreach (var item in NumbersToDestroy)
		{
			item.SetActive(false);
			Destroy(item);
		}

		existingUnits = GameManager.instance.boardScript.hasUnits;
		obstacles = GameManager.instance.boardScript.obstacles;
		int startPosX = Convert.ToInt32(this.transform.position.x)-PlayerFOV;
		int startPosY = Convert.ToInt32(this.transform.position.y)-PlayerFOV;
		int length = 2 * PlayerFOV+1;

		for (int x = startPosX; x < startPosX+length; x++)
		{
			for (int y = startPosY; y < startPosY+length; y++)
			{
				Vector3 currentPos = new Vector3(x, y, 0f);
				if (currentPos!=this.transform.position)
				{
					if (ValidGrid(currentPos))
					{
						int nearestUnitDis = GetNearestUnitDistance(currentPos);
						if (nearestUnitDis<=9&&nearestUnitDis>0)
						{
							Instantiate(NumberTiles[nearestUnitDis - 1], currentPos, Quaternion.identity).transform.SetParent(GameManager.instance.boardScript.transform);
						}
					}
				}
			}
		}


	}

	private int GetNearestUnitDistance(Vector3 target)
	{
		int nearestDistance = -1;
		foreach (var unit in existingUnits)
		{
			int currentDis = PathCheck.GetDistance(target, unit);
			if (nearestDistance == -1) nearestDistance = currentDis;

			else
			{
				if (currentDis<nearestDistance)
				{
					nearestDistance = currentDis;
				}
			}
			Debug.Log(currentDis);


		}
		return nearestDistance;
		
	}



	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag=="Case")
		{
			if (GameManager.instance.playerHasKey)
			{
				collision.gameObject.SetActive(false);
				GameObject.Find("Canvas/Objective").GetComponent<Text>().text = "Objective: Escape";
				GameManager.instance.boardScript.hasUnits.Remove(collision.transform.position);
				GameManager.instance.playerHasCase = true;
				DrawInformation();

			}
		}
		if (collision.tag=="Key")
		{
			collision.gameObject.SetActive(false);
			GameManager.instance.boardScript.hasUnits.Remove(collision.transform.position);
			GameManager.instance.playerHasKey = true;
			GameObject.Find("Canvas/Objective").GetComponent<Text>().text = "Objective: Open the case";
			DrawInformation();
		}

		if (collision.tag == "Enemy")
		{
			GameManager.instance.playerHP -= 100;
			if (GameManager.instance.playerHP<=0)
			{
				Debug.Log("GAMEOVER");
				GameManager.instance.Restart();
			}
		}
		if (collision.tag == "Exit")
		{
			if (GameManager.instance.playerHasCase)
			{
				GameManager.instance.Restart();
			}
		}
	}
	protected override void AttemptMove<T>(int xDir, int yDir)
    {
		float xPos = this.transform.position.x;
		float yPos = this.transform.position.y;
		if ((xDir < 0 && xPos == 0) || (xDir > 0 && xPos == GameManager.instance.boardScript.columns-2) || (yDir < 0 && yPos == 0) || (yDir > 0 && yPos == GameManager.instance.boardScript.rows - 2))
		{
			return;
		}
			

		
		base.AttemptMove<T>(xDir, yDir);
		RaycastHit2D hit;
		//Set the playersTurn boolean of GameManager to false now that players turn is over.
	}

}
