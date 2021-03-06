/* Game Manager
 * Modified (very heavily) from Done_GameController.cs from Unity Space Shooter Tutorial
 * Controls level flow.
 * 
 * Copyright (c) 2016 by Evelyn Wightman. All rights reserved. 
 * Subject to the terms and conditions contained in LICENSE file.
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
	public GameObject ChildSpawnPoint;
	public DialogueDisplayer dialogueDisplay;
	public BoardManager boardManager;
	//these stats are level-dependent and get changed in the script
	public int tramplerCount;
	public float spawnWait;
	public float startWait;
	public float waveWait; 
	public float dayDuration;


	[Header("Witches")]
	public OtherWitch fungusWitch;

	[Header("Lighting")]
	public Animator fixedLight;
	public AnimationClip dayCycleLightingAnimationClip;
	public Animator trackingLight;
	public AnimationClip sunTrackingAnimationClip;
	public Animator houseLight;

	[Header("Music")]
	public AudioClip dayMusic;
	public AudioClip nightMusic;

	[Header("Items")]
	public List<GameObject> thingsToPutInDemo;
	public GameObject seedPrefab;

	[Header("Cheatsy Dev Stuff")]
	public int cheatLevel;

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
	private AudioSource audioSource;
	public bool tutorialOn = true; //public for testing
	private IEnumerator spawnCoroutine;
	private bool gameInProgress = false;


	void Awake()
	{
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
		audioSource = GetComponent<AudioSource> ();

		spawnCoroutine = SpawnWaves (0f);
	}

	/* StartGame
	 * Sets level to 0 and calls InitLevel. From here every step will call its own next step. 
	 */
	void StartGame(){
		gameInProgress = true;
		for (int i = 0; i < 50; i++) {
			Instantiate (seedPrefab, new Vector3(5f, 2f, 0f), Quaternion.identity);
		}
		boardManager.PlacePots (2);
		level = 0;
		InitLevel ();
	}

	/* StartDemo
	 * Instantiates demo items and starts game.
	 */
	public void StartDemo(){
		foreach (GameObject thing in thingsToPutInDemo) {
			Instantiate (thing);
		}
		StartGame ();
	}

	/* InitLevel
	 * Sets up level and calls coroutine to actually start it.
	 */
	void InitLevel ()
	{
		//disable clicking while we set up
		clickHandler.enabled = false;

		level++;

		//display level text
		levelText.text = textBlob.Get("day") + level;
		levelPanel.SetActive (true);

		//start music
		audioSource.clip = dayMusic;
		audioSource.Play ();

		//empty enemies list
		enemies.Clear();

		//set up lighting animations
		float animationSpeed = dayCycleLightingAnimationClip.length/dayDuration;
		fixedLight.speed = animationSpeed;

		animationSpeed = sunTrackingAnimationClip.length / dayDuration;
		trackingLight.speed = animationSpeed;

		//hide level text and start level
		StartCoroutine("BeginLevel");

	}

	/* BeginLevel
	 * Starts the level and controls what happens as the level progresses based on what level we're at.
	 * Either calls WaitForDayEnd which will end the day after enough time has passed, or calls EndDay directly. 
	 */
	IEnumerator BeginLevel(){
		yield return new WaitForSeconds (levelStartDelay);

		levelPanel.SetActive (false);

		//enable clicking
		clickHandler.enabled = true;


		int switchCase = level;

		//Switch for level-specific instructions
		switch(switchCase){
		case 1:
			{
				//start lights going on the right animation, then stop them for fine control
				fixedLight.SetTrigger("startDayCycle");
				trackingLight.SetTrigger ("startDayCycle");
				houseLight.SetTrigger ("HouseLightOff");
				fixedLight.speed = 0f;
				trackingLight.speed = 0f;
				StartCoroutine (ProgressQuarterDay ());

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

				StartCoroutine (ProgressQuarterDay ());

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

				StartCoroutine (ProgressQuarterDay ());

				//wait
				yield return new WaitForSeconds (5);

				//show hitting tutorial
				tutorialText.text = textBlob.Get("hittingTutorial");
				StartCoroutine("DisplayTutorial");

				StartCoroutine (ProgressQuarterDay ());
				yield return new WaitForSeconds (3);

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
				//set stats
				tramplerCount = 2;
				spawnWait = 4;
				startWait = 0;
				waveWait = 40;
				dayDuration = 40;

				//start lights going on the right animation, then stop them for fine control
				fixedLight.SetTrigger("startDayCycle");
				trackingLight.SetTrigger ("startDayCycle");
				houseLight.SetTrigger ("HouseLightOff");
				fixedLight.speed = 0f;
				trackingLight.speed = 0f;
				StartCoroutine (ProgressQuarterDay ());

				//combine tomatoes
				tutorialText.text = textBlob.Get("combineTomatoesTutorial");
				StartCoroutine ("DisplayTutorial");

				//wait for tomatoe to appear
				GameObject tomatoe = GameObject.FindGameObjectWithTag ("FightingPlant");
				while (tomatoe == null) {
					tomatoe = GameObject.FindGameObjectWithTag ("FightingPlant");
					yield return null;
				}
				Debug.Log ("FightingPlant: " + tomatoe);
				//display tutorial
				tutorialText.text = textBlob.Get("createdTomatoeTutorial");
				StartCoroutine("DisplayTutorial");

				StartCoroutine (ProgressQuarterDay ());

				//wait for tomatoe to be planted
				MeleePlantController controller = tomatoe.GetComponent<MeleePlantController>();
				while(controller.planted == false){
					yield return null;
				}
				//display tutorial
				tutorialText.text = textBlob.Get("plantStatTutorial");
				StartCoroutine("DisplayTutorial");

				StartCoroutine (ProgressQuarterDay (dayDuration/4));

				//spawn waves with no chance of children
				spawnCoroutine = SpawnWaves (0f);
				StartCoroutine (spawnCoroutine);
				StartCoroutine (ProgressQuarterDay (dayDuration/4));
				StartCoroutine(WaitForDayEnd(dayDuration/1.9f));
				break;
			}
		case 3:
			{
				//set stats
				tramplerCount = 2;
				spawnWait = 4;
				startWait = 0;
				waveWait = 20;
				dayDuration = 40;

				//start lights going
				fixedLight.SetTrigger("startDayCycle");
				trackingLight.SetTrigger ("startDayCycle");
				houseLight.SetTrigger ("HouseLightOff");

				//wait for day end
				StartCoroutine ("WaitForDayEnd");

				//spawn waves of children
				spawnCoroutine = SpawnWaves (1f);
				StartCoroutine (spawnCoroutine);

				break;
			}
		case 4:
			{
				dayDuration = 60;
				//add more pots
				boardManager.PlacePots (4);

				//start lights going
				fixedLight.SetTrigger("startDayCycle");
				trackingLight.SetTrigger ("startDayCycle");
				houseLight.SetTrigger ("HouseLightOff");

				//wait for day end
				StartCoroutine ("WaitForDayEnd");

				//spawn mixed child/villager waves
				spawnCoroutine = SpawnWaves (.5f);
				StartCoroutine (spawnCoroutine);

				tutorialText.text = textBlob.Get ("addPots");
				StartCoroutine ("DisplayTutorial");

				//wait for that tutorial to be clicked away
				while (!clickHandler.enabled)
					yield return null;

				tutorialText.text = textBlob.Get ("endTutorial");
				StartCoroutine ("DisplayTutorial");

				break;
			}
		default:
			{
				spawnCoroutine = SpawnWaves (.5f);
				StartCoroutine (spawnCoroutine);
				StartCoroutine ("WaitForDayEnd");

				//start lights going
				fixedLight.SetTrigger("startDayCycle");
				trackingLight.SetTrigger ("startDayCycle");
				houseLight.SetTrigger ("HouseLightOff");
				break;
			}
		}
	}

	/* DisplayTutorial 
	 * Pauses the game and shows tutorial image and text forever (call HideTutorial to stop it)
	 */
	IEnumerator DisplayTutorial(){
		//don't show if tutorial is turned off
		if (tutorialOn) {

			//pause game and deactivate click handler
			clickHandler.enabled = false;
			Time.timeScale = 0.0f;
			//show tutorial
			tutorialImage.SetActive (true);

			while (true)
				yield return null;
		}
	}

	/* HideTutorial
	 * hides the tutorial image. Called on button press.
	 */
	public void HideTutorial(){
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
	IEnumerator SpawnWaves (float probChild)
	{
		//wait for startWait
		yield return new WaitForSeconds (startWait);

		//loop until someone stops the coroutine
		while (true)
		{
			//make tramplerCount new enemies
			for (int i = 0; i < tramplerCount; i++)
			{
				GameObject enemy;
				//spawn enemy
				//probChild probability it's a child
				if (Random.Range (0f, 1f) < probChild) {
					enemy = ChildSpawnPoint.GetComponent<SpawnManager>().SpawnTrampler();
				}
				else{
					enemy = VillagerSpawnPoint.GetComponent<SpawnManager>().SpawnTrampler();
				}

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
	 * TWO OVERLOADS. Optional time to wait in seconds. Default is dayDuration.
	 */
	IEnumerator WaitForDayEnd(){
		return WaitForDayEnd (dayDuration);
	}
	IEnumerator WaitForDayEnd (float time)
	{
		yield return new WaitForSeconds (time);	
		EndDay (); 
	}

	/* EndLevel
	 * won = true of we won, false if we lost
	 * Called when we reach the end of the level by WaitForDayEnd or by BoardManager if all the grass dies
	 * Clears out enemies
	 * Calls BeginNight when done.
	 */
	public void EndDay()
	{
		StopCoroutine ("WaitForDayEnd");
		StopCoroutine (spawnCoroutine);

		//disable the click handler so the user can't keep doing things
		clickHandler.enabled = false;
		clickHandler.ChillOut ();

		//stop witch moving
		player.GoTo(player.transform.position);

		//tell all enemies to flee
		foreach (GameObject enemy in enemies) {
			if (enemy != null)
				enemy.GetComponent<TramplerController> ().leaving = true;
		}

		//Tell relevant game objects the day has ended (they also have lists to clear out)
		player.OnDayEnd ();
		VillagerSpawnPoint.GetComponent<SpawnManager> ().OnDayEnd ();
		foreach (GameObject plant in GameObject.FindGameObjectsWithTag("FightingPlant")) {
			MeleePlantController con = plant.GetComponent<MeleePlantController> ();
			if(con != null)
				con.OnDayEnd();
		}

		//Proceed to nightTime
		StartCoroutine(BeginNight());
	}

	/* BeginNight
	 * Ages plants, calls EncounterWitch.
	 */
	IEnumerator BeginNight(){

		//make sure night lights are up
		trackingLight.speed = 1f;
		trackingLight.SetTrigger("startNight");
		fixedLight.speed = 1f;
		fixedLight.SetTrigger ("startNight");
		houseLight.SetTrigger ("HouseLightOn");

		//fade out music
		StartCoroutine(FadeOut());

		yield return new WaitForSeconds (2f);

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

		//get rid of all the enemies
		foreach (GameObject enemy in enemies) {
			enemy.transform.position = VillagerSpawnPoint.transform.position; //move them away to trigger OnTriggerExit before destroy
			Destroy (enemy);
		}

		StartCoroutine ("EncounterWitch");
	}

	/* EncounterWitch
	 * gets a witch and starts dialogue with her.
	 * Calls InitLevel to start next day when done.
	 */
	IEnumerator EncounterWitch(){
		yield return new WaitForSeconds (1);
		//start night music
		audioSource.volume = 1;
		audioSource.clip = nightMusic;
		audioSource.Play();

		//eventually get a witch at random here.
		dialogueDisplay.StartConversation (fungusWitch);
		while (dialogueDisplay.gameObject.activeSelf == true) {
			yield return null;
		}

		//start next level
		InitLevel();
	}


	/* GameOver
	 * Called by BoardManager when there's no grass left
	 */
	public IEnumerator GameOver(){
		//display game over message
		tutorialText.text = textBlob.Get("gameOver");
		StartCoroutine ("DisplayTutorial");

		//wait for that tutorial to be clicked away
		while (!clickHandler.enabled)
			yield return null;

		//reload
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	/* ToggleTutorial
	 * What it says on the tin. Called by button press in menu.
	 */
	public void ToggleTutorial(){
		if (tutorialOn == true) {
			tutorialOn = false;
		} else
			tutorialOn = true;
	}

	/* ProgressQuarterDay
	 * progress through a quarter of the day's lighting cycle in x seconds (default is 5)
	 * animation must start with speed == 0
	 */
	IEnumerator ProgressQuarterDay(float x = 5f){
		//wait for current animation to stop
		while (fixedLight.speed != 0f) {
			yield return null;
		}
		//set speeds and start animations
		float animationSpeed = dayCycleLightingAnimationClip.length/ (4f*x);
		fixedLight.speed = animationSpeed;

		animationSpeed = sunTrackingAnimationClip.length / (4f*x);
		trackingLight.speed = animationSpeed;

		//wait x seconds
		yield return new WaitForSeconds(x);

		//stop animations
		fixedLight.speed = 0f;
		trackingLight.speed = 0f;
	}

	/* FadeOut
	 * fades out the audio.
	 */
	IEnumerator FadeOut(){
		while (audioSource.volume > 0f) {
			audioSource.volume -= 0.01f;
			yield return null;
		}
	}

	/* Cheat
	 * Skip to a set level for debugging
	 */
	public void Cheat(){
		level = cheatLevel;
		InitLevel ();
	}

	/* ShowCredits
	 * Displays credits in tutorial box. Called by button press in menu.
	 */
	public void ShowCredits(){
		tutorialText.text = textBlob.Get("credits");
		StartCoroutine ("DisplayTutorial");
	}

	/* NewGameButton
	 * Starts a new game. Reloads scene if a game is already in progress.
	 */
	public void NewGameButton(){
		if (gameInProgress) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		} else
			StartGame ();
	}
		
}