using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleePlantController : FloraController {

	public float hitStrength;
	public float hitRecharge;

	private List<GameObject> inRange = new List<GameObject>();
	private GameObject hitTarget;
	private float hitCountdown = 0;
	private Animator animator;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		animator = GetComponent<Animator> ();
		//start tipped over (not planted)

	}
	
	// Update is called once per frame
	void Update () {
		//Deal damage to one trampler which is in range (Let player choose target eventually)
		if (inRange.Count != 0 && hitCountdown <= 0) {
			hit (hitTarget);
			hitCountdown = hitRecharge;
		}
		hitCountdown = hitCountdown - 1*Time.deltaTime;


	}

	protected override void OnTriggerEnter2D(Collider2D other){
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

	void OnTriggerExit2D(Collider2D other){
		//if it's a trampler
		if (other.tag == "Trampler") {
			//it's no longer in range
			inRange.Remove (other.gameObject);
			//if there's something else still in range, hit that instead
			if (inRange.Count != 0) {
				hitTarget = inRange [0]; 
			}
		}
	}

	void hit(GameObject target){
		//handle visuals
		animator.SetTrigger ("hit");
		//deal damage
		hitTarget.GetComponent<TramplerController> ().TakeDamage (hitStrength);
	}
}