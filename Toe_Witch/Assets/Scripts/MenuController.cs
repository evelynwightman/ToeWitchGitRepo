using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour {

	public GameObject menuPanel;
	public GameObject menuToggleButton;
	public float speed;

	private Vector3 onPosition = new Vector3(-200f, -70f, 0f);
	private Vector3 offPosition = new Vector3(-760f, -70f, 0f);
	private TextHandler textBlobs;

	void Start(){
		menuPanel.SetActive (false);
		transform.position = offPosition;

		textBlobs = GameObject.Find ("GameManager").GetComponent<TextHandler> ();

		menuToggleButton.transform.GetComponentInChildren<Text> ().text = textBlobs.Get ("menuOpen");
	}

	public void ToggleMenu(){
		Debug.Log ("Toggle");
		if (transform.position == onPosition) {
			menuPanel.SetActive (false);
			StartCoroutine ("SlideOffMenu");

		} else {
			menuPanel.SetActive (true);

			StartCoroutine ("SlideOnMenu");
		}
	}

	IEnumerator SlideOnMenu(){
		while (!Mathf.Approximately (transform.position.magnitude, onPosition.magnitude)) {
			transform.position = Vector3.MoveTowards (transform.position, onPosition, speed * Time.deltaTime);
			yield return null;
		}
		menuToggleButton.transform.GetComponentInChildren<Text> ().text = textBlobs.Get ("menuClose");
	}

	IEnumerator SlideOffMenu(){
		Debug.Log ("SlideOffMenu");
		while (!Mathf.Approximately (transform.position.magnitude, offPosition.magnitude)) {
			transform.position = Vector3.MoveTowards (transform.position, offPosition, speed * Time.deltaTime);
			yield return null;
		}
		menuToggleButton.transform.GetComponentInChildren<Text> ().text = textBlobs.Get ("menuOpen");
	}
}
