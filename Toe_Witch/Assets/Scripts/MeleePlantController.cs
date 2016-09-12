/* MeleePlantController
 * Evelyn Wightman 2016
 * Child class of FloraController. Controlls fighting plants. Adds hitting, health bar, and limited lifespan.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MeleePlantController : FloraController {

	public float hitStrength;
	public float hitRecharge;

	private List<GameObject> inRange = new List<GameObject>();
	private GameObject hitTarget;
	private float hitCountdown = 0;
	private Animator animator;
	private GameObject healthBar;
	private GameObject daysRemainingBar;

	protected override void Start () {
		base.Start();
		//Find components
		animator = GetComponent<Animator> ();
		healthBar = transform.FindChild ("PlantCanvas").FindChild ("HealthBackground").FindChild ("Health").gameObject;
		daysRemainingBar = transform.FindChild ("PlantCanvas").FindChild ("DaysRemaining").gameObject;
		audioSource = GetComponent<AudioSource> ();

		//Align daysRemainingBar
		Vector3 position = daysRemainingBar.transform.GetComponent<RectTransform>().localPosition;
		daysRemainingBar.transform.GetComponent<RectTransform>().localPosition = new Vector3(
			position.x + .5f - (adultLifeSpan/2f)*.1f, position.y,	position.z);
		//Fill daysRemainingBar
		daysRemainingBar.transform.GetComponent<Image>().fillAmount = adultLifeSpan*.1f;

		//hide status bars until planted
		if (!planted) {
			healthBar.SetActive (false);
			healthBar.transform.parent.gameObject.SetActive (false);
			daysRemainingBar.SetActive (false);
		}
	}

	void Update () {
		//Deal damage to one trampler which is in range
		if (hitTarget != null && hitCountdown <= 0) {
			hit (hitTarget);
			hitCountdown = hitRecharge;
		}

		hitCountdown = hitCountdown - 1*Time.deltaTime;
	}

	/* OnTriggerEnter2D
	 * Calls base fcn (handles being damaged by trampler). Adds incoming trampler to list of things we can hit.
	 */
	protected override void OnTriggerEnter2D(Collider2D other){
		//unless we're a seed
		if (isSeed) {
			return;
		}
		//if we're planted
		if (planted) {
			//handle being damaged
			base.OnTriggerEnter2D (other);

			//if it's a trampler 
			if (other.tag == "Trampler") {
				//add it to the list of things to hit and make it a priority
				inRange.Add (other.gameObject);
				hitTarget = other.gameObject;
			}
		}
	}

	/* OnTriggerExit2D
	 * Removes exiting tramplers from list of things we can hit and assigns a new hit target (if there is one)
	 */
	void OnTriggerExit2D(Collider2D other){
		//unless we're a seed
		if (isSeed) {
			return;
		}
		//if it's a trampler
		if (other.tag == "Trampler") {
			//it's no longer in range
			inRange.Remove (other.gameObject);
			//if there's something else still in range, hit that instead
			if (inRange.Count != 0) {
				hitTarget = inRange [0];
			} else
				hitTarget = null;
		}
	}

	/* hit
	 * Hit the given target. Handles visuals and dealing damage.
	 */
	void hit(GameObject target){
		//handle visuals
		animator.SetTrigger ("hit");
		//handle sound
		if (audioSource == null)
			Debug.Log ("Yes, here!");
		audioSource.volume = 1f;
		audioSource.clip = hittingSound;
		audioSource.Play();
		//deal damage
		if (hitTarget != null)
			hitTarget.GetComponent<TramplerController> ().TakeDamage (hitStrength);
	}

	/* TakeDamage
	 * Calls base damage(). Updates health bar and checks if we're dead.
	 */
	public override void TakeDamage(float damage){
		base.TakeDamage (damage);

		//update health bar
		Image healthImage = healthBar.GetComponent<Image> ();
		healthImage.fillAmount = (float)health / (float)startingHealth;

		//color health bar
		if (healthImage.fillAmount <= .3f) {
			healthImage.color = Color.red;
		} else if (healthImage.fillAmount <= .5f) {
			healthImage.color = Color.yellow;
		} else {
			healthImage.color = Color.green;
		}

		//check if dead
		if (health <= 0) {
			Die ();
		}
	}

	/* Age
	 * Calls base age and adjusts daysRemainingBar.
	 */
	public override void Age ()
	{
		//don't age unless we're planted
		if(!planted){
			return;
		}
		//call base Age()
		base.Age ();
		//decrement daysRemainingBar
		daysRemainingBar.GetComponent<Image> ().fillAmount = daysRemainingBar.GetComponent<Image> ().fillAmount - .1f;
		//check whether it's time to die
		if (age >= adultLifeSpan) {
			Die ();
		}
	}

	/* Plant
	 * Calls base Plant() and activates status bars
	 */
	public override void Plant(){
		base.Plant ();
		//activate status bars
		healthBar.SetActive (true);
		healthBar.transform.parent.gameObject.SetActive (true);
		daysRemainingBar.SetActive (true);
	}

	/* OnDayEnd
	 * Clears out list of things we can hit (they may have been vanished)
	 */
	public void OnDayEnd(){
		inRange.Clear ();
	}
} 