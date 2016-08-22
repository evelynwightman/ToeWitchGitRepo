/* DialogueDisplayer
 * Evelyn Wightman 2016
 * Attached to DialoguePanel. Handles dialogue flow (provides otherWitch text, sets response options, repeats). 
 * Called by GameManager via public fcn StartConversation. At each dialogue step, text is set and each button is 
 * a UnityAction which calls the next step in the conversation flow.
 */
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

	/* Choice
	 * Set buttons to perform the given actions on click.
	 */
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

	/* SetUpText
	 * Set the text of the other witch's dialogue and all relevant buttons.
	 */
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

	/* StartConversation
	 * Initiates conversation flow with given witch. 
	 */
	public void StartConversation(OtherWitch otherWitch){
		//pause game
		Time.timeScale = 0.0f;

		witch = otherWitch;

		//get a random item to offer up for trade from this witch's stash
		tradeOffer = witch.tradableItems [Random.Range (0, witch.tradableItems.Count)];

		//set up greeting and hand it over to the user to chose the next step.
		SetUpText(textBlob.Get("fungusWitchGreeting"), textBlob.Get("toeWitchGreetingYes"), 
			textBlob.Get("toeWitchGreetingNo"));
		Choice(new UnityAction(OfferTrade), new UnityAction(Leave));
	}

	/* OfferTrade
	 */
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

	/* OfferGift
	 */
	void OfferGift(){
		string giftOfferText = textBlob.Get ("fungusWitchOfferGift");
		giftOfferText = giftOfferText.Replace ("*", tradeOffer.tag);

		SetUpText(giftOfferText, textBlob.Get("toeWitchGiftYes"));
		Choice(new UnityAction (ExecuteGift));

	}

	/* ExecuteTrade
	 * Removes the trade suggestion from inventory and adds the tradeOffer item. Also continues convo flow.
	 */
	void ExecuteTrade(){
		GameObject trade = Instantiate (tradeOffer);
		inventory.RemoveAndDestroy (tradeSuggestion);
		inventory.Add (trade);

		SetUpText (textBlob.Get("fungusWitchExecuteTrade"), textBlob.Get("toeWitchClose"));
		Choice (new UnityAction (Close));
	}

	/* ExecuteGift
	 * Adds the gift item to inventory. Also continues convo flow.
	 */
	void ExecuteGift(){
		GameObject item = Instantiate (tradeOffer);
		inventory.Add (item);

		SetUpText (textBlob.Get("fungusWitchExecuteGift"), textBlob.Get("toeWitchClose"));
		Choice (new UnityAction (Close));
	}

	/* RefuseTrade
	 */
	void RefuseTrade(){
		SetUpText (textBlob.Get("fungusWitchRefuseTrade"), textBlob.Get("toeWitchClose"));
		Choice (new UnityAction (Close));
	}

	/* Leave
	 */
	void Leave(){
		SetUpText (textBlob.Get("fungusWitchLeave"), textBlob.Get("toeWitchClose"));
		Choice (new UnityAction (Close));
	}

	/* Close
	 * Resumes game and closes dialogue window.
	 */
	void Close(){
		Time.timeScale = 1.0f;
		this.gameObject.SetActive (false);
	}


}
