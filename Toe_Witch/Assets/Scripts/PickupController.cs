using UnityEngine;
using System.Collections;

public class PickupController : MonoBehaviour {
	public bool inInventory = false;
	public bool pickable;

	protected GameObject shadow;
	protected BoardManager boardManager;

	void Start(){
		shadow = transform.FindChild("Shadow").gameObject;
		SpriteRenderer shadowRenderer = shadow.GetComponent<SpriteRenderer> ();
		shadowRenderer.enabled = false;
		shadowRenderer.color = new Color (1f, 1f, 1f, .5f);
		shadowRenderer.sprite = GetComponent<SpriteRenderer> ().sprite;
		boardManager = GameObject.Find ("Board").GetComponent<BoardManager> ();
		pickable = true;
	}

	void Update(){
		if (transform.hasChanged) {
			GetComponent<SpriteRenderer> ().sortingOrder = Mathf.RoundToInt (transform.position.y * 100f) * -1;
			transform.hasChanged = false;
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		//if the player runs into us and we can be picked up
		if (other.tag == "Player" && pickable) {
			//tell them to pick us up
			other.gameObject.GetComponent<PlayerController> ().PickUp (this.gameObject);
		}
	}

	public bool ICanGoHere(Vector3 location){
		//If we are a plant
		if (transform.tag == "FightingPlant" || transform.tag == "Plant") {
			//We can go in the yard but not on the porch
			if (boardManager.IsInYard (location) && !boardManager.IsOnPorch (location))
				return true;
		} 
		//If we are a toe
		else if (transform.tag == "Toe") {
			//We can go in the nursery
			if (boardManager.IsInNursery (location) && !boardManager.IsOnPorch (location)) {
				return true;
			}
		}

		return false;
	}

	public void returnShadow(){
		shadow.GetComponent<SpriteRenderer>().enabled = false;
		shadow.transform.position = transform.position;
	}
}
