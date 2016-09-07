/* PlayerController
 * Evelyn Wightman 2016
 * Modified from PlayerController.cs from Unity 2D Roguelike Tutorial
 */

using UnityEngine;
using System.Collections;

public class PlayerController : MovingObject
{
	public InventoryController inventory;
	public AudioClip pickupSound;
	public AudioClip hitSound;

	protected override void Start()
	{
		base.Start ();
		facingRight = false;
	}

	protected override void Update()
	{
		//Deal damage to one trampler which is in range (Let player choose target eventually)
		if (inRange.Count != 0 && hitCountdown <= 0) {
			hit (hitTarget);
			hitCountdown = hitRecharge;
		}
		hitCountdown = hitCountdown - 1*Time.deltaTime;

		//check to see if we've pick up a thing we're tracking
		if (trackTarget != null) {
			if (trackTarget.GetComponent<PickupController> () != null) {
				if (trackTarget.GetComponent<PickupController> ().inInventory == true) {
					trackTarget = null;
				}
			}
		}

		//if we're still tracking something, set it as our destination
		if (trackTarget != null) {
			//if not, set us up to go toward it
			endPoint = trackTarget.transform.position;
		}

		//Move toward current destination
		MoveInYard (endPoint);
		base.Update ();
	}

	/* GoTo
	 * Sets a new destination
	 */
	public void GoTo(Vector3 pointToGoTo){
		trackTarget = null;
		endPoint = pointToGoTo;
	}

	/* Track
	 * Sets a GameObject for us to follow
	 */
	public void Track(GameObject objectToFollow){
		trackTarget = objectToFollow;
	}

	/* OnCollisionEnter2D
	 * Handles attacking nearby foes
	 */
	void OnCollisionEnter2D (Collision2D other){
		//if a trampler comes into range
		if (other.gameObject.tag == "Trampler") {
			//add it to the list of things to hit and make it a priority
			inRange.Add (other.gameObject);
			hitTarget = other.gameObject;
		}
	}

	/* OnCollisionExit2D
	 * Helps keep track of which foes are in range
	 */
	void OnCollisionExit2D(Collision2D other){
		//the thing is no longer in range
		inRange.Remove(other.gameObject);
		//if there's something else still in range, hit that instead
		if (inRange.Count != 0) {
			hitTarget = inRange [0]; 
		}
	}

	/* Hit
	 * Attacks a foe
	 */
	protected virtual void hit(GameObject hitTarget){
		if (hitTarget == null)
			return;
		//handle visuals
		animator.SetTrigger ("hit");
		FaceSprite (hitTarget.transform.position);
		//deal damage
		hitTarget.GetComponent<TramplerController> ().TakeDamage (hitStrength);
		//play audio
		audioSource.clip = hitSound;
		audioSource.volume = 1f;
		audioSource.Play ();
	}

	/* PickUp
	 * Adds given item to inventory 
	 */
	public void PickUp(GameObject item){
		audioSource.clip = pickupSound;
		audioSource.volume = .25f;
		audioSource.Play ();
		inventory.Add (item);
	}

	/* PutDown
	 * Removes current itemToPutDown from inventory (if we have one) and plants it if its a plant
	 * (Inherets from MovingObject because we need to call it from there)
	 */
	public override void PutDown(){
		if (itemToPutDown != null) {
			//handle visuals and move itemToPutDown to current position
			itemToPutDown.transform.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
			itemToPutDown.transform.GetComponent<PickupController> ().returnShadow ();
			itemToPutDown.transform.position = transform.position;
			//remove from inventory
			inventory.Remove (itemToPutDown);
			//if it's a plant, plant it here
			if (itemToPutDown.tag == "Plant" || itemToPutDown.tag == "FightingPlant" || 
				itemToPutDown.tag == "Grass") {
				itemToPutDown.GetComponent<FloraController>().Plant ();
			}
			//if it's a toe, see if there's a plant to combine it with
			if (itemToPutDown.tag == "Toe") {
				itemToPutDown.GetComponentInChildren<ObjectCombiner>().Combine ();
			}
			//now we have nothing to put down: update the vars
			haveLocationToPutDown = false;
			itemToPutDown = null;
		}
	}
		
}
