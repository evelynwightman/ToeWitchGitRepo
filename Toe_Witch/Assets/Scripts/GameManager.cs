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
	private int level = 0;
	private Vector3 screenPosition;
	private Vector3 offset;
	private GameObject mouseTarget;
	private GameObject shadow;
	private List<GameObject> enemies = new List<GameObject>();


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

		//find level text
		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();

		//Start the game
		InitLevel();
	}
	
	void InitLevel ()
	{
		//may want to disable click handler while we set up

		level++;

		//display level text
		levelText.text = "Day" + level;
		levelImage.SetActive (true);

		//hide level text and start level
		Invoke("BeginLevel", levelStartDelay);

	}

	void BeginLevel(){
		levelImage.SetActive (false);
		StartCoroutine ("SpawnWaves");
		StartCoroutine ("WaitForDayEnd");
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
			Destroy (enemy);
		}

		//Proceed to nightTime
		BeginNight();
	}

	void BeginNight(){
		//Age all plants
		List<GameObject> allPlants = GameObject.FindGameObjectsWithTag("Plant").ToList();
		List<GameObject> fightingPlants = GameObject.FindGameObjectsWithTag("FightingPlant").ToList();
		allPlants.Concat (fightingPlants);
		Debug.Log ("plants size " + allPlants.Count);
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