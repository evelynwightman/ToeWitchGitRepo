/* TextHandler
 * Evelyn Wightman 2016
 * Reads text in from file and stores it in a dictionary. Public Get allows text to be fetched by key.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TextHandler : MonoBehaviour {

	public TextAsset textFile;

	private Dictionary<string, string> textBlobs = new Dictionary<string, string>();

	void Awake () {
		//build the textBlobs dictionary from the text file
		using (StringReader reader = new StringReader (textFile.text)) {
			string key;
			string text;
			string[] splitLine = new string[2];
			string line = reader.ReadLine();

			//until we reach the end of the string
			while (line != null)
			{
				//split off the first word in the line and make it the key. The remainder is the text
				splitLine = line.Split(new char[] {' '}, 2);
				key = splitLine [0];
				text = splitLine [1];

				//add the key/text pair as an entry in the dictionary
				textBlobs.Add (key, text);

				//read the next line
				line = reader.ReadLine ();
			}

		}
	}

	/* GetText
	 * returns text associated with given key
	 */
	public string Get(string key){
		string value;
		textBlobs.TryGetValue (key, out value);
		if (value != null) {
			return value;
		} else
			Debug.LogError ("No text found with key " + key);
		return "";
	}
}
