/* YesNoHandler
 * Evelyn Wightman 2016
 * Attached to a yes/no popup box. Public function PoseQuestion allows outside script to set question text and 
 * desired 'yes' outcome. 'No' outcome is always nothing.
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class YesNoHandler : MonoBehaviour {

	public Text question;
	public Button yesButton;
	public Button noButton;

	private TextHandler textBlob;

	void Start(){
		textBlob = GameObject.Find("GameManager").GetComponent<TextHandler> ();
		if (textBlob == null) {
			Debug.LogError ("Dialogue Displayer could not find TextHandler");
		}

		this.gameObject.SetActive (false);
	}

	void Choice(UnityAction yesEvent, UnityAction noEvent = null){
		this.gameObject.SetActive (true);

		yesButton.onClick.RemoveAllListeners ();
		yesButton.onClick.AddListener (yesEvent);

		noButton.onClick.RemoveAllListeners ();
		noButton.onClick.AddListener (noEvent);
	}

	/* SetUpText
	 * Sets the text on the box to given strings (question text, yes button text, and no button text)
	 */
	void SetUpText(string questionText, string yesText, string noText){
		question.text = questionText;

		this.yesButton.transform.GetComponentInChildren<Text> ().text = yesText;

		this.noButton.transform.GetComponentInChildren<Text> ().text = noText;
	}

	/* PoseQuestion
	 * Takes in a question to ask the user and the action to take if the user clicks yes.
	 */
	public void PoseQuestion(string question, UnityAction yesAction){
		SetUpText (question, textBlob.Get ("yes"), textBlob.Get ("no"));
		Choice(yesAction, new UnityAction(NeverMind));
	}

	/* NeverMind
	 * The 'no' action: turn off the question box and do nothing.
	 */
	void NeverMind(){
		this.gameObject.SetActive (false);
	}
}
