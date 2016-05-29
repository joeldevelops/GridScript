using UnityEngine;
using System.Collections;

public class TileSet : ScriptableObject {

	//The TileSet class allows you to switch between prefabs quickly in the inspector. 
	//To set up a TileSet, right-click inside the asset folder and go to create > TileSet.
	//In the inspector for the TileSet increase the array size to the desired number(e.g. 5 tiles).
	//Drag and drop the desired prefabs into the inspector then
	//put the TileSet inside the GridScript in the inspector and you are good to go.
	
	public Transform[] prefabs = new Transform[0];
	
}