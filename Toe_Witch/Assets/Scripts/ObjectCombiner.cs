/* ObjectCombiner
 * Lives on the Clickable aspect of toes. When the player puts down a toe, she will call Combine to see if there's 
 * anything there the toe should combine with. Handles checking the recipes, creating the new hybrid plant, and 
 * destroying the toe and old plant.
 * 
 * Copyright (c) 2016 by Evelyn Wightman. All rights reserved. 
 * Subject to the terms and conditions contained in LICENSE file.
 */
using UnityEngine;
using System.Collections;

public class ObjectCombiner : MonoBehaviour {

	public GameObject Tomatoe;

	private GameObject newThing;

	/* ExecuteCombination
	 * Decides what this toe and the given plant should become when combined. Creates the new thing and destroys the toe and plant.
	 */
	void ExecuteCombination(GameObject plant){

		//Find out what they are
		string they = plant.name;
		string[] theySplit = they.Split (new char[] {' ','('});
		they = theySplit [0];

		//Find out what we are
		string me = transform.parent.name;
		string[] meSplit = me.Split (new char[] {' ','('});
		me = meSplit [0];

		//By our powers combined, we are...???
		if( (me == "Toe" && they == "Tomato") || (me == "Tomato" && they == "Toe")){
			newThing = Tomatoe;
		}
		else{
			return;
		}

		//destroy the plant
		Destroy(plant.gameObject);

		//instantiate the new hybrid plant
		Quaternion spawnRotation = Quaternion.identity;
		Instantiate (newThing, transform.position, spawnRotation);

		//destroy ourself (the toe)
		Destroy(transform.parent.gameObject);
	}

	/* Combine
	 * Check to see if there's a plant to combine with, and if so calls ExecuteCombination to combine with that plant
	 */
	public void Combine(){
		GameObject plant = FindPlant ();

		if (plant != null) {
			ExecuteCombination(plant);
		}
	}

	/* FindPlant
	 * Checks for a plant at given position. If no position given, checks at current position. Returns null if no plant found.
	 */

	//FindPlant at current position
	public GameObject FindPlant(Vector3 position){
		//Find all the things that are also on this spot
		RaycastHit2D[] hits;
		Ray ray = new Ray (Camera.main.transform.position, position - Camera.main.transform.position);
		hits = Physics2D.RaycastAll (ray.origin, ray.direction);

		//If any of them are a plant, return the plant
		foreach (UnityEngine.RaycastHit2D item in hits){
			if (item.transform.tag == "Plant") {
				return item.transform.gameObject;
			}
		}
		return null;
	}
	//If no position given, FindPlant at current position
	GameObject FindPlant(){
		return FindPlant (transform.position);
	}
}
