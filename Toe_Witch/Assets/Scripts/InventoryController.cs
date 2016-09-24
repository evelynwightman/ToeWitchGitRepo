/* InventoryController
 * Contains TWO CLASSES, an INVENTORY which manages adding and removing items to/from inventory and organizing 
 * them within it, and a SLOT which manages the contents of a single slot in inventory.
 * 
 * Copyright (c) 2016 by Evelyn Wightman. All rights reserved. 
 * Subject to the terms and conditions contained in LICENSE file.
 */

/* Slot Class
 * Manages a single slot in inventory. Handles adding and removing items to/from slot. Can hold multiple items with the
 * same tag.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Slot{
	public string itemType = null; //tag of items currently held. Null when empty
	public List<GameObject> contents = new List<GameObject>();
	public Vector3 position;
	public GameObject counter; //diblet to display how many things we have


	/* Add
	 * Adds given item to this slot
	 */
	public void Add(GameObject item){
		//add to contents
		contents.Add (item);
		//update tag to match this item
		itemType = item.tag;

		//tell the item it's in the inventory
		item.GetComponent<PickupController> ().inInventory = true;

		//if there's more than one item, set the counter
		if (contents.Count > 1) {
			counter.SetActive (true);
			counter.GetComponentInChildren<Text>().text = contents.Count.ToString();
		}
	}

	/* Remove
	 * Removes given item from this slot
	 */
	public void Remove(GameObject item){
		//remove item from contents
		if (!contents.Remove (item))
			Debug.LogWarning ("You tried to remove " + item + " from inventory, but it was not in that slot");
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

/* InventoryController Class
 * Manages adding and removing items to/from inventory and organizing them within it.
 */
public class InventoryController : MonoBehaviour {

	public int numSlots;
	public float slotSize;
	public float slotGapSize;
	public float speed;
	public GameObject canvas;
	public GameObject counterPrefab;
	public int maxSlotCapacity;

	private List<Slot> slots; //list of Slot objects which can be filled with items

	void Start(){
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

	/* Add
	 * Adds given item to inventory
	 */
	public void Add(GameObject item){
		foreach (Slot slot in slots) {
			//if there's a slot holding this kind of thing already and it's not full
			if (slot.itemType == item.tag && slot.contents.Count < maxSlotCapacity){
				//if it has this item already something's gone wonky
				if (slot.contents.Contains(item)){
					Debug.LogWarning("You tried to add " + item + " to inventory twice.");
					return;
				}
				//tell item it's no longer pickable
				item.GetComponent<PickupController> ().pickable = false;
				//add this thing to that slot
				slot.Add (item);
				//set the item to zoom into place in that slot
				StartCoroutine(MoveIntoInventory(item, slot.position));
				return;
			}
		}

		//otherwise, add the item to the first empty slot
		foreach (Slot slot in slots) {
			if (slot.itemType == null) {
				//tell item it's no longer pickable
				item.GetComponent<PickupController> ().pickable = false;
				slot.Add (item);
				StartCoroutine(MoveIntoInventory(item, slot.position));
				return;
			}
		}

		//if there's no empty slot nothing happens
	}

	IEnumerator MoveIntoInventory(GameObject item, Vector3 destination){
		//move toward destination
		while(!Mathf.Approximately (item.transform.position.y, destination.y)){
			item.transform.position = Vector3.MoveTowards (item.transform.position, destination, speed*Time.deltaTime);
			yield return null;
		}
		item.transform.position = destination;
	}

	/* Remove
	 * Removes given item from inventory
	 */
	public void Remove(GameObject item){
		//check each slot for the item and remove it
		foreach (Slot slot in slots) {
			if (slot.itemType == item.tag) {
				slot.Remove (item);
				//reshuffle inventory in case we have a gap now
				ReShuffle ();
				return;
			}
		}
	}


	public void RemoveAndDestroy(GameObject item){
		Remove (item);
		GameObject.Destroy (item);
	}

	/* ReShuffle
	 * Rearranges items in slots so we don't have gaps left to right
	 */
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
							Remove(item);
							Add(item);
							break;
						}
					}
				}
			}
		}
	}

	/*Contains
	 * returns an int representing the number of objects of type tag are in the inventory right now
	 */
	public int Contains(string tag){
		int counter = 0;
		foreach (Slot slot in slots) {
			if (slot.itemType == tag){
				counter = counter + slot.contents.Count;
			}
		}
		return counter;
	}

	/* GetFightingPlant
	 * returns a fighting plant from inventory
	 */
	public GameObject GetFightingPlant (){
		foreach (Slot slot in slots) {
			if (slot.itemType == "FightingPlant") {
				return slot.contents [0];
			}
		}
		return null;
	}


	/*GetRandom
	 * returns a random item from inventory
	 */
	/*
	public GameObject GetRandom (){
		//see how many slots have stuff in them
		int slotsWithStuff = 0;
		foreach (Slot slot in slots) {
			if (slot.contents.Count > 0) {
				slotsWithStuff++;
			}
			else break;
		}

		//return null if inventory is empty
		if (slotsWithStuff == 0)
			return null;

		//return a random item from a random slot with stuff in it.
		Slot thisSlot = slots [Random.Range (0, slotsWithStuff)];
		return thisSlot.contents [Random.Range (0, thisSlot.contents.Count)];
	}
	*/
}