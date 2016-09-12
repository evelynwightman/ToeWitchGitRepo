/* TramplerController
 * Evelyn Wightman 2016
 * What it says on the tin.
 */
using UnityEngine;
using System.Collections;

public class TramplerController : MovingObject {

	[Header("Dropping Stuff")]
	public GameObject toe; //the toe we drop when we run away
	public float chanceToDrop; //the probability that we will drop a toe (should be between 0 and 1)
	[Header("Stats")]
	public float startingHealth;
	public float tramplage; //how much damage our tramping does
	public float herpTime; //how long to spend herping
	public float herpWait; //time to wait between looking in a different direction
	public float nurseryWaitTime; //how long we'll stay out of the nursery
	public float wreckTime; //how long to spend wrecking
	public float wreckWait; //how long to wait between damages while wrecking
	//[HideInInspector]
	public bool leaving = false; //are we leaving the yard?
	public bool isChild;

	private float herpCount; //tracks where we are in herping sequence (0 means not herping)
	private float lifetimeCount; //tracks how long we've been alive
	private bool entering = true; //are we heading into the yard?
	private Vector3 herpSpot;
	public float health; //public for DEBUGGING
	private Vector3 exitPoint;
	private Vector3 entryPoint;
	private BoxCollider2D boxCollider;
	private bool wrecking = false; //are we currently wrecking a square?

	protected override void Start () {
		boardManager = GameObject.Find ("Board").GetComponent<BoardManager>();
		boxCollider = transform.GetComponent<BoxCollider2D> ();
		base.Start ();
		herpCount = 0;
		lifetimeCount = 0;
		health = startingHealth;
		exitPoint = new Vector3 (boardManager.columns + 1, boardManager.rows + 2); //make this spawn point
		entryPoint = new Vector3 ((float)boardManager.columns - 1, Random.Range(0, boardManager.rows -1));
		entering = true;
	}

	/* Restart 
	 * Resets defaults so we're ready to go again
	 */
	public void Restart (){
		leaving = false;
		boxCollider.size = boxCollider.size*4;
		Start ();
	}

	protected override void Update () {
		lifetimeCount = lifetimeCount + 1*Time.deltaTime;

		//if we're not actively wrecking
		if (!wrecking) {
			//check flags, do things
			if (leaving) {
				LeaveYard ();
			} else if (entering) {
				EnterYard ();
			} else if (moving) {
				MoveInYard (endPoint);
			}
			//if we're at our endPoint and not already wrecking, wreck this spot
			if (!moving && !wrecking && herpCount == 0) {
				StartCoroutine(WreckThisSpot ());
			}
			/*
			//if we're not moving or herping
			else if (!moving && herpCount == 0) {
				//herp derp and go to a new random spot
				StartCoroutine (HerpDerp ());
			}
			*/
		}
		base.Update ();
	}

	/* OnCollisionEnter2D
	 * If we run into a boundary, find somewhere else to go.
	 */
	void OnCollisionEnter2D(Collision2D other){
		//porch wall
		if (other.gameObject.tag == "Boundary") {
			//stop moving
			moving = false;
			//herp derp and go to a new random spot
			StartCoroutine(HerpDerp());
		}
	}

	/* TakeDamage
	 * Takes damage and tells us to run away if we're too hurt
	 */
	public void TakeDamage(float damage){
		//stop wrecking
		StopCoroutine("WreckThisSpot");
		wrecking = false;
		animator.SetBool ("wrecking", false);
		//take damage
		health = health - damage;
		//handle visuals
		animator.SetTrigger ("damage");
		//if we're at 0 health and we're not already leaving
		if (health <= 0 && !leaving) {
			//run away
			leaving = true;
			boxCollider.size = boxCollider.size/4;
			//maybe drop a toe
			if (Random.Range (0, 1) < chanceToDrop) {
				DropToe ();
			}
		}
	}

	/* DropToe
	 * Instantiates a new toe at current position
	 */
	void DropToe(){
		Quaternion spawnRotation = Quaternion.identity;
		Instantiate (toe, transform.position, spawnRotation);
	}

