using UnityEngine;
using System.Collections;

public class PotController : MonoBehaviour {

	public float startingHealth;

	private float health;
	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update(){
		//keep everything overlapping according to y position
		spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

	}

	void OnTriggerEnter2D(Collider2D other){
		//if a trampler steps on you
		if (other.tag == "Trampler") {
			//take damage equal to the tramplage of that trampler
			TakeDamage(other.gameObject.GetComponent<TramplerController>().tramplage);
		}
	}

	protected virtual void TakeDamage(float damage){
		health = health - damage;

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
}
