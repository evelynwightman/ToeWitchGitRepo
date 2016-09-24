/* StaticObjectController
 * Simple class for objects which do not move around. Sets sorting order based on position.
 * 
 * Copyright (c) 2016 by Evelyn Wightman. All rights reserved. 
 * Subject to the terms and conditions contained in LICENSE file.
 */

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
