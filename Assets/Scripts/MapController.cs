using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

	public int m_Width;
	public int m_Height;
	public int[] m_Blanks;
	private Dictionary<string, GameObject> m_Map;
	// Use this for initialization
	void Start () {
		GenerateMap();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void GenerateMap(){
		m_Map = new Dictionary<string, GameObject>();
		for(int i = 0; i < m_Blanks.Length; i+=2){
			m_Map.Add(GeneratePositionString(m_Blanks[i], m_Blanks[i+1]), null);
		}

	}
	private string GeneratePositionString(int x, int y){
		return (x + "," + y);
	}
}