	/* HerpDerp
	 * Gets a new spot to move to. Simulates indecision. Flips Trampler back and forth for herpTime.
	 */
	IEnumerator HerpDerp(){
		animator.SetBool ("walking", false);
		while (herpCount < herpTime) {
			herpSpot = getRandomSpotInYard ();
			FaceSprite (herpSpot);
			herpCount++;
			yield return new WaitForSeconds (herpWait);
		}
		herpCount = 0;
		endPoint = herpSpot;
		moving = true;
		animator.SetBool ("walking", true);
	}

	IEnumerator WreckThisSpot(){
		//animator bool set
		wrecking = true;
		animator.SetBool ("wrecking", true);
		int wreckCount = 0;
		GameObject plant;
		while (wreckCount < wreckTime) {
			//damage all plants we're touching
			Collider2D[] plants = Physics2D.OverlapCircleAll(transform.position, GetComponent<BoxCollider2D>().size.x/2);
			foreach (Collider2D clickable in plants) {
				plant = clickable.transform.parent.gameObject;
				if (plant.tag == "Grass") {
					plant.GetComponent<GrassController> ().TakeDamage (tramplage);
				} else if (plant.tag == "FightingPlant") {
					plant.GetComponent<MeleePlantController> ().TakeDamage (tramplage);
				}
			}

			wreckCount ++;
			yield return new WaitForSeconds (wreckWait);
		}
		wrecking = false;
		animator.SetBool ("wrecking", false);

		StartCoroutine (HerpDerp ());
		//animator bool set

	}

	/* getRandomSpotInYard
	 * Returns a random position within the bounds of the yard
	 * Handles nursery wait time and keeps trampler off porch
	 */
	Vector3 getRandomSpotInYard(){
		//get a random spot in the yard (int spot if child)
		Vector3 random;
		if (!isChild){
			random = new Vector3 (Random.Range (boardManager.yardWest + .5f, boardManager.yardEast - .5f), 
				Random.Range (boardManager.yardSouth + .5f, boardManager.yardNorth - .5f), 0.0f);
		}
		else {
			random = new Vector3 (Random.Range((int)(boardManager.yardWest + .5f), (int)(boardManager.yardEast + .5f)), 
				Random.Range ((int)(boardManager.yardSouth + .5f), (int)(boardManager.yardNorth + .5f)), 0);
		}
		
		//if you haven't been around longer than your nurseryWaitTime, don't go into the nursery
		if (nurseryWaitTime > lifetimeCount * Time.deltaTime && boardManager.IsInNursery(random)) {
			return getRandomSpotInYard ();
		} 
		//don't ever go onto the porch
		if (boardManager.IsOnPorch(random)) {
			return getRandomSpotInYard ();
		}

		return random;
	}

	/* LeaveYard
	 * Handles running back to the spawn point
	 */
	void LeaveYard(){
		moving = true;
		speed = runningSpeed;
		//if we're still in the yard, leave
		if (transform.position.x < boardManager.yardEast) {
			endPoint = new Vector3 (boardManager.columns, transform.position.y, 0f);
		} //once we're out, head back to the spawn point which will reset us
		else {
			endPoint = exitPoint;
		}
		//handle visuals
		animator.SetBool ("running", true);
		FaceSprite (endPoint);

		//move towards endPoint
		transform.position = Vector3.MoveTowards (transform.position, endPoint, speed * Time.deltaTime); 
	}

	/* EnterYard
	 * Handles walking into yard
	 */
	void EnterYard(){
		moving = true;
		animator.SetBool ("walking", true);
		//if we're not at entry point
		if (!Mathf.Approximately (transform.position.magnitude, entryPoint.magnitude)) {
			//move toward it
			FaceSprite (entryPoint);
			transform.position = Vector3.MoveTowards (transform.position, entryPoint, speed * Time.deltaTime);
		} else {
			//we have arrive: set the variables
			entering = false;
			moving = false;
		}
	}
}
