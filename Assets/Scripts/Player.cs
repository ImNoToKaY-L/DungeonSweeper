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

	//private List<Vector3> existingUnits;
	private List<Vector3> obstacles;
	private Dictionary<Vector3, GameObject> UnitMap;
	public int PlayerFOV = 1;
	public int DistanceDetection = 20;
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
			||obstacles.Contains(target)||UnitMap.ContainsKey(target))
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


	}

	public void ClearAllNumbers()
	{
		GameObject[] NumbersToDestroy = GameObject.FindGameObjectsWithTag("Number");
		foreach (var item in NumbersToDestroy)
		{
			item.SetActive(false);
			Destroy(item);
		}
	}

	public void DrawInformation()
	{
		ClearAllNumbers();

		//existingUnits = GameManager.instance.boardScript.hasUnits;
		obstacles = GameManager.instance.boardScript.obstacles;
		UnitMap = GameManager.instance.boardScript.UnitMap;
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
						if (nearestUnitDis<=DistanceDetection&&nearestUnitDis>0)
						{
							Instantiate(NumberTiles[nearestUnitDis - 1], currentPos, Quaternion.identity).transform.SetParent(GameManager.instance.boardScript.transform);
						}
					}
				}

				if (UnitMap.ContainsKey(currentPos))
				{
					UnitMap[currentPos].SetActive(true);

					switch (UnitMap[currentPos].tag)
					{
						case "Key":
							GameManager.instance.KeyFound = true;
							Debug.Log("Key has been found");
							break;
						case "Case":
							GameManager.instance.CaseFound = true;
							Debug.Log("Case has been found");

							break;
						case "Exit":
							GameManager.instance.ExitFound = true;
							Debug.Log("Exit has been found");

							break;
					}
				}

			}
		}


	}

	private int GetNearestUnitDistance(Vector3 target)
	{
		int nearestDistance = -1;
		foreach (var unit in UnitMap.Keys)
		{
			int currentDis = PathCheck.GetDistance(target, unit);
			if (UnitMap[unit].tag!="Enemy"&& !UnitMap[unit].activeSelf)
			{
				if (nearestDistance == -1) nearestDistance = currentDis;

				else
				{
					if (currentDis < nearestDistance)
					{
						nearestDistance = currentDis;
					}
				}
			}



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
				//GameManager.instance.boardScript.hasUnits.Remove(collision.transform.position);
				GameManager.instance.boardScript.UnitMap.Remove(collision.transform.position);
				GameManager.instance.playerHasCase = true;
				//DrawInformation();

			}
		}
		if (collision.tag=="Key")
		{
			collision.gameObject.SetActive(false);
			//GameManager.instance.boardScript.hasUnits.Remove(collision.transform.position);
			GameManager.instance.boardScript.UnitMap.Remove(collision.transform.position);
			GameManager.instance.playerHasKey = true;
			GameObject.Find("Canvas/Objective").GetComponent<Text>().text = "Objective: Open the case";
			//DrawInformation();
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
				//GameManager.instance.Restart();
				ClearAllNumbers();
				GameManager.instance.ProgressToNextLevel();
			}
		}
	}
	public override void AttemptMove<T>(int xDir, int yDir)
    {
		float xPos = this.transform.position.x;
		float yPos = this.transform.position.y;
		if ((xDir < 0 && xPos == 0) || (xDir > 0 && xPos == GameManager.instance.boardScript.columns-2) || (yDir < 0 && yPos == 0) || (yDir > 0 && yPos == GameManager.instance.boardScript.rows - 2))
		{
			return;
		}
			

		
		base.AttemptMove<T>(xDir, yDir);
		//Set the playersTurn boolean of GameManager to false now that players turn is over.
	}

}
