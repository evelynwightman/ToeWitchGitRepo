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

	void SetUpText(string questionText, string yesText, string noText){
		question.text = questionText;

		this.yesButton.transform.GetComponentInChildren<Text> ().text = yesText;

		this.noButton.transform.GetComponentInChildren<Text> ().text = noText;
	}

	public void PoseQuestion(string question, UnityAction yesAction){
		SetUpText (question, textBlob.Get ("yes"), textBlob.Get ("no"));
		Choice(yesAction, new UnityAction(NeverMind));
	}

	void NeverMind(){
		this.gameObject.SetActive (false);
	}
}
