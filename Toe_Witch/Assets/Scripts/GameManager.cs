/* Game Controller
 * Evelyn Wightman 2016
 * Modified from Done_GameController.cs from Unity Space Shooter Tutorial
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f; //seconds
	public GameObject VillagerSpawnPoint;
	public int tramplerCount;
	public float spawnWait;
	public float startWait;
	public float waveWait; 
	public float dayDuration;

	public static GameManager instance = null;

	private Text levelText;
	private GameObject levelImage;
	private Text tutorialText;
	private GameObject tutorialImage;
	private int level = 0;
	private Vector3 screenPosition;
	private Vector3 offset;
	private GameObject mouseTarget;
	private GameObject shadow;
	private List<GameObject> enemies = new List<GameObject>();
	private bool done;
	private ClickHandler clickHandler;


	void Awake()
	{
		//make sure there's only ever one GameManager
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else if (instance != this) {
			Destroy (this.gameObject);
			return;
		}

		//find level and tutorial text
		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		tutorialImage = GameObject.Find ("TutorialImage");
		tutorialText = GameObject.Find ("TutorialText").GetComponent<Text> ();

		//start with tutorial image off
		tutorialImage.SetActive(false);

		//start not being able to click on stuff
		clickHandler = GetComponent<ClickHandler>();
		clickHandler.enabled = false;

		//Start the game
		InitLevel();
	}
	
	void InitLevel ()
	{
		//disable clicking while we set up
		clickHandler.enabled = false;

		level++;

		//display level text
		levelText.text = "Day" + level;
		levelImage.SetActive (true);

		//empty enemies list
		enemies.Clear();

		//hide level text and start level
		StartCoroutine("BeginLevel");

	}

	IEnumerator BeginLevel(){
		yield return new WaitForSeconds (levelStartDelay);

		levelImage.SetActive (false);

		//enable clicking
		clickHandler.enabled = true;

		switch(level){
		case 1:
			{
				//display seed planting tutorial
				tutorialText.text = "Time to garden! Drag seeds from your inventory to a pot in the nursery to plant them. \n \n Click anywhere to continue";
				done = false;
				StartCoroutine ("DisplayTutorial");
				while (!done) {
					yield return null;
				}

				//wait for them to plant a tomato
				bool planted = false;
				while (!planted) {
					GameObject[] plants = GameObject.FindGameObjectsWithTag ("Plant");
					foreach (GameObject plant in plants) {
						if (plant.GetComponent<FloraController> ().planted) {
							planted = true;
						}
					}
					yield return null;
				}

				//spawn a single trampler
				GameObject enemy = VillagerSpawnPoint.GetComponent<SpawnManager>().SpawnTrampler();
				enemies.Add (enemy);

				//wait
				yield return new WaitForSeconds (5);

				//show hitting tutorial
				tutorialText.text = "Looks like someone from the village come to visit. I hate visitors! They always trample up my grass. Click on him to chase him away.";
				StartCoroutine("DisplayTutorial");

				break;
			}
		case 2:
			{
				//combine tomatoes
				tutorialText.text = "So many visitors! I know just the thing to scare them off. Drag one of those toes onto one of my tomato plants.";
				done = false;
				StartCoroutine ("DisplayTutorial");
				while (!done) {
					yield return null;
				}
				//plant stuff in yard
				StartCoroutine("WaitForTomatoe");

				StartCoroutine ("SpawnWaves");
				break;
			}
		default:
			{
				StartCoroutine ("SpawnWaves");
				break;
			}
		}

		//start spawning enemies and running day time

		StartCoroutine ("WaitForDayEnd");
	}

	IEnumerator WaitForTomatoe(){
		GameObject tomatoe = GameObject.FindGameObjectWithTag ("FightingPlant");
		while (tomatoe == null) {
			tomatoe = GameObject.FindGameObjectWithTag ("FightingPlant");
			yield return null;
		}
		tutorialText.text = "Perfect! You've created a tomatoe plant! Drag it anywhere on the yard to plant it.";
		StartCoroutine("DisplayTutorial");

		MeleePlantController controller = tomatoe.GetComponent<MeleePlantController>();

		while(controller.planted == false){
			yield return null;
		}
		tutorialText.text = "Great. Now, plants that have been magicked together aren't too stable, so it'll only last a few days. The yellow dots show how many days it has left. Watch out for its health too. Ain't no plant that likes to be trampled. (Grumble)";
		StartCoroutine("DisplayTutorial");
	}

	IEnumerator DisplayTutorial(){
		//pause game
		Time.timeScale = 0.0f;
		//show tutorial
		tutorialImage.SetActive(true);
		//wait for mouse click to get rid of it
		while (!Input.GetMouseButtonDown (0)) {
			yield return null;
		}
		tutorialImage.SetActive (false);
		Time.timeScale = 1.0f;
		//let whatever called this know we're done
		done = true;
	}

	
	IEnumerator SpawnWaves ()
	{
		yield return new WaitForSeconds (startWait);
		while (true)
		{
			for (int i = 0; i < tramplerCount; i++)
			{
				GameObject enemy = VillagerSpawnPoint.GetComponent<SpawnManager>().SpawnTrampler();
				//add new trampler to list of enemies
				enemies.Add (enemy);
				yield return new WaitForSeconds (spawnWait);
			}
			yield return new WaitForSeconds (waveWait);
		}
	}

	/* WaitForDayEnd
	 * Calls EndLevel(won) if it's still running at the end of the day.
	 */
	IEnumerator WaitForDayEnd ()
	{
		yield return new WaitForSeconds (dayDuration);
		EndDay (true); //we won
	}

	/* EndLevel
	 * won = true of we won, false if we lost
	 * Called when we reach the end of the level by WaitForDayEnd or by BoardManager if all the grass dies
	 */
	public void EndDay(bool won)
	{
		StopCoroutine ("WaitForDayEnd");
		StopCoroutine ("SpawnWaves");

		//get rid of all the enemies
		foreach (GameObject enemy in enemies) {
			enemy.transform.position = VillagerSpawnPoint.transform.position; //move them away to trigger OnTriggerExit before destroy
			Destroy (enemy);
		}

		GameObject.Find ("Player").GetComponent<PlayerController> ().OnDayEnd ();
		VillagerSpawnPoint.GetComponent<SpawnManager> ().OnDayEnd ();

		//Proceed to nightTime
		BeginNight();
	}

	void BeginNight(){
		//Age all plants
		List<GameObject> allPlants = GameObject.FindGameObjectsWithTag("Plant").ToList();
		List<GameObject> fightingPlants = GameObject.FindGameObjectsWithTag("FightingPlant").ToList();
		allPlants.AddRange (fightingPlants);
		foreach (GameObject plant in allPlants) {
			if (plant.GetComponent<FloraController> () != null) {
				FloraController controller = plant.GetComponent<FloraController> ();
				controller.Age();
			}
			else if(plant.GetComponent<MeleePlantController>() != null) {
				MeleePlantController controller = plant.GetComponent<MeleePlantController>();
				controller.Age();
			}
		}

		//start next level
		InitLevel();

	}
		
}