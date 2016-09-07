/* PickupController
 * Evelyn Wightman 2016
 * Attached to anything that can be picked up. Handles being picked up, managing its shadow, and knowing where it can go.
 */
using UnityEngine;
using System.Collections;

public class PickupController : MonoBehaviour {
	public bool inInventory = false;
	public bool pickable;
	public GameObject pot = null;

	protected GameObject shadow;
	protected BoardManager boardManager;

	private GameObject canvas = null;

	void Awake(){
		pickable = true;
	}

	void Start(){
		shadow = transform.FindChild("Shadow").gameObject;
		SpriteRenderer shadowRenderer = shadow.GetComponent<SpriteRenderer> ();
		boardManager = GameObject.Find ("Board").GetComponent<BoardManager> ();
		shadowRenderer.sprite = GetComponent<SpriteRenderer> ().sprite;
		shadowRenderer.enabled = false;
		shadowRenderer.color = new Color (1f, 1f, 1f, .5f);
		if (transform.FindChild ("PlantCanvas") != null) {
			canvas = transform.FindChild ("PlantCanvas").gameObject;
		}
	}

	void Update(){

		//If position has changed, update sorting order accordingly.
		if (transform.hasChanged) {
			//if we're in a pot we should be in the same sorting layer as it.
			if (pot != null) {
				GetComponent<SpriteRenderer> ().sortingOrder = pot.GetComponent<SpriteRenderer> ().sortingOrder + 1;
				//tell the pot we are in it.
				pot.GetComponent<PotController>().setPlant(transform.gameObject);
			}
			//update sorting order
			else GetComponent<SpriteRenderer> ().sortingOrder = Mathf.RoundToInt ((transform.position.y-.5f) * 100f) * -1;
			transform.hasChanged = false;

			//including the canvas if we have one
			if (canvas != null){
				canvas.GetComponent<Canvas> ().sortingOrder = Mathf.RoundToInt ((transform.position.y-.5f) * 100f) * -1;
			}
		}
	}

	/* OnTriggerEnter2D
	 * Says "Pick me!" to they player when she bumps into us.
	 */
	void OnTriggerEnter2D(Collider2D other){
		//if the player runs into us and we can be picked up
		if (other.tag == "Player" && pickable) {
			//tell them to pick us up
			other.gameObject.GetComponent<PlayerController> ().PickUp (this.gameObject);
		}
	}

	/* ICanGoHere
	 * Decides whether we can be put at given location based on our tag. Returns true if we can go there, false if not.
	 */
	public bool ICanGoHere(Vector3 location){
		//If we are grass
		if (transform.tag == "Grass") {
			//we must go on the lawn
			if (boardManager.IsInYard (location) && !boardManager.IsInNursery (location))
				return true;
			else {
				return false;
			}
		}
		//If we are a plant
		if (transform.tag == "FightingPlant" || transform.tag == "Plant") {
			//if we are a regular plant we must go in a pot in the nursery
			if (transform.tag == "Plant") {
				GameObject perhapsAPot = FindPot (location);
				if (boardManager.IsInNursery (location) && !boardManager.IsOnPorch (location) && perhapsAPot != null) {
					//the pot must be empty
					if (!perhapsAPot.GetComponent<PotController>().hasPlant())
						return true;
				}
				return false;
			} 

			//if we are a fighting plant we must go on the lawn
			if (transform.tag == "FightingPlant" && boardManager.IsInYard (location) 
				&& !boardManager.IsInNursery (location))
				return true;
			else {
				return false;
			}
				
		} 
		//If we are a toe
		else if (transform.tag == "Toe") {
			//We can go on top of a plant
			if (GetComponentInChildren<ObjectCombiner> ().FindPlant (transform.FindChild("Shadow").position) != null) {
				//but it can't be a seed
				if (!GetComponentInChildren<ObjectCombiner> ().FindPlant (transform.FindChild ("Shadow").position).
					GetComponent<FloraController> ().isSeed) {
					return true;
				}
			}
		}
		return false;
	}

	/* FindPot
	 * Returns true if there's a pot at current location, false otherwise.
	 */
	GameObject FindPot(Vector3 location){
		//Find all the things that are also on this spot
		RaycastHit2D[] hits;
		Ray ray = new Ray (Camera.main.transform.position, location - Camera.main.transform.position);
		hits = Physics2D.RaycastAll (ray.origin, ray.direction);

		//If any of them are a pot, return the pot
		foreach (UnityEngine.RaycastHit2D item in hits){
			if (item.transform.tag == "Pot") {
				return item.transform.gameObject;
			}
		}
		return null;
	}

	/* ReturnShadow
	 * Turns off shadow and puts it back on top of us.
	 */
	public void returnShadow(){
		shadow.GetComponent<SpriteRenderer>().enabled = false;
		shadow.transform.position = transform.position;
	}

	/* Bounce
	 * Makes us get bigger and then smaller again. A bit of visual bounce for emphasis.
	 */
	public IEnumerator Bounce(){
		Vector3 size = transform.localScale;
		float increase = 1.25f; //ugh, hard-coded numbers?
		float bounceSpeed = 2f;

		//increase size
		while (!Mathf.Approximately(transform.localScale.magnitude, (size * increase).magnitude)) {
			transform.localScale = Vector3.MoveTowards (transform.localScale, size * increase, bounceSpeed * Time.deltaTime);
			yield return null;
		}
		//decrease size
		while (!Mathf.Approximately(transform.localScale.magnitude, size.magnitude)) {
			transform.localScale = Vector3.MoveTowards (transform.localScale, size, bounceSpeed * Time.deltaTime);
			yield return null;
		}
	}
}
