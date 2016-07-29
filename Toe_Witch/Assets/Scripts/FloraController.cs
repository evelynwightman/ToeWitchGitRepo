using UnityEngine;
using System.Collections;

public class FloraController : MonoBehaviour {

	public float startingHealth;
	public bool isSeed;
	public int lifeSpan;
	public int growUpTime;

	[Header("Sprites")]
	public Sprite seed;
	public Sprite sprout;
	public Sprite adultPlant;
	public bool planted = false;

	public float health;
	protected SpriteRenderer spriteRenderer;
	protected PickupController pickupController;
	protected BoardManager boardManager;
	protected float age = 0;

	protected virtual void Start () {
		health = startingHealth;
		pickupController = GetComponent<PickupController> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		boardManager = GameObject.Find ("Board").GetComponent<BoardManager> ();
		transform.rotation = Quaternion.Euler(0f, 0f, -35f); //start tapped
		if (transform.tag == "Grass") {
			Plant ();
		}
		if (isSeed) {
			spriteRenderer.sprite = seed;
		} else {
			spriteRenderer.sprite = adultPlant;
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


	public virtual void Plant(){
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

		//if we're a seed, sprout
		if (isSeed) {
			spriteRenderer.sprite = sprout;
		}
	}

	private Vector3 SnapToGrid(Vector3 currentPosition){
		return new Vector3(Mathf.Round(currentPosition.x), Mathf.Round(currentPosition.y), 0f);
	}

	public virtual void GrowUp(){
		spriteRenderer.sprite = adultPlant;
		isSeed = false;
	}

	public virtual void Age(){
		//unless we're planted
		if(!planted){
			return;
		}
		//increment age
		age++;

		//check whether it's time to grow up
		if (isSeed && age >= growUpTime) {
			GrowUp ();
		}

		//check whether it's time to die
		if (age >= lifeSpan) {
			Die ();
		}
	}


	protected void Die(){
		Destroy (this.gameObject);
	}
}
