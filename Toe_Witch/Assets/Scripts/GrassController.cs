﻿/* Grass Controller
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
	protected override void TakeDamage(float damage) {
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
}
