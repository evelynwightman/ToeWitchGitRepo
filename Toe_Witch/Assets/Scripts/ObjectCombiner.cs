using UnityEngine;
using System.Collections;

public class ObjectCombiner : MonoBehaviour {

	public GameObject Tomatoe;

	private GameObject newThing;

	void ExecuteCombination(GameObject plant){

		//Find out what they are
		string they = plant.name;
		string[] theySplit = they.Split (' ');
		they = theySplit [0];

		//Find out what we are
		string me = transform.parent.name;
		string[] meSplit = me.Split (' ');
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

	public void Combine(){
		GameObject plant = FindPlant ();

		if (plant != null) {
			ExecuteCombination(plant);
		}
	}

	GameObject FindPlant(){
		return FindPlant (transform.position);
	}

	public GameObject FindPlant(Vector3 position){
		//Find all the things that are also on this spot
		RaycastHit2D[] hits;
		Ray ray = new Ray (Camera.main.transform.position, position - Camera.main.transform.position);
		hits = Physics2D.RaycastAll (ray.origin, ray.direction);

		//If any of them are a plant, return the plant
		foreach (UnityEngine.RaycastHit2D item in hits){
			if (item.transform.tag == "Plant" || item.transform.tag == "FightingPlant") {
				return item.transform.gameObject;
			}
		}
		return null;
	}
}
