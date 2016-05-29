using UnityEngine;
using System.Collections;

public class GridScript : MonoBehaviour {

	public float width = 1.0f; //width of a given grid square
	public float height = 1.0f; //height of a given grid square
	
	public Transform tilePrefab; //Represents the prefab to be placed
	public TileSet tileSet; //Allows user to switch between prefabs without dragging/dropping
	public float tileWidth = 1f; //Respresents scaling for prefab
	public float tileHeight = 1f; //Respresents scaling for prefab
	
	public Color color = Color.white; //color of the girlines

	void OnDrawGizmos(){
		Vector3 pos = Camera.current.transform.position;
		Gizmos.color = this.color;
		
		for(float y = pos.y -800.0f; y < pos.y + 800.0f; y += this.height) {
			Gizmos.DrawLine(new Vector3(-1000000.0f, (Mathf.Floor(y/this.height)*this.height), 0.0f), 
							new Vector3(1000000.0f, (Mathf.Floor(y/this.height)*this.height), 0.0f));
		}
		
		for(float x = pos.x -1200.0f; x < pos.x + 1200.0f; x += this.width) {
			Gizmos.DrawLine(new Vector3((Mathf.Floor(x/this.width)*this.width), -1000000.0f, 0.0f), 
			                new Vector3((Mathf.Floor(x/this.width)*this.width), 1000000.0f, 0.0f));
		}
	}
}
