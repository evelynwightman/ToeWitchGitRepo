/* OtherWitch
 * One instance per witch. Holds the name of the witch and a list of items which she has to trade.
 * 
 * Copyright (c) 2016 by Evelyn Wightman. All rights reserved. 
 * Subject to the terms and conditions contained in LICENSE file.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OtherWitch : MonoBehaviour {

	public List<GameObject> giftableItems = new List<GameObject>();
	public List<GameObject> tradableItems = new List<GameObject>();
	public string whichWitch;
}
