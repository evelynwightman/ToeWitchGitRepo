using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour {

	public int columns = 8;
	public int rows = 7;
	public Sprite[] dirtSprites;
	[HideInInspector]
	public float porchNorth = 4.5f;
	[HideInInspector]
	public float porchSouth = 1.5f;
	[HideInInspector]
	public float porchEast = 1.5f;
	[HideInInspector]
	public float nurseryNorth = 5.5f;
	[HideInInspector]
	public float nurserySouth = 0.5f;
	[HideInInspector]
	public float nurseryEast = 2.5f;
	[HideInInspector]
	public float yardNorth = 6.5f;
	[HideInInspector]
	public float yardSouth = -0.5f;
	[HideInInspector]
	public float yardEast = 7.5f;
	[HideInInspector]
	public float yardWest = -0.5f;

	void Start(){
		//randomise the dirt
		Transform dirtParent = transform.Find ("Dirt");
		foreach(Transform child in dirtParent){
			child.GetComponent<SpriteRenderer> ().sprite = dirtSprites[Random.Range(0, dirtSprites.Length)];
			//turns out if you set something in the inspector it overrides forever, even if you hide it in the inspector later.
			porchNorth = 4.5f;
			porchSouth = 1.5f;
			porchEast = 1.5f;
			nurseryNorth = 5.5f;
			nurserySouth = 0.5f;
			nurseryEast = 2.5f;
			yardNorth = 6.5f;
			yardSouth = -0.5f;
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

}