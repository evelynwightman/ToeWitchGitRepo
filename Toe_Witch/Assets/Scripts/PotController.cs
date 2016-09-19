/* PotController
 * Evelyn Wightman 2016
 * Attached to pots. Handles damage and putting them in the correct sorting layer.
 */

using UnityEngine;
using System.Collections;

public class PotController : StaticObjectController {

	public float startingHealth;

	private GameObject plant;
	private float health;


	protected override void Start () {
		base.Start ();
	}

	/* OnTriggerEnter2D
	 * Handles being damaged by trampler
	 */
	void OnTriggerEnter2D(Collider2D other){
		//if a trampler steps on you
		if (other.tag == "Trampler") {
			//take damage equal to the tramplage of that trampler
			TakeDamage(other.gameObject.GetComponent<TramplerController>().tramplage);
		}
	}

	/* TakeDamage
	 * Handles taking damage and updating sprite accordingly.
	 */
	protected virtual void TakeDamage(float damage){
		health = health - damage;

		// updating sprites off for now because I don't have damage sprites for the pots.
		/*
		if (health <= 0) {
			spriteRenderer.sprite = dirt;
		} else if (health < trampledHealthBar) {
			spriteRenderer.sprite = trampledHealth;
		} else if (health < ruffledHealthBar) {
			spriteRenderer.sprite = ruffledHealth;
		}
		*/
	}

	public void setPlant(GameObject newPlant)
	{
		plant = newPlant;
	}

	public bool hasPlant(){
		if (plant == null)
			return false;
		else
			return true;
	}
		
}
