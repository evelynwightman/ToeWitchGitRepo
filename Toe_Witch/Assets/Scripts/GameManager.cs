/* Game Controller
 * Evelyn Wightman 2016
 * Modified from Done_GameController.cs from Unity Space Shooter Tutorial
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
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
	public bool tutorialOn = true; //set in menu once there's a menu. Currently no tutorial means no level variation.
	public DialogueDisplayer dialogueDisplay;

	[Header("Witches")]
	public OtherWitch fungusWitch;

	public static GameManager instance = null;

	private Text levelText;
	private GameObject levelPanel;
	private Text tutorialText;
	private GameObject tutorialImage;
	private int level = 0;
	private Vector3 screenPosition;
	private Vector3 offset;
	private GameObject mouseTarget;
	private GameObject shadow;
	private List<GameObject> enemies = new List<GameObject>();
	private ClickHandler clickHandler;
	private TextHandler textBlob;
	private PlayerController player;


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
		levelPanel = GameObject.Find ("LevelPanel");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		tutorialImage = GameObject.Find ("TutorialImage");
		tutorialText = GameObject.Find ("TutorialText").GetComponent<Text> ();

		//start with tutorial image off
		tutorialImage.SetActive(false);

		//start not being able to click on stuff
		clickHandler = GetComponent<ClickHandler>();
		clickHandler.enabled = false;

		textBlob = GetComponent<TextHandler> ();
		dialogueDisplay = GameObject.Find ("DialoguePanel").GetComponent<DialogueDisplayer> ();
		player = GameObject.Find ("Player").GetComponent<PlayerController> ();
	}

	void Start(){
		InitLevel ();
	}
	
	void InitLevel ()
	{
		//disable clicking while we set up
		clickHandler.enabled = false;

		level++;

		//display level text
		levelText.text = textBlob.Get("day") + level;
		levelPanel.SetActive (true);

		//empty enemies list
		enemies.Clear();

		//hide level text and start level
		StartCoroutine("BeginLevel");

	}

	IEnumerator BeginLevel(){
		yield return new WaitForSeconds (levelStartDelay);

		levelPanel.SetActive (false);

		//enable clicking
		clickHandler.enabled = true;

		int switchCase = -1;
		if (tutorialOn) {
			switchCase = level;
		}
		//Switch for level-specific instructions
		switch(switchCase){
		case 1:
			{ 
				//display moving and picking up tutorial
				tutorialText.text = textBlob.Get("pickupTutorial");
				StartCoroutine ("DisplayTutorial");

				InventoryController inventory = GameObject.Find ("Inventory").GetComponent<InventoryController> ();
				while (inventory.Contains ("Plant") < 2) {
					yield return null;
				}

				//display seed planting tutorial
				tutorialText.text = textBlob.Get("plantSeedTutorial");
				StartCoroutine ("DisplayTutorial");

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

				//spawn a single trampler guaranteed to drop a toe
				GameObject enemy = VillagerSpawnPoint.GetComponent<SpawnManager>().SpawnTrampler(1f);
				enemies.Add (enemy);

				//wait
				yield return new WaitForSeconds (5);

				//show hitting tutorial
				tutorialText.text = textBlob.Get("hittingTutorial");
				StartCoroutine("DisplayTutorial");

				//wait for them to scare trampler off
				while (!enemy.GetComponent<TramplerController>().leaving) {
					yield return null;
				}

				tutorialText.text = textBlob.Get("toeDropTutorial");
				StartCoroutine("DisplayTutorial");

				//wait a bit, then end day
				yield return new WaitForSeconds (3);
				EndDay ();
		
				break;
			}
		case 2:
			{
				//combine tomatoes
				tutorialText.text = textBlob.Get("combineTomatoesTutorial");
				StartCoroutine ("DisplayTutorial");

				//wait for tomatoe to appear
				GameObject tomatoe = GameObject.FindGameObjectWithTag ("FightingPlant");
				while (tomatoe == null) {
					tomatoe = GameObject.FindGameObjectWithTag ("FightingPlant");
					yield return null;
				}
				//display tutorial
				tutorialText.text = textBlob.Get("createdTomatoeTutorial");
				StartCoroutine("DisplayTutorial");

				//wait for tomatoe to be planted
				MeleePlantController controller = tomatoe.GetComponent<MeleePlantController>();
				while(controller.planted == false){
					yield return null;
				}
				//display tutorial
				tutorialText.text = textBlob.Get("plantStatTutorial");
				StartCoroutine("DisplayTutorial");

				StartCoroutine ("SpawnWaves");
				StartCoroutine ("WaitForDayEnd");
				break;
			}
		default:
			{
				StartCoroutine ("SpawnWaves");
				StartCoroutine ("WaitForDayEnd");
				break;
			}
		}
	}

	/* DisplayTutorial 
	 * Pauses the game and shows tutorial image and text forever (call HideTutorial to stop it)
	 */
	IEnumerator DisplayTutorial(){
		//pause game and deactivate click handler
		clickHandler.enabled = false;
		Time.timeScale = 0.0f;
		//show tutorial
		tutorialImage.SetActive(true);

		while (true)
			yield return null;
	}

	/* HideTutorial
	 * hides the tutorial image. Called on button press.
	 */
	public void HideTutorial(){
		Debug.Log ("HideTutorial");
		StopCoroutine ("DisplayTutorial");

		tutorialImage.SetActive (false);
		Time.timeScale = 1.0f;

		//re-enable click handler once button has been pressed
		clickHandler.enabled = true;
	}

	/* SpawnWaves
	 * Uses all the spawn stats to create* waves of enemies forever
	 * *get the spawnManager to create
	 */
	IEnumerator SpawnWaves ()
	{
		//wait for startWait
		yield return new WaitForSeconds (startWait);

		//loop until someone stops the coroutine
		while (true)
		{
			//make tramplerCount new enemies
			for (int i = 0; i < tramplerCount; i++)
			{
				//spawn enemy
				GameObject enemy = VillagerSpawnPoint.GetComponent<SpawnManager>().SpawnTrampler();
				//add new enemy to list of enemies
				enemies.Add (enemy);
				//wait between enemies
				yield return new WaitForSeconds (spawnWait);
			}
			//wait between waves
			yield return new WaitForSeconds (waveWait);
		}
	}

	/* WaitForDayEnd
	 * Calls EndLevel(won) if it's still running at the end of the day.
	 */
	IEnumerator WaitForDayEnd ()
	{
		yield return new WaitForSeconds (dayDuration);	
		EndDay (); 
	}

	/* EndLevel
	 * won = true of we won, false if we lost
	 * Called when we reach the end of the level by WaitForDayEnd or by BoardManager if all the grass dies
	 * Clears out enemies
	 */
	public void EndDay()
	{
		StopCoroutine ("WaitForDayEnd");
		StopCoroutine ("SpawnWaves");

		//disable the click handler so the user can't keep doing things
		clickHandler.enabled = false;
		clickHandler.ChillOut ();

		//stop witch moving
		player.GoTo(player.transform.position);

		//get rid of all the enemies
		foreach (GameObject enemy in enemies) {
			enemy.transform.position = VillagerSpawnPoint.transform.position; //move them away to trigger OnTriggerExit before destroy
			Destroy (enemy);
		}

		//Tell relevant game objects the day has ended (they also have lists to clear out)
		player.OnDayEnd ();
		VillagerSpawnPoint.GetComponent<SpawnManager> ().OnDayEnd ();
		foreach (GameObject plant in GameObject.FindGameObjectsWithTag("FightingPlant")) {
			plant.GetComponent<MeleePlantController> ().OnDayEnd();
		}

		//Proceed to nightTime
		BeginNight();
	}

	/* BeginNight
	 * Ages plants
	 */
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

		StartCoroutine ("EncounterWitch");

	}


	IEnumerator EncounterWitch(){
		yield return new WaitForSeconds (2);

		dialogueDisplay.StartConversation (fungusWitch);
		while (dialogueDisplay.gameObject.activeSelf == true) {
			yield return null;
		}

		//eventually wait while player does stuff here?

		//start next level
		InitLevel();
	}


	/* GameOver
	 * Called by BoardManager when there's no grass left
	 */
	public void GameOver(){
		EndDay ();
		//a temporary measure
		tutorialText.text = textBlob.Get("gameOver");
		StartCoroutine ("DisplayTutorial");
	}
		
}