/* BoardManager 
 * Keeps track of where game areas are (yard, porch, nursery), and randomizes any board aspects that need randomizing on
 * setup.
 * 
 * Copyright (c) 2016 by Evelyn Wightman. All rights reserved. 
 * Subject to the terms and conditions contained in LICENSE file.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {

	//board dimensions
	public int columns = 8;
	public int rows = 7;
	public Sprite[] dirtSprites;
	//area boundaries
	[HideInInspector]
	public float porchNorth;
	[HideInInspector]
	public float porchSouth;
	[HideInInspector]
	public float porchEast;
	[HideInInspector]
	public float nurseryNorth;
	[HideInInspector]
	public float nurserySouth;
	[HideInInspector]
	public float nurseryEast;
	[HideInInspector]
	public float yardNorth;
	[HideInInspector]
	public float yardSouth;
	[HideInInspector]
	public float yardEast;
	[HideInInspector]
	public float yardWest;
	public GameObject potPrefab;

	private GameObject[] grasses;

	void Start(){
		//randomise the dirt
		Transform dirtParent = transform.Find ("Dirt");
		foreach (Transform child in dirtParent) {
			child.GetComponent<SpriteRenderer> ().sprite = dirtSprites [Random.Range (0, dirtSprites.Length)];
			//turns out if you set something in the inspector it overrides forever, even if you hide it in the inspector later, so I need to 
			//set these in Start()??.
			porchNorth = 4.5f;
			porchSouth = 1.5f;
			porchEast = 1.5f;
			nurseryNorth = 5.5f;
			nurserySouth = 0.5f;
			nurseryEast = 2.5f;
			yardNorth = 6.5f;
			yardSouth = 0f; //-0.5f; not actually the south border, but where I want things to stop
			yardEast = 7.5f;
			yardWest = -0.5f;
		}
	}

	/* IsInYard
	 * returns true if given point is in yard, false otherwise
	 * includes nursery and porch in yard
	 */
	public bool IsInYard(Vector3 position){
		if (position.x >= yardWest && position.x <= yardEast && position.y >= yardSouth && position.y <= yardNorth) {
			return true;
		}
		return false;
	}

	/* IsInNursery
	 * returns true if given point is in nursery, false otherwise
	 * includes porch in nursery
	 */
	public bool IsInNursery(Vector3 position){
		if (position.y >= nurserySouth)
		if (position.x >= yardWest && position.x <= nurseryEast && position.y >= nurserySouth && position.y <= nurseryNorth) {
			return true;
		}
		return false;
	}

	/* IsOnPorch
	 * returns true if given point is on porch, false otherwise
	 */
	public bool IsOnPorch(Vector3 position){
		if (position.x >= yardWest && position.x <= porchEast && position.y >= porchSouth && position.y <= porchNorth) {
			return true;
		}
		return false;
	}

	/* CheckLawn 
	 * checks to see if all grass is dead. If so, tell GameManager it's all over
	 */
	public void CheckLawn(){
		//find all the grass so we can check if they're dead
		grasses = GameObject.FindGameObjectsWithTag ("Grass");

		//check each grass
		foreach (GameObject grass in grasses) {
			//if it's alive and planted, return
			if (!grass.GetComponent<GrassController> ().isDead && 
				grass.GetComponent<GrassController> ().planted) {
				return;
			}
		}
		//if you haven't returned by now, all the grass is dead, so tell the GameManager we lose.
		StartCoroutine(GameObject.Find("GameManager").GetComponent<GameManager>().GameOver());
	}

	/* PlacePots
	 * Destroys all existing pots and replaces them with num number of pots, rounded down to an even number so thigns stay symmetric
	 */
	public void PlacePots(int num){
		//find and destroy all child pots
		GameObject potFather = transform.FindChild("Pots").gameObject;
		Queue<GameObject> potsToDestroy = new Queue<GameObject>();
		foreach (Transform child in potFather.transform) {
			potsToDestroy.Enqueue (child.gameObject);
		}
		GameObject pot;
		while (potsToDestroy.Count > 0) {
			pot = potsToDestroy.Dequeue();
			Destroy (pot.gameObject);
		}

		//instantiate new pots and make them children of the potFather
		if (num > 6) {
			pot = (GameObject)Instantiate (potPrefab, new Vector3 (-3.5f, 1.5f, 0) + potFather.transform.position, Quaternion.identity);
			pot.transform.parent = potFather.transform;
			pot = (GameObject)Instantiate (potPrefab, new Vector3 (-3.5f, -2.5f, 0)+ potFather.transform.position, Quaternion.identity);
			pot.transform.parent = potFather.transform;
		}
		if (num > 4) {
			pot = (GameObject)Instantiate (potPrefab, new Vector3 (-2.5f, 1.5f, 0)+ potFather.transform.position, Quaternion.identity);
			pot.transform.parent = potFather.transform;
			pot = (GameObject)Instantiate (potPrefab, new Vector3 (-2.5f, -2.5f, 0)+ potFather.transform.position, Quaternion.identity);
			pot.transform.parent = potFather.transform;
		}
		if (num > 2) {
			pot = (GameObject)Instantiate (potPrefab, new Vector3 (-1.5f, 1.5f, 0)+ potFather.transform.position, Quaternion.identity);
			pot.transform.parent = potFather.transform;
			pot = (GameObject)Instantiate (potPrefab, new Vector3 (-1.5f, -2.5f, 0)+ potFather.transform.position, Quaternion.identity);
			pot.transform.parent = potFather.transform;
		}
		if (num > 0) {
			pot = (GameObject)Instantiate (potPrefab, new Vector3 (-1.5f, 0.5f, 0)+ potFather.transform.position, Quaternion.identity);
			pot.transform.parent = potFather.transform;
			pot = (GameObject)Instantiate (potPrefab, new Vector3 (-1.5f, -1.5f, 0)+ potFather.transform.position, Quaternion.identity);
			pot.transform.parent = potFather.transform;
		}
	}

}