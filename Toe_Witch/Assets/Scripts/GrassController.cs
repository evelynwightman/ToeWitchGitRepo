/* Grass Controller
 * Evelyn Wightman 2016
 * Child class of FloraController. Controls grass.
 */
using UnityEngine;
using System.Collections;

public class GrassController : FloraController {

	public Sprite fullHealth;
	public float ruffledHealthBar;
	public Sprite ruffledHealth;
	public float trampledHealthBar;
	public Sprite trampledHealth;
	public Sprite dirt;
	//[HideInInspector]
	public bool isDead = false;

	protected override void Start () {
		base.Start();
	}

	/* TakeDamage
	 * Handles taking damage and updating the sprite accordingly. Tells Game
	 */
	public override void TakeDamage(float damage) {
		base.TakeDamage(damage);
		if (health <= 0) {
			spriteRenderer.sprite = dirt;
			isDead = true;
			boardManager.CheckLawn ();
		} else if (health < trampledHealthBar) {
			spriteRenderer.sprite = trampledHealth;
		} else if (health < ruffledHealthBar) {
			spriteRenderer.sprite = ruffledHealth;
		} 
	}

	public override void Plant(){
		base.Plant ();

		//make us a child of the yard
		GameObject yard = GameObject.Find("Yard");
		transform.parent = yard.transform;

		//if there's any grass underneath us, destroy it.
		foreach (Transform grass in yard.transform) {
			if (grass != this.transform) {
				if (transform.position == grass.position) {
					Debug.Log (grass);
					Destroy (grass.gameObject);
				}
			}
		}

		//show adult grass sprite
		spriteRenderer.sprite = adultPlant;
		spriteRenderer.material = adultPlantMaterial;
	}
}
