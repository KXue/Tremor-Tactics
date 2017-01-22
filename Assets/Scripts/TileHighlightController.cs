using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HighlightType : byte {None, Traverseable, Attackable};
public class TileHighlightController : MonoBehaviour {
	private static Color[] HighLightColors = {Color.white, new Color(0.22f, 0.28f, 0.85f, 0.588f), new Color(0.85f, 0.2f, 0.22f, 0.588f)};
	public MeshRenderer m_BaseMesh;
	public GameObject m_TileRing;
	public bool Selected{

		get{return m_Selected;}

		set{
			m_Selected = value;
			if(m_Selected == false){
				m_SelectAngle = 0;
			}
		}
	}
	//Period in seconds
	public float m_SelectCyclePeriod;
	//Sine function will be y offset by 1
	public float m_SelectCycleAmplitude;
	private HighlightType m_Highlight = HighlightType.None;
	private bool m_Selected = false;
	//Radians
	private float m_SelectAngle = 0;

	// Use this for initialization
	void Start () {
		UpdateAppearance();
	}
	
	// Update is called once per frame
	void Update () {
		if(m_Selected){
			m_SelectAngle += (Time.deltaTime / m_SelectCyclePeriod) * (2 * Mathf.PI);
			if(m_SelectAngle >= (Mathf.PI * 2)){
				m_SelectAngle -= (Mathf.PI * 2);
			}
		}
	}
	void OnMouseDown()
	{
		Selected = !Selected;
	}
	void FixedUpdate()
	{
		float ringScale = Mathf.Sin(m_SelectAngle) * m_SelectCycleAmplitude + 1;
		m_TileRing.transform.localScale = new Vector3(ringScale, 1f, ringScale);
	}
	public void Highlight(HighlightType type){
		m_Highlight = type;
		UpdateAppearance();

	}
	private void UpdateAppearance(){
		m_BaseMesh.material.color = HighLightColors[(int)m_Highlight];
	}
}
