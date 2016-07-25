using UnityEngine;
using System.Collections;

public class GrassController : FloraController {

	public Sprite fullHealth;
	public float ruffledHealthBar;
	public Sprite ruffledHealth;
	public float trampledHealthBar;
	public Sprite trampledHealth;
	public Sprite dirt;

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void TakeDamage(float damage) {
		base.TakeDamage(damage);
		if (health <= 0) {
			spriteRenderer.sprite = dirt;
		} else if (health < trampledHealthBar) {
			spriteRenderer.sprite = trampledHealth;
		} else if (health < ruffledHealthBar) {
			spriteRenderer.sprite = ruffledHealth;
		}
	}
}
