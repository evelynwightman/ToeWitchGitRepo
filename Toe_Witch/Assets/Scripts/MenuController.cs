/* MenuController
 * Evelyn Wightman 2016
 * Handles the main menu.
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class MenuController : MonoBehaviour {

	public GameObject menuPanel;
	public GameObject menuToggleButton;
	public float speed;
	public GameObject yesNo;

	private Vector3 onPosition = new Vector3(-200f, -70f, 0f);
	private Vector3 offPosition = new Vector3(-760f, -120f, 0f);
	private TextHandler textBlobs;

	void Start(){
		menuPanel.SetActive (true);
		transform.position = onPosition;

		textBlobs = GameObject.Find ("GameManager").GetComponent<TextHandler> ();

		menuToggleButton.transform.GetComponentInChildren<Text> ().text = textBlobs.Get ("menuClose");
	}

	/* ToggleMenu
	 * if menu is on, turn it off, if it's off, turn it on.
	 */
	public void ToggleMenu(){
		if (transform.position == onPosition) {
			menuPanel.SetActive (false);
			StartCoroutine ("SlideOffMenu");

		} else {
			menuPanel.SetActive (true); 
			StartCoroutine ("SlideOnMenu");
		}
	}

	/* SlideOnMenu
	 * Moves menu into onscreen position
	 */
	IEnumerator SlideOnMenu(){
		while (!Mathf.Approximately (transform.position.magnitude, onPosition.magnitude)) {
			Time.timeScale = 1.0f; //un-pause if soemthing else has paused;
			transform.position = Vector3.MoveTowards (transform.position, onPosition, speed * Time.deltaTime);
			yield return null;
		}
		//pause game while menu is on
		Time.timeScale = 0.0f;

		menuToggleButton.transform.GetComponentInChildren<Text> ().text = textBlobs.Get ("menuClose");
	}

	/* SlideOnMenu
	 * Moves menu into offscreen position
	 */
	IEnumerator SlideOffMenu(){
		//resume game
		Time.timeScale = 1.0f;

		while (!Mathf.Approximately (transform.position.magnitude, offPosition.magnitude)) {
			transform.position = Vector3.MoveTowards (transform.position, offPosition, speed * Time.deltaTime);
			yield return null;
		}
		menuToggleButton.transform.GetComponentInChildren<Text> ().text = textBlobs.Get ("menuOpen");
	}

	/* OfferQuit
	 * Pops up a 'yes/no' box to make sure the user wants to quit.
	 */
	public void OfferQuit(){
		yesNo.transform.GetComponent<YesNoHandler> ().PoseQuestion (textBlobs.Get ("quitQuery"), new UnityAction (QuitGame));
	}

	/* QuitGame
	 */
	void QuitGame(){
		Application.Quit();
	}
}
