using UnityEngine;
using UnityEditor;

using System.Collections;
using System.IO;

[CustomEditor(typeof(GridScript))]
public class GridEditorScript : Editor {

	GridScript grid; //An instance of the GridScript
	
	private int oldIndex;
	
	void OnEnable(){
		oldIndex = 0;
		grid = (GridScript)target;
	}
	
	[MenuItem("Assets/Create/Tileset")] //Tells the computer where to create the TileSet ScriptableObject
	static void CreateTileSet(){
		var asset = ScriptableObject.CreateInstance<TileSet>();
		var path = AssetDatabase.GetAssetPath(Selection.activeObject);
		
		if(string.IsNullOrEmpty(path)){
			path = "Assets";
		}
		else if(Path.GetExtension(path) != ""){
			path = path.Replace(Path.GetFileName(path), "");
		}
		else{
			path += "/";
		}
		
		var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "TileSet.asset");
		AssetDatabase.CreateAsset(asset, assetPathAndName);
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
		asset.hideFlags = HideFlags.DontSave;
	}
	
	public override void OnInspectorGUI(){
		//base.OnInspectorGUI();
		//Activates when user selects the parent Grid object with GridScript.cs in dropdown mode
		
		grid.width = createSlider("Grid Width", grid.width); //Original code
		grid.height = createSlider("Grid Height", grid.height); //Original code
		grid.tileWidth = createSlider("Tile Width", grid.tileWidth); //Sets the horizontal scaling for the prefab
		grid.tileHeight = createSlider("Tile Height", grid.tileHeight); //Sets the vertical scaling for the prefab
		
		if(GUILayout.Button("Open Grid Window")){
			GridWindowScript window = (GridWindowScript)EditorWindow.GetWindow(typeof(GridWindowScript));
			window.init();
		}
		
		//Tile prefab 
		EditorGUI.BeginChangeCheck();
		var newTilePrefab = (Transform)EditorGUILayout.ObjectField("Tile Prefab", 
												grid.tilePrefab, typeof(Transform), false);
		if(EditorGUI.EndChangeCheck()) {
			grid.tilePrefab = newTilePrefab;
			Undo.RecordObject(target, "Grid Changed");
		}
		
		//Tile map
		EditorGUI.BeginChangeCheck();
		var newTileSet = (TileSet)EditorGUILayout.ObjectField("Tileset", grid.tileSet, typeof(TileSet), false);
		if(EditorGUI.EndChangeCheck()){
			grid.tileSet = newTileSet;
			Undo.RecordObject(target, "Grid Changed");
		}
		
		if(grid.tileSet != null){
			EditorGUI.BeginChangeCheck();
			var names = new string[grid.tileSet.prefabs.Length];
			var values = new int[names.Length];
			
			for(int i = 0; i < names.Length; i++){
				names[i] = grid.tileSet.prefabs[i] != null ? grid.tileSet.prefabs[i].name : "";
				values[i] = i;
			}
			
			var index = EditorGUILayout.IntPopup("Select Tile", oldIndex, names, values);
			
			if(EditorGUI.EndChangeCheck()){
				Undo.RecordObject(target, "Grid Changed");
				if(oldIndex != index){
					oldIndex = index;
					grid.tilePrefab = grid.tileSet.prefabs[index];
					
					float width = grid.tilePrefab.GetComponent<Renderer>().bounds.size.x;
					float height = grid.tilePrefab.GetComponent<Renderer>().bounds.size.y;
					
					grid.width = width;
					grid.height = height;
				}
			}
		}
	}
	
	private float createSlider(string labelName, float sliderPosition) {
		GUILayout.BeginHorizontal();
		GUILayout.Label(labelName);
		sliderPosition = EditorGUILayout.Slider(sliderPosition, 1f, 31, null); // Slider goes from 1 to 39
		GUILayout.EndHorizontal();
		
		return sliderPosition;
	}
	
	void OnSceneGUI(){
		int controlId = GUIUtility.GetControlID(FocusType.Passive);
		Event e = Event.current; //Read input
		Ray ray = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, 
								-e.mousePosition.y + Camera.current.pixelHeight));
		Vector3 mousePos = ray.origin;
		
		if(e.isMouse && e.type == EventType.MouseDown && e.button == 0){
			GUIUtility.hotControl = controlId;
			e.Use();
			
			GameObject gameObject;
			Transform prefab = grid.tilePrefab;
			
			if(prefab){ //Only run this if prefab is not null
				Undo.IncrementCurrentGroup();
				Vector3 alligned = new Vector3((Mathf.Floor(mousePos.x/grid.width)*grid.width + grid.width/2.0f),
				                               Mathf.Floor(mousePos.y/grid.height)*grid.height + grid.height/2.0f,
				                               0.0f);
				if(GetTransformFromPosition(alligned) != null){
					return; //Do not place prefab if one already exists
				}
				gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject);
				gameObject.transform.position = alligned;
				gameObject.transform.parent = grid.transform;
				gameObject.transform.localScale = new Vector3(grid.tileWidth, grid.tileHeight, 0f); //Set scaling to slider values
				Undo.RegisterCreatedObjectUndo(gameObject, "Create" + gameObject.name);
			}
		}
		
		if(e.isMouse && e.type == EventType.MouseDown && e.button == 1){
			GUIUtility.hotControl = controlId;
			e.Use ();
			Vector3 alligned = new Vector3(Mathf.Floor(mousePos.x/grid.width)*grid.width + grid.width/2.0f,
			                               Mathf.Floor(mousePos.y/grid.height)*grid.height + grid.height/2.0f,
			                               0.0f);
			Transform transform = GetTransformFromPosition(alligned);
			if(transform != null){
				DestroyImmediate(transform.gameObject); //Destroy object if right-clicked
			}
		}
		
		if(e.isMouse && e.type == EventType.MouseUp){
			GUIUtility.hotControl = 0;
		}
	}
	
	Transform GetTransformFromPosition(Vector3 alligned){
		int i = 0;
		while(i < grid.transform.childCount){ //Cycle through all "placed" prefabs to test for a given position
			Transform transform = grid.transform.GetChild(i);
			if(transform.position == alligned){
				return transform; //Prefab already exists here
			}
			
			i++;
		}
		return null; //No prefab exists here
	}

}
