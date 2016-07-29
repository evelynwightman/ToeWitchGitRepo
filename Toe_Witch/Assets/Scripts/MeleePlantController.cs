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
	private AudioSource audioSource;

	// Use this for initialization
	protected override void Start () {
		base.Start();
		//Find components
		animator = GetComponent<Animator> ();
		audioSource = GetComponent<AudioSource> ();
		healthBar = transform.FindChild ("PlantCanvas").FindChild ("HealthBackground").FindChild ("Health").gameObject;
		daysRemainingBar = transform.FindChild ("PlantCanvas").FindChild ("DaysRemaining").gameObject;

		//Align daysRemainingBar
		Vector3 position = daysRemainingBar.transform.GetComponent<RectTransform>().localPosition;
		daysRemainingBar.transform.GetComponent<RectTransform>().localPosition = new Vector3(
			position.x + .5f - (lifeSpan/2f)*.1f, position.y,	position.z);
		//Fill daysRemainingBar
		daysRemainingBar.transform.GetComponent<Image>().fillAmount = lifeSpan*.1f;

		//hide status bars until planted
		if (!planted) {
			healthBar.SetActive (false);
			healthBar.transform.parent.gameObject.SetActive (false);
			daysRemainingBar.SetActive (false);
		}
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
			}
		}
	}

	void hit(GameObject target){
		//handle visuals
		animator.SetTrigger ("hit");
		//handle sound
		audioSource.Play();
		//deal damage
		hitTarget.GetComponent<TramplerController> ().TakeDamage (hitStrength);
	}

	protected override void TakeDamage(float damage){
		base.TakeDamage (damage);

		//update health bar
		Image healthImage = healthBar.GetComponent<Image>();
		healthImage.fillAmount = (float)health / (float)startingHealth;
		//color health bar
		Debug.Log("healthBar.fillAmount = " + healthImage.fillAmount);
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

	public override void Age ()
	{
		//unless we're planted
		if(!planted){
			return;
		}
		//call base Age()
		base.Age ();
		//decrement daysRemainingBar
		daysRemainingBar.GetComponent<Image> ().fillAmount = daysRemainingBar.GetComponent<Image> ().fillAmount - .1f;
	}

	public override void Plant(){
		base.Plant ();
		//activate status bars
		healthBar.SetActive (true);
		healthBar.transform.parent.gameObject.SetActive (true);
		daysRemainingBar.SetActive (true);
	}
} 