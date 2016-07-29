using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Slot{
	public string itemType = null;
	public List<GameObject> contents = new List<GameObject>();
	public Vector3 position;
	public GameObject counter;

	public void Add(GameObject item){
		contents.Add (item);
		itemType = item.tag;
		//tell the item it's in the inventory
		item.GetComponent<PickupController> ().inInventory = true;

		//if there's more than one item, set the counter
		if (contents.Count > 1) {
			counter.SetActive (true);
			counter.GetComponentInChildren<Text>().text = contents.Count.ToString();
		}
	}

	public void Remove(GameObject item){
		contents.Remove (item);
		//tell the item its no longer in the inventory
		item.GetComponent<PickupController> ().inInventory = false;

		//update the counter
		counter.GetComponentInChildren<Text>().text = contents.Count.ToString();

		//if we just removed our last item, set item type to null
		if (contents.Count == 0) {
			itemType = null;
		}

		//if we just removed our second to last item, turn off the counter
		if (contents.Count <= 1) {
			counter.SetActive (false);
		}
	}
}

public class InventoryController : MonoBehaviour {

	public int numSlots;
	public int maxSlotCapacity;
	public float slotSize;
	public float slotGapSize;
	public float speed;
	public GameObject canvas;
	public GameObject counterPrefab;

	private List<Slot> slots; //list of Slot objects which can be filled with items
	private Dictionary<GameObject, Vector3> movingItems; //items currently moving <item, destination> 
	private List<GameObject> deathRow = new List<GameObject>();

	void Start(){
		//movingItems = new Dictionary<GameObject, Vector3> ();

		//size inventory GameObject to the number of slots we have
		transform.localScale = new Vector3(numSlots * slotSize + (numSlots + 1) * slotGapSize, 
			transform.localScale.y + 2*slotGapSize, 0f);
		
		slots = new List<Slot> ();
		//fill "slots" with slot objects
		for (int i=0; i < numSlots; i++){
			Slot slot = new Slot();
			//put new slot in position (this is fake - it has no GameObject associated with it)
			//inventory position - half inventory width aligns with left edge, + half slot to center, + however many slots to the right
			float x = transform.position.x - transform.localScale.x / 2  + .5f * slotSize + i * slotSize + i * slotGapSize;
			slot.position = new Vector3(x, transform.position.y, 0f);

			//make a new counter object 
			GameObject counter = Instantiate(counterPrefab);
			//set as child of canvas
			counter.transform.SetParent(canvas.transform);
			//set position
			counter.transform.position = new Vector3(x + .5f, 7, 0f);
			//set inactive
			counter.SetActive(false);
			//give reference to slot
			slot.counter = counter;

			slots.Add(slot);
		}
	}

	void Update(){
		//for each item which is moving
		movingItems = new Dictionary<GameObject, Vector3> ();
		int a = movingItems.Count;
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
		foreach (Slot slot in slots) {
			//if there's a slot holding this kind of thing already and it's not full
			if (slot.itemType == item.tag && slot.contents.Count <= maxSlotCapacity){
				//add this thing to that slot
				slot.Add (item);
				//set the item to zoom into place in that slot
				movingItems.Add(item, slot.position);
				return;
			}
		}

		//otherwise, add the item to the first empty slot
		foreach (Slot slot in slots) {
			if (slot.itemType == null) {
				slot.Add (item);
				movingItems.Add(item, slot.position);
				return;
			}
		}
		//if there's no empty slot nothing happens
	}

	public void Remove(GameObject item){
		foreach (Slot slot in slots) {
			if (slot.itemType == item.tag) {
				slot.Remove (item);
				ReShuffle ();
				return;
			}
		}

		//reposition the remaining items on screen
		ReShuffle();
	}

	void ReShuffle(){
		//for each slot
		for (int i = 0; i < numSlots; i++) {
			//if the slot is empty
			if (slots [i].itemType == null) {
				//check all subsequent slots
				for (int j = i; j < numSlots; j++) {
					//if you find one with stuff in it
					if (slots [j].itemType != null) {
						//move all the stuff into the empty slot
						foreach (GameObject item in slots[j].contents) {
							slots [j].Remove (item);
							slots [i].Add (item);
							break;
						}
					}
				}
			}
		}
	}
}
