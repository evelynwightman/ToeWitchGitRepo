/* SpawnManager
 * Evelyn Wightman 2016
 * Spawns Tramplers. When tramplers return to the spawn point, it re-deploys them rather than create a new one.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour {

	public GameObject villagerPrefab;

	private List<GameObject> waitingTramplers = new List<GameObject>();

	/* OnTriggerEnter2D
	 * Adds returning tramplers to list for later redeployment
	 */
	void OnTriggerEnter2D(Collider2D other){
		//when a trampler comes home from the witch's
		if (other.tag == "Trampler") {
			if (other.gameObject.GetComponent<TramplerController> ().leaving == true) {
				//deactivate it and put it in the list of tramplers to deploy later
				waitingTramplers.Add (other.gameObject);
				other.gameObject.SetActive (false);
			}
		}
	}

	/* SpawnTrampler
	 * If there's a trampler waiting to go, reset it and send it out. If not, make a new one. 
	 */
	public GameObject SpawnTrampler(){
		GameObject trampler;
		if (waitingTramplers.Count == 0) {
			//instantiate a new trampler
			trampler = villagerPrefab;
			Quaternion spawnRotation = Quaternion.identity;
			trampler = (GameObject)Instantiate (trampler, transform.position, spawnRotation);
			trampler.SetActive (true); //because OnTriggerEnter will deactivate on instantiate
		} else {
			//activate a waiting trampler
			trampler = waitingTramplers[0];
			waitingTramplers.Remove (trampler);
			trampler.SetActive(true);
			trampler.GetComponent<TramplerController>().Restart();

		}
		return trampler;
	}

	/* OnDayEnd
	 * Clear out the list of waiting tramplers.
	 */
	public void OnDayEnd(){
		waitingTramplers.Clear ();
	}
}
