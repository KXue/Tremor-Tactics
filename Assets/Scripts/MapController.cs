using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DirectionEnum : byte {DownRight, UpRight, Up, UpLeft, DownLeft, Down};
public class WaveObject{
	private int m_X;
	private int m_Y;
}
public class MapController : MonoBehaviour {
	private static int[][] directions = new int[][]{
		new int[]{1,0},
		new int[]{1,-1},
		new int[]{0,-1},
		new int[]{-1,0}, 
		new int[]{-1,1}, 
		new int[]{0,1}
	};
	public int m_Width;
	public int m_Height;
	public int[] m_Blanks;
	public GameObject[] m_TilePrefabs;
	private Dictionary<string, TileManager> m_Map;

	private Vector3 m_XOffset = new Vector3(1.5f, 0f, 0.866f);
	private Vector3 m_YOffset = new Vector3(0f, 0f, 1.732f);
	private TileManager m_SelectedTileManager = null;
	private float waveTime = 0;
	// Use this for initialization
	void Start () {
		GenerateMap();
		m_Map[GeneratePositionString(0, 1)].Highlight(HighlightType.Traverseable);
		m_Map[GeneratePositionString(1, 0)].Highlight(HighlightType.Attackable);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			Dictionary<string, TileManager>.ValueCollection valueColl =
   				m_Map.Values;
			bool clickResolved = false;
			foreach(TileManager t in valueColl){
				//Another tile clicked
				if(m_SelectedTileManager != t && t != null && t.IsSelected()){
					if(m_SelectedTileManager != null){
						m_SelectedTileManager.SetSelected(false);
						m_SelectedTileManager = t;
					}
					
					clickResolved = true;
					m_SelectedTileManager = t;
				}
			}
			//Either clicked selected tile or outside of board
			if(!clickResolved){
				m_SelectedTileManager = null;
			}
		}
		waveTime += Time.deltaTime;
		if(waveTime >= 1){
			m_Map[GeneratePositionString(0, 0)].NextStep();
			waveTime = 0;
		}
	}
	public bool TryAddTile(int x, int y){
		string positionString = GeneratePositionString(x, y);
		bool retVal = false;
		if(m_Map.ContainsKey(positionString)){
			if(m_Map[positionString] == null){
				retVal = true;
				m_Map[positionString] = CreateTileManager(x, y);
			}
		}
		else{
			retVal = true;
			m_Map.Add(positionString, CreateTileManager(x, y));
		}
		return retVal;
	}
	public int HexDistance(int x1, int y1, int x2, int y2){
		return (Mathf.Abs(x1 - x2)
			+ Mathf.Abs(x1 + y1 - x2 - y2)
			+ Mathf.Abs(y1 - y2)) / 2;
	}
	private void GenerateMap(){
		m_Map = new Dictionary<string, TileManager>();
		//fill in the blanks with null
		for(int i = 0; i < m_Blanks.Length; i+=2){
			m_Map.Add(GeneratePositionString(m_Blanks[i], m_Blanks[i+1]), null);
		}
		string positionString = "";
		for(int i = 0; i < m_Width; i++){
			for(int j = 0; j < m_Height; j++){
				positionString = GeneratePositionString(i, j);
				if(!m_Map.ContainsKey(positionString)){
					m_Map.Add(positionString,
						CreateTileManager(i, j));
				}
			}
		}
	}
	private TileManager CreateTileManager(int x, int y){
		TileManager newTileManager = new TileManager();
		newTileManager.m_Instance = Instantiate(m_TilePrefab[0], 
			MapToWorldPosition(x, y), 
			Quaternion.identity) as GameObject;
		newTileManager.Setup();
		return newTileManager;
	}
	private Vector3 MapToWorldPosition(int x, int y){
		return ((x - (m_Width / 2)) * m_XOffset) + ((y - (m_Height / 2)) * m_YOffset);
	}
	private string GeneratePositionString(int x, int y){
		return (x + "," + y);
	}

	private void GenerateWave(int x, int y){
		string positionString = GeneratePositionString(x, y);
		if(m_Map.ContainsKey(positionString) && m_Map[positionString] != null){

		}
	}
}
