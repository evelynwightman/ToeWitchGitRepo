using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingObject : MonoBehaviour {

	public BoardManager boardManager;
	[Header("Base Stats")]
	public float speed;
	public float runningSpeed;
	public float hitStrength;
	public float hitRecharge = 0.5f;

	[HideInInspector]
	public GameObject itemToPutDown = null;
	[HideInInspector]
	public bool haveLocationToPutDown = false;
	[HideInInspector]
	public Vector3 locationToPutDown;

	protected Rigidbody2D rb;
	protected Vector3 endPoint;
	protected bool facingRight = false;
	protected bool moving = true;
	protected List<GameObject> inRange = new List<GameObject>();
	protected GameObject hitTarget;
	protected float hitCountdown;
	protected Animator animator;
	protected AudioSource audioSource;
	protected GameObject trackTarget;
	protected SpriteRenderer spriteRenderer;
	protected GameObject canvas = null;

	private Vector3 pointer;

	protected virtual void Start()
	{
		rb = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		audioSource = GetComponent<AudioSource> ();
		endPoint = transform.position;
		hitCountdown = hitRecharge;
		spriteRenderer = GetComponent<SpriteRenderer> ();
		if (transform.FindChild ("PlantCanvas") != null) {
			canvas = transform.FindChild ("PlantCanvas").gameObject;
		}
	}

	protected virtual void Update(){
		//keep everything overlapping according to y position
		spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

		if (canvas != null){
			canvas.GetComponent<Canvas> ().sortingOrder = Mathf.RoundToInt (transform.position.y * 100f) * -1;
		}
	}

	/* MoveInYard
	 * Moves an object to the endPoint at constant velocity
	 * handles animation and puts a thing down at endPoint if we have something to put there
	 */
	protected void MoveInYard(Vector3 endPoint){
		moving = true;
		//handle visuals
		animator.SetBool ("walking", true);
		FaceSprite(endPoint);

		//move toward end point
		transform.position = Vector3.MoveTowards (transform.position, endPoint, speed * Time.deltaTime); 

		//make sure we stay in the yard
		transform.position = new Vector3 (
			Mathf.Clamp (transform.position.x, boardManager.yardWest + .5f, boardManager.yardEast - .5f),
			Mathf.Clamp (transform.position.y, boardManager.yardSouth + .5f, boardManager.yardNorth - .5f),
			0f
		);

		//if we've reached the end point, stop
		if (Mathf.Approximately (transform.position.magnitude, endPoint.magnitude)) {
			moving = false;
			animator.SetBool ("walking", false);
			//if we have a thing to put down, put it down here
			if (haveLocationToPutDown)
				PutDown ();
		}
	}

	/* PutDown
	 * Must be implemented by child class
	 */
	public virtual void PutDown(){
		Debug.Log ("PutDown has not been implemented!");
	}

	/* FaceSprite
	 * Faces sprite toward the target (along x only)
	 */
	protected void FaceSprite(Vector3 target){
		//if target is to the right and we're not already facing right
		if (target.x > transform.position.x && !facingRight) {
			//face right
			transform.Rotate(0, 180, 0);
			facingRight = true;
		}
		//if target is to the left and we're facing right
		else if (target.x < transform.position.x && facingRight) {
			//face left
			transform.Rotate(0, -180, 0);
			facingRight = false;
		}
	}

	public void OnDayEnd(){
		inRange.Clear ();
	}
		
}
