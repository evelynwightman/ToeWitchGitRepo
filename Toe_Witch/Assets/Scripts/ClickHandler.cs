using UnityEngine;
using System.Collections;

public class ClickHandler : MonoBehaviour {

	public PlayerController player;

	private bool isMouseDrag = false;
	private Vector3 screenPosition;
	private Vector3 offset;
	private GameObject mouseTarget;
	private GameObject shadow;

	void Start(){
		player = GameObject.Find ("Player").GetComponent<PlayerController>();
	}

	void OnLevelWasLoaded(int index){
		player = GameObject.Find ("Player").GetComponent<PlayerController>();
	}

	void Update(){
		//handle mouse input
		if (Input.GetMouseButtonDown(0))
		{
			//cancel whatever item movement we had going on
			if (player.itemToPutDown != null) {
				player.itemToPutDown.GetComponent<PickupController>().returnShadow();
				player.itemToPutDown.transform.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
				player.itemToPutDown = null;
				player.haveLocationToPutDown = false;
			}

			GetMouseTarget();

			//if we clicked on a clickable thing
			if (mouseTarget != null) {
				//if this is an item to pick up from the inventory
				if (mouseTarget.GetComponent<PickupController>() != null && mouseTarget.GetComponent<PickupController>().inInventory == true) {
					isMouseDrag = true;
					//flag this to be put down once player gets to where they're putting it
					player.itemToPutDown = mouseTarget;
					//set item transparent
					mouseTarget.transform.GetComponent<SpriteRenderer> ().color = new Color(1f,1f,1f,.5f);
					//enable the item's transparent shadow
					shadow = mouseTarget.transform.FindChild ("Shadow").gameObject;
					shadow.GetComponent<SpriteRenderer> ().enabled = true;

					screenPosition = Camera.main.WorldToScreenPoint (mouseTarget.transform.position);
					offset = mouseTarget.transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
				} 
				//if this is not an item from the inventory
				else {
					//tell player to track that object
					player.Track(mouseTarget);
				}
			} 
			//if we clicked on empty space
			else {
				//tell player to go to mouse click
				Vector3 endPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				endPoint = new Vector3(endPoint.x, endPoint.y, 0); 
				player.GoTo (endPoint);
			}
		}

		//if we released the mouse button
		if (Input.GetMouseButtonUp(0))
		{
			isMouseDrag = false;
			//find mouse position in world space with offset changes
			Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
			Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offset;
			//If the player has a thing to put down
			if (player.itemToPutDown != null){ 
				//if the thing can go here
				if (player.itemToPutDown.GetComponent<PickupController> ().ICanGoHere (currentPosition)) {
					//tell the player to put it here
					player.haveLocationToPutDown = true;
					player.locationToPutDown = currentPosition;
					player.GoTo (currentPosition);
				} 
				//if the thing cannot go here
				else {
					//put that thing back where it came from or so help me
					player.itemToPutDown.GetComponent<PickupController>().returnShadow();
					player.itemToPutDown.transform.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 1f);
				}
			}
		}

		//if we've clicked and are dragging
		if (isMouseDrag)
		{
			//find mouse position in world space with offset changes
			Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
			Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offset;
			//Update target gameobject's current postion.
			shadow.transform.position = currentPosition;
			//If the object could go here
			if (player.itemToPutDown.GetComponent<PickupController> ().ICanGoHere (currentPosition)) {
				//make the shadow normal colored
				shadow.transform.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, .5f);
			} else {
				//make the shadow red
				shadow.transform.GetComponent<SpriteRenderer> ().color = new Color (1f, 0f, 0f, .5f);
			}
		}
	}

	/* GetMouseTarget
	 * returns the collider that was clicked on by the mouse, null if no such collider
	 * all items except Clickable children of objects that can be dragged around should be set to ignore raycast
	 */
	void GetMouseTarget()
	{
		RaycastHit2D hit;
		mouseTarget = null;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		hit = Physics2D.Raycast (ray.origin, ray.direction);
		if (hit.collider != null)
		{
			//set mouse target to the parent of the clickable
			mouseTarget = hit.collider.gameObject.transform.parent.gameObject;
		}
	}
}
