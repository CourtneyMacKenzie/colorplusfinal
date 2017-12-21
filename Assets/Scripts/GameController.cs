using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public GameObject cubePrefab, nextCubePrefab;
	float gameLength = 60;
	int gridX = 8;
	int gridY = 5;
	GameObject[,] grid;
	GameObject nextCube;
	Vector3 cubePos;
	Vector3 nextCubePos = new Vector3 (7, 10, 0);
	float turnLength = 3;
	int turn = 0;
	Color[] myColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.magenta };
	int score = 0;
	GameObject activeCube = null;


	// Use this for initialization
	void Start ()
	{
		CreateGrid ();

	}


	void CreateGrid ()
	{
		grid = new GameObject[gridX, gridY];

		for (int y = 0; y < gridY; y++) {
			for (int x = 0; x < gridX; x++) {
				cubePos = new Vector3 (x * 2, y * 2, 0);
				grid [x, y] = Instantiate (cubePrefab, cubePos, Quaternion.identity);
				grid [x, y].GetComponent<CubeController> ().myX = x;
				grid [x, y].GetComponent<CubeController> ().myY = y;
			}
		}
	}

	void CreateNextCube ()
	{
		nextCube = Instantiate (nextCubePrefab, nextCubePos, Quaternion.identity);
		nextCube.GetComponent<Renderer> ().material.color = myColors [Random.Range (0, myColors.Length)];
	}


	void EndGame (bool win)
	{
		//end the game
		if (win) {
			//players win
			print ("You've won! Woo!");
		} else {
			//players lose
			print ("You've lost. Please try again!");
		}
	}


	GameObject PickWhiteCube (List<GameObject> whiteCubes) {
		//no white cubes in the row
		if (whiteCubes.Count == 0) {
			//error value
			return null;
		}
		//pick a random white cube
		return whiteCubes [Random.Range (0, whiteCubes.Count)];
	}

	GameObject FindAvailableCube (int y)
	{
		List<GameObject> whiteCubes = new List<GameObject> ();

		//count the number of white cubes
		for (int x = 0; x < gridX; x++) {
			if (grid [x, y].GetComponent<Renderer> ().material.color == Color.white) {
				whiteCubes.Add (grid [x, y]);

			}
		}
		return PickWhiteCube (whiteCubes);
	}

	GameObject FindAvailableCube ()
	{
		List<GameObject> whiteCubes = new List<GameObject> ();

		//make a lit of white cubes
		for (int y = 0; y < gridY; y++) {
			for (int x = 0; x < gridX; x++) {
				if (grid [x, y].GetComponent<Renderer> ().material.color == Color.white) {
					whiteCubes.Add (grid [x, y]);

				}
			}
		}
		return PickWhiteCube (whiteCubes);
	}

	void SetCubeColor (GameObject myCube, Color color) {
		//no available cube in that row
		if (myCube == null) {
			EndGame (false); 

		} else {
			//assign the nextCube's color to the chosen cube
			myCube.GetComponent<Renderer> ().material.color = color;
			Destroy (nextCube);
			nextCube = null;
		}
	}

	void PlaceNextCube (int y)
	{
		GameObject whiteCube = FindAvailableCube (y);

		SetCubeColor (whiteCube, nextCube.GetComponent<Renderer> ().material.color);

	}

	void AddRandomBlackCube ()
	{
		GameObject whiteCube = FindAvailableCube ();
		SetCubeColor (whiteCube, Color.black);
	}

	void ProcessKeyboardInput ()
	{

		int numKeyPressed = 0;

		if (Input.GetKeyDown (KeyCode.Alpha1) || Input.GetKeyDown (KeyCode.Keypad1)) {
			numKeyPressed = 1;
		}
		if (Input.GetKeyDown (KeyCode.Alpha2) || Input.GetKeyDown (KeyCode.Keypad2)) {
			numKeyPressed = 2;
		}
		if (Input.GetKeyDown (KeyCode.Alpha3) || Input.GetKeyDown (KeyCode.Keypad3)) {
			numKeyPressed = 3;
		}
		if (Input.GetKeyDown (KeyCode.Alpha4) || Input.GetKeyDown (KeyCode.Keypad4)) {
			numKeyPressed = 4;
		}
		if (Input.GetKeyDown (KeyCode.Alpha5) || Input.GetKeyDown (KeyCode.Keypad5)) {
			numKeyPressed = 5;
		}
		//if we still have a new cube, put it in the specified row, and the player pressed a valid number key
		if (nextCube != null && numKeyPressed != 0) {
		
			//put it in specified row
			//subtract 1 because the grid array has a 0-based index, starts with 0 not 1
			PlaceNextCube (numKeyPressed - 1);
		}
	}

	public void ProcessClick (GameObject clickedCube, int x, int y, Color cubeColor, bool active) {
		// did we click on a colored cube?
		if (cubeColor != Color.white && cubeColor != Color.black) {
			// if that cube was active, deactive it and set that cube to not be active anymore
			if (active) {
				// deactivate it
				clickedCube.transform.localScale /= 1.5f;
				clickedCube.GetComponent<CubeController> ().active = false;
				activeCube = null;

			}
			// the colored clicked cube was not active
		
			else {
				//deactivate any previously active cube
				if (activeCube != null) {
					activeCube.transform.localScale /= 1.5f;
					activeCube.GetComponent<CubeController> ().active = false;

				}
				//activate the newly clicked cube
				clickedCube.transform.localScale *= 1.5f;
				clickedCube.GetComponent<CubeController> ().active = true;
				activeCube = clickedCube;

			
			}
		} 
		else if (cubeColor == Color.white && activeCube != null) {
			int xDist = clickedCube.GetComponent<CubeController> ().myX - activeCube.GetComponent<CubeController> ().myX;
			int yDist = clickedCube.GetComponent<CubeController> ().myY - activeCube.GetComponent<CubeController> ().myY;

			// if we are within 1, including diagonals
			if (Mathf.Abs (yDist) <= 1 && Mathf.Abs (xDist) <= 1) {
				clickedCube.GetComponent<Renderer> ().material.color = activeCube.GetComponent<Renderer> ().material.color;
				clickedCube.transform.localScale *= 1.5f;
				clickedCube.GetComponent<CubeController> ().active = true;
	
				// set the old active cube to be white and not active
				activeCube.GetComponent<Renderer> ().material.color = Color.white;
				activeCube.transform.localScale /= 1.5f;
				activeCube.GetComponent<CubeController> ().active = false;


				//keep track of the new active cube
				activeCube = clickedCube;

		

			}
		}
	}


	// Update is called once per frame
	void Update ()
	{

		ProcessKeyboardInput ();
		
		if (Time.time > turnLength * turn) {
			turn++;

			//if we still have an existing next cube
			if (nextCube != null) {
				score -= 1;
				AddRandomBlackCube ();
			}

			CreateNextCube ();
		}

	}
}