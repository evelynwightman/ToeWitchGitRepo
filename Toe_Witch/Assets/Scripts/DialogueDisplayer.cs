using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class DialogueDisplayer : MonoBehaviour {

	public Text otherWitchDialogue;
	public Image otherWitchIcon;
	public Button yesButton;
	public Button noButton;
	public Button leaveButton;

	private TextHandler textBlob;

	void Start(){
		textBlob = GameObject.Find("GameManager").GetComponent<TextHandler> ();
		if (textBlob == null) {
			Debug.LogError ("Dialogue Displayer could not find TextHandler");
		}
		this.gameObject.SetActive (false);
	}

	void Choice(UnityAction yesEvent, UnityAction noEvent = null, UnityAction leaveEvent = null){
		this.gameObject.SetActive (true);

		yesButton.onClick.RemoveAllListeners ();
		yesButton.onClick.AddListener (yesEvent);

		noButton.onClick.RemoveAllListeners ();
		if(noEvent != null)
			noButton.onClick.AddListener (noEvent);

		leaveButton.onClick.RemoveAllListeners ();
		if(leaveEvent != null)
			leaveButton.onClick.AddListener (leaveEvent);
	}

	void SetUpText(string otherWitchDialogue, string yesText, string noText = "", string leaveText = ""){
		this.otherWitchDialogue.text = otherWitchDialogue;

		this.yesButton.transform.GetComponentInChildren<Text> ().text = yesText;

		if (noText == "")
			noButton.gameObject.SetActive (false);
		else {
			noButton.gameObject.SetActive (true);
			this.noButton.transform.GetComponentInChildren<Text> ().text = noText;
		}

		if (leaveText == "")
			leaveButton.gameObject.SetActive (false);
		else {
			leaveButton.gameObject.SetActive (true);
			this.leaveButton.transform.GetComponentInChildren<Text> ().text = leaveText;
		}
	}

	public void StartConversation(){
		SetUpText(textBlob.Get("fungusWitchGreeting"), textBlob.Get("toeWitchGreetingYes"), 
			textBlob.Get("toeWitchGreetingNo"));
		Choice(new UnityAction(OfferTrade), new UnityAction(Leave));
	}

	void OfferTrade(){
		SetUpText(textBlob.Get("fungusWitchOfferTrade"), textBlob.Get("toeWitchTradeYes"), 
			textBlob.Get("toeWitchTradeNo"), textBlob.Get("toeWitchTradeAnother"));
		Choice(new UnityAction(ExecuteTrade), new UnityAction(RefuseTrade), new UnityAction(OfferTrade));
	}

	void ExecuteTrade(){
		SetUpText (textBlob.Get("fungusWitchExecuteTrade"), textBlob.Get("toeWitchClose"));
		Choice (new UnityAction (Close));
	}

	void RefuseTrade(){
		SetUpText (textBlob.Get("fungusWitchRefuseTrade"), textBlob.Get("toeWitchClose"));
		Choice (new UnityAction (Close));
	}

	void Leave(){
		SetUpText (textBlob.Get("fungusWitchLeave"), textBlob.Get("toeWitchClose"));
		Choice (new UnityAction (Close));
	}

	void Close(){
		this.gameObject.SetActive (false);
	}


}
