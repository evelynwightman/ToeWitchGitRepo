/* Game Controller
 * Evelyn Wightman 2016
 * Modified from Done_GameController.cs from Unity Space Shooter Tutorial
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
		level++;
		InitGame();
	}
		
	void OnLevelWasLoaded(int index)
	{
		//I don't see why I need this here since it's also in Awake, but OKAY :C
		if (instance != this) {
			Destroy (this.gameObject);
			return;
		}
		
		level++;

		//find level text
		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();

		InitGame();
	}
	
	void InitGame ()
	{
		//may want to disable click handler while we set up

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
				VillagerSpawnPoint.GetComponent<SpawnManager>().SpawnTrampler();
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
		EndLevel (true); //we won
	}

	/* EndLevel
	 * won = true of we won, false if we lost
	 * Called when we reach the end of the level by WaitForDayEnd or by BoardManager if all the grass dies
	 */
	public void EndLevel(bool won)
	{
		StopCoroutine ("WaitForDayEnd");
		StopCoroutine ("SpawnWaves");

		SceneManager.LoadScene("Main");
	}
		
}