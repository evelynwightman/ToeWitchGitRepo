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
	private OtherWitch witch;
	private GameObject tradeOffer;
	private GameObject tradeSuggestion;
	private InventoryController inventory;

	void Start(){
		textBlob = GameObject.Find("GameManager").GetComponent<TextHandler> ();
		if (textBlob == null) {
			Debug.LogError ("Dialogue Displayer could not find TextHandler");
		}
		inventory = GameObject.Find ("Inventory").GetComponent<InventoryController> ();
		if (inventory == null) {
			Debug.LogError ("Dialogue Displayer could not find InventoryController");
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

	public void StartConversation(OtherWitch otherWitch){
		Time.timeScale = 0.0f;
		witch = otherWitch;

		//get a random item to offer up for trade from this witch's stash
		tradeOffer = witch.tradableItems [Random.Range (0, witch.tradableItems.Count)];

		SetUpText(textBlob.Get("fungusWitchGreeting"), textBlob.Get("toeWitchGreetingYes"), 
			textBlob.Get("toeWitchGreetingNo"));
		Choice(new UnityAction(OfferTrade), new UnityAction(Leave));
	}

	void OfferTrade(){
		string tradeOfferText = textBlob.Get ("fungusWitchOfferTrade");

		//get a random item to suggest for trade from ToeWitch's inventory
		tradeSuggestion = inventory.GetRandom ();
		//if ToeWitch's inventory is empty, offer the item as a gift instead
		if (tradeSuggestion == null) {
			OfferGift ();
			return;
		}

		tradeOfferText = tradeOfferText.Replace ("*", tradeOffer.tag);
		tradeOfferText = tradeOfferText.Replace ("#", tradeSuggestion.tag);

		SetUpText(tradeOfferText, textBlob.Get("toeWitchTradeYes"), 
			textBlob.Get("toeWitchTradeNo"), textBlob.Get("toeWitchTradeAnother"));
		Choice(new UnityAction(ExecuteTrade), new UnityAction(RefuseTrade), new UnityAction(OfferTrade));
	}

	void OfferGift(){
		string giftOfferText = textBlob.Get ("fungusWitchOfferGift");
		giftOfferText = giftOfferText.Replace ("*", tradeOffer.tag);

		SetUpText(giftOfferText, textBlob.Get("toeWitchGiftYes"));
		Choice(new UnityAction (ExecuteGift));

	}

	void ExecuteTrade(){
		inventory.Add (Instantiate (tradeOffer));
		inventory.RemoveAndDestroy (tradeSuggestion);

		SetUpText (textBlob.Get("fungusWitchExecuteTrade"), textBlob.Get("toeWitchClose"));
		Choice (new UnityAction (Close));
	}

	void ExecuteGift(){
		GameObject item = Instantiate (tradeOffer);
		inventory.Add (item);

		SetUpText (textBlob.Get("fungusWitchExecuteGift"), textBlob.Get("toeWitchClose"));
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
		Time.timeScale = 1.0f;
		this.gameObject.SetActive (false);
	}


}
