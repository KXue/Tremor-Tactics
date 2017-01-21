using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HighlightType : byte {None, Traverseable, Attackable};
public class TileController : MonoBehaviour {
	private static Color[] HighLightColors = {Color.white, new Color(0.22f, 0.28f, 0.85f, 0.588f), new Color(0.85f, 0.2f, 0.22f, 0.588f)};
	public MeshRenderer m_BaseMesh;
	private bool m_Selected = false;
	public HighlightType m_Highlight = HighlightType.None;
	// Use this for initialization
	void Start () {
		UpdateAppearance();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void SetSelect(bool selected){
		m_Selected = selected;
		UpdateAppearance();
	}
	public void Highlight(HighlightType type){
		m_Highlight = type;
		UpdateAppearance();

	}
	private void UpdateAppearance(){
		m_BaseMesh.material.color = HighLightColors[(int)m_Highlight];
	}
}
