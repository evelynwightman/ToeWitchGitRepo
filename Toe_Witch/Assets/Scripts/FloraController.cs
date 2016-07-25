using UnityEngine;
using System.Collections;

public class FloraController : MonoBehaviour {

	public float startingHealth;


	protected float health;
	protected SpriteRenderer spriteRenderer;
	protected PickupController pickupController;
	protected BoardManager boardManager;
	public bool planted = false;

	protected virtual void Start () {
		health = startingHealth;
		pickupController = GetComponent<PickupController> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		boardManager = GameObject.Find ("Board").GetComponent<BoardManager> ();
		transform.rotation = Quaternion.Euler(0f, 0f, -35f); //start tapped
		if (transform.tag == "Grass") {
			Plant ();
		}
	}

	protected virtual void OnTriggerEnter2D(Collider2D other){
		//if a trampler steps on you
		if (other.tag == "Trampler") {
			//take damage equal to the tramplage of that trampler
			TakeDamage(other.gameObject.GetComponent<TramplerController>().tramplage);
		}
	}
	
	protected virtual void TakeDamage(float damage){
		health = health - damage;
	}


	public void Plant(){
		if (pickupController != null) {
			pickupController.pickable = false;
		}
		planted = true;

		//snap to grid if we're in the nursery
		if (boardManager.IsInNursery (transform.position) && !boardManager.IsOnPorch (transform.position)) {
			transform.position = SnapToGrid (transform.position);
		}

		//untap
		transform.rotation = Quaternion.Euler (0f, 0f, 0f);
	}

	private Vector3 SnapToGrid(Vector3 currentPosition){
		return new Vector3(Mathf.Round(currentPosition.x), Mathf.Round(currentPosition.y), 0f);
	}

}
