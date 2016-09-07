/* OtherWitch
 * Evelyn Wightman 2016
 * One instance per witch. Holds the name of the witch and a list of items which she has to trade.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OtherWitch : MonoBehaviour {

	public List<GameObject> giftableItems = new List<GameObject>();
	public List<GameObject> tradableItems = new List<GameObject>();
	public string whichWitch;
}
