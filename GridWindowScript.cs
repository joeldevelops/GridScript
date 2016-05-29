using UnityEngine;
using UnityEditor;
using System.Collections;

public class GridWindowScript : EditorWindow {

	//Opens the GridWindow to allow a user to change the grid color
	
	GridScript grid;

	public void init(){
		grid = (GridScript)FindObjectOfType(typeof(GridScript));
	}
	
	void OnGUI(){
		grid.color = EditorGUILayout.ColorField(grid.color, GUILayout.Width(200));
	}
}
