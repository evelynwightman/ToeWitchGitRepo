using UnityEngine;
using System.Collections;

public class StaticObjectController : MonoBehaviour {

	protected SpriteRenderer spriteRenderer;

	protected virtual void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();

		//keep everything overlapping according to y position
		spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
	}
}
