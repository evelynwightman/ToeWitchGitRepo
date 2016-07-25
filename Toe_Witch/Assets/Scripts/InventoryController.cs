using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour {

	public int slots;
	public float slotSize;
	public float slotGapSize;
	public float speed;

	private List<GameObject> items;
	private Dictionary<GameObject, Vector3> movingItems;
	private List<GameObject> deathRow = new List<GameObject>();

	private void Start(){
		items = new List<GameObject>();
		transform.localScale = new Vector3(slots * slotSize + (slots + 1) * slotGapSize, 
			transform.localScale.y + 2*slotGapSize, 0f);
		movingItems = new Dictionary<GameObject, Vector3>();
	}

	void Update(){
		//for each item which is moving
		foreach (KeyValuePair<GameObject,Vector3> entry in movingItems) {
			//if the item has reached its destination, remove it from movingItems
			if (Mathf.Approximately (entry.Key.transform.position.magnitude, entry.Value.magnitude)) {
				deathRow.Add(entry.Key);
			}

			//move the item toward its destination
			entry.Key.transform.position = Vector3.MoveTowards (entry.Key.transform.position, entry.Value, speed*Time.deltaTime);
		}

		foreach (GameObject item in deathRow) {
			movingItems.Remove (item);
		}
	}

	/* Add
	 * Adds given item to inventory
	 * Returns true if item was successfully added to inventory, false otherwise
	 */
	public void Add(GameObject item){
		//if there's room in inventory
		if (items.Count < slots) {
			//add the item
			PutInSlot (item, items.Count + 1);
			items.Add (item);
			item.GetComponent<PickupController> ().inInventory = true;
		}
	}

	public void Remove(GameObject item){
		//remove the item from the list
		items.Remove (item);
		item.GetComponent<PickupController> ().inInventory = false;
		//reposition the remaining items on screen
		ReShuffle();
	}

	void ReShuffle(){
	int slot = 0;
		foreach (GameObject i in items) {
			PutInSlot (i, slot);
			slot++;
		}
	}

	/* PutInSlot
	 * Moves given game object to position of given slot in inventory
	 */
	void PutInSlot(GameObject item, int slot){
		//inventory position - half inventory width + half item width -> aligns item left edge with inventory left edge
		float offset = transform.position.x - transform.localScale.x / 2 + item.transform.localScale.x / 2;
		//from edge move in however many slots and gaps
		float x = offset + (slot-1) * slotSize + slot * slotGapSize;
		//set the item to zoom into place
		movingItems.Add(item, new Vector3(x, transform.position.y, 0f));
	}

}
