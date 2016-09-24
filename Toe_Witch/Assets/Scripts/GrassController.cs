/* Grass Controller
 * Child class of FloraController. Controls grass.
 * 
 * Copyright (c) 2016 by Evelyn Wightman. All rights reserved. 
 * Subject to the terms and conditions contained in LICENSE file.
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
	[HideInInspector]
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

	/* Plant
	 * Adds yard management stuff to Plant. 
	 */
	public override void Plant(){
		base.Plant ();

		isSeed = false;

		//make us a child of the yard
		GameObject yard = GameObject.Find("Yard");
		transform.parent = yard.transform;

		//if there's any grass underneath us, destroy it.
		foreach (Transform grass in yard.transform) {
			if (grass != this.transform) {
				if (transform.position == grass.position) {
					Destroy (grass.gameObject);
				}
			}
		}

		//show adult grass sprite
		spriteRenderer.sprite = adultPlant;
		spriteRenderer.material = adultPlantMaterial;
	}
}
