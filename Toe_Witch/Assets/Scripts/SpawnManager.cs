using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour {

	public GameObject villagerPrefab;

	private List<GameObject> waitingTramplers = new List<GameObject>();

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Trampler") {
			if (other.gameObject.GetComponent<TramplerController> ().leaving == true) {
				waitingTramplers.Add (other.gameObject);
				other.gameObject.SetActive (false);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		waitingTramplers.Remove (other.gameObject);
	}

	public GameObject SpawnTrampler(){
		GameObject trampler;
		//note: This won't spawn a new trampler until the previously spawned one has left the spawn point
		if (waitingTramplers.Count == 0) {
			//instantiate a new trampler
			trampler = villagerPrefab;
			Quaternion spawnRotation = Quaternion.identity;
			trampler = (GameObject)Instantiate (trampler, transform.position, spawnRotation);
			trampler.SetActive (true); //because OnTriggerEnter will deactivate on instantiate
		} else {
			//activate a waiting trampler
			trampler = waitingTramplers[0];
			trampler.SetActive(true);
			trampler.GetComponent<TramplerController>().Restart();
		}
		return trampler;
	}

	public void OnDayEnd(){
		waitingTramplers.Clear ();
	}
}
