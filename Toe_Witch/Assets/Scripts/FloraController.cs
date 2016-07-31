/* BoardManager
 * Evelyn Wightman 2016
 * Base class controller for all plants, including grass. Handles being planted, taking damage, and aging.
 */

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

	public float health; //public for testing purposes only
	protected SpriteRenderer spriteRenderer;
	protected PickupController pickupController;
	protected BoardManager boardManager;
	protected float age = 0;

	protected virtual void Start () {
		health = startingHealth;
		//get components
		pickupController = GetComponent<PickupController> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		boardManager = GameObject.Find ("Board").GetComponent<BoardManager> ();
		//start off tapped
		transform.rotation = Quaternion.Euler(0f, 0f, -35f); 
		//grass should start planted (thus untapped)
		if (transform.tag == "Grass") {
			Plant ();
		}
		//set sprite if needed
		if (isSeed) {
			spriteRenderer.sprite = seed;
		} else {
			spriteRenderer.sprite = adultPlant;
		}
	}

	/* OnTriggerEnter2D
	 * Looks for tramplers stepping on us, calls taking damage
	 */
	protected virtual void OnTriggerEnter2D(Collider2D other){
		//if a trampler steps on you
		if (other.tag == "Trampler") {
			//take damage equal to the tramplage of that trampler
			TakeDamage(other.gameObject.GetComponent<TramplerController>().tramplage);
		}
	}

	/* TakeDamage
	 * What it says on the box
	 */
	protected virtual void TakeDamage(float damage){
		health = health - damage;
	}

	/* Plant
	 * Plants us. Lots of stuff only happens when we're planted.
	 */
	public virtual void Plant(){
		//set it so we can no longer be picked up
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

	/* SnapToGrid
	 * Centers us in a grid square
	 */
	private Vector3 SnapToGrid(Vector3 currentPosition){
		return new Vector3(Mathf.Round(currentPosition.x), Mathf.Round(currentPosition.y), 0f);
	}

	/* GrowUp
	 * Changes us from sprout to adult plant
	 */
	public virtual void GrowUp(){
		spriteRenderer.sprite = adultPlant;
		isSeed = false;

		//visual bounce for emphasis
		StartCoroutine(pickupController.Bounce());
	}

	/* Age
	 * Increases the age of the plant and checks if it's time to grow up or die
	 */
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

	/* Die
	 * Destroy this plant
	 */
	protected void Die(){
		Destroy (this.gameObject);
	}
}
