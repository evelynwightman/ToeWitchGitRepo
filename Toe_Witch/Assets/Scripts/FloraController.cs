/* BoardManager
 * Evelyn Wightman 2016
 * Base class controller for all plants, including grass. Handles being planted, taking damage, and aging.
 */

using UnityEngine;
using System.Collections;

public class FloraController : MonoBehaviour {

	public float startingHealth;
	public bool isSeed;
	public int adultLifeSpan;
	public int growUpTime;
	[HideInInspector]

	[Header("Sprites")]
	public Sprite seed;
	public Material seedMaterial;
	public Sprite sprout;
	public Material sproutMaterial;
	public Sprite adultPlant;
	public Material adultPlantMaterial;
	public bool planted = false;

	[Header("Sounds")]
	public AudioClip growUpSound;
	public AudioClip plantingSound;
	public AudioClip hittingSound;

	public float health; //public for testing purposes only
	protected SpriteRenderer spriteRenderer;
	protected PickupController pickupController;
	protected BoardManager boardManager;
	protected float age = 0;
	protected AudioSource audioSource;
	 
	protected virtual void Start () {
		health = startingHealth;
		AudioSource audioSource = transform.GetComponent<AudioSource> ();

		//get components
		pickupController = GetComponent<PickupController> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		boardManager = GameObject.Find ("Board").GetComponent<BoardManager> ();
		//start off tapped
		transform.rotation = Quaternion.Euler(0f, 0f, -35f); 
		//if this is part of the yard it should start planted (thus untapped)
		if (transform.parent != null){
			if(transform.parent.name == "Yard") {
				Plant ();
			}
		}
		//set sprite if needed
		if (isSeed) {
			spriteRenderer.sprite = seed;
			spriteRenderer.material = seedMaterial;
		} else {
			spriteRenderer.sprite = adultPlant;
			spriteRenderer.material = adultPlantMaterial;
		}
	}

	/* OnTriggerEnter2D
	 * Looks for tramplers stepping on us, calls taking damage
	 */
	protected virtual void OnTriggerEnter2D(Collider2D other){
		//if a trampler steps on you
		if (other.tag == "Trampler" && planted) {
			//take damage equal to the tramplage of that trampler
			TakeDamage(other.gameObject.GetComponent<TramplerController>().tramplage);
		}
	}

	/* TakeDamage
	 * What it says on the box
	 */
	public virtual void TakeDamage(float damage){
		health = health - damage;
	}

	/* Plant
	 * Plants us. Lots of stuff only happens when we're planted.
	 */
	public virtual void Plant(){
		//set it so we can no longer be picked up
		if (pickupController != null) {
			pickupController.pickable = false;
			Debug.Log ("Not pickable");
		}

		//play sound
		if (audioSource != null) {
			audioSource.volume = 1f;
			audioSource.clip = plantingSound;
			audioSource.Play ();
		}

		planted = true;

		//snap to grid if we're in the nursery or we're grass
		if ((boardManager.IsInNursery (transform.position) && 
			!boardManager.IsOnPorch (transform.position)) || tag == "Grass") {
			transform.position = SnapToGrid (transform.position);
		}

		//give the pickup controller the pot we're in so it can align us properly on the sorting layers
		GameObject pot = FindPot(transform.position);
		if (pot != null) {
			if (GetComponent<PickupController>() != null) {
				GetComponent<PickupController> ().pot = pot;
			}
		}

		//untap
		transform.rotation = Quaternion.Euler (0f, 0f, 0f);

		//if we're a seed, sprout and align with the top of the pot
		if (isSeed) {
			spriteRenderer.sprite = sprout;
			spriteRenderer.material = sproutMaterial;
			transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
		}
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
		AudioSource audio = GetComponent<AudioSource> ();
		if (audio != null) {
			audio.volume = 1;
			audio.clip = growUpSound;
			audio.Play ();
		}
		spriteRenderer.sprite = adultPlant;
		spriteRenderer.material = adultPlantMaterial;
		isSeed = false;

		//align with top of pot
		transform.position = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z);

		//visual bounce for emphasis
		StartCoroutine(pickupController.Bounce());

		//reset age to 0 to measure adultLifeSpan
		age = 0;
	}

	/* Age
	 * Increases the age of the plant and checks if it's time to grow up
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
	}

	/* Die
	 * Destroy this plant
	 */
	protected void Die(){
		Destroy (this.gameObject);
	}
}
