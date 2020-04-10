using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

	using System.Collections.Generic;		//Allows us to use Lists. 
	using UnityEngine.UI;                   //Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{
		public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
		public bool playersTurn = true;		
		public BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.
		public bool playerHasKey = false;
		public bool playerHasCase = false;
		public int playerHP = 100;
		public List<Enemy> enemies;
		public bool enemyIntercepting = false;
		public bool enemyCooperating = false;
		public bool KeyFound, CaseFound, ExitFound;
		public Evaluation evaluation;
		
		//Awake is always called before any Start functions
		void Awake()
		{
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)
		{
			instance.playersTurn = true;
			instance.playerHasKey = false;
			instance.playerHasCase = false;
			instance.playerHP = 100;
			Destroy(gameObject);

		}
		

			boardScript = GetComponent<BoardManager>();
			
			//Call the InitGame function to initialize the first level 
			InitGame();
		}

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]

		
		public void InitGame()
		{
		enemies = new List<Enemy>();
		playersTurn = true;
		playerHasKey = false;
		playerHasCase = false;
		if (!Save.LoadGame)
			boardScript.SetupScene();
		else
			boardScript.SetupSceneFromSave(new Save(false).LoadBoard());
		evaluation =  Evaluation.LoadEvaluation();
		evaluation.StepPrediction();
		enemyIntercepting = false;
		enemyCooperating = false;
		KeyFound = false;
		CaseFound = false;
		ExitFound = false;




}

	public void Restart()
	{

		SceneManager.LoadScene("Body");
	}

	public void ProgressToNextLevel()
	{
		evaluation.Evaluate(boardScript.record);

		boardScript.NextLevel();
		InitGame();
	}


	private void Update()
	{
		if (!playerHasKey&&!playerHasCase)
		{
			if (GameObject.Find("Canvas/Objective").GetComponent<Text>().text != "Objective: Fetch the Key")
			GameObject.Find("Canvas/Objective").GetComponent<Text>().text = "Objective: Fetch the Key ";

		}

		else if (playerHasKey&&!playerHasCase)
		{
			if (GameObject.Find("Canvas/Objective").GetComponent<Text>().text!= "Objective: Open the case")
			{
				GameObject.Find("Canvas/Objective").GetComponent<Text>().text = "Objective: Open the case";
			}
		}
		else
		{
			if (GameObject.Find("Canvas/Objective").GetComponent<Text>().text != "Objective: Escape")
			{
				GameObject.Find("Canvas/Objective").GetComponent<Text>().text = "Objective: Escape";
			}
		}

		if (playersTurn)
		{
			return;
		}

		playersTurn = true;


		GameObject[] paintedGrid = GameObject.FindGameObjectsWithTag("Soda");
		foreach (var item in paintedGrid)
		{
			item.SetActive(false);
			Destroy(item);

		}

		foreach (var enemy in enemies)
		{
			enemy.MoveEnemy();
		}


	}

	public void AddEnemyToList(Enemy script)
	{
		//Add Enemy to List enemies.
		enemies.Add(script);
	}



}


