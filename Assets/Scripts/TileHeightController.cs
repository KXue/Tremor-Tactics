using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeightStep : sbyte {Trough = -2, Down, Middle, Up, Crest};
public class TileHeightController : MonoBehaviour {
	public float m_StepAmplitude;
	public HeightStep m_HeightState;
	public GameObject m_BaseModel;
	public GameObject m_TopModel;
	private float m_TileHeight;
	private bool m_IsIncrementing = true;
	private static float m_DefaultTileHeight = 1f;
	// Use this for initialization
	void Start () {
		UpdateHeight();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void NextStep(){
		if(m_IsIncrementing){
			m_HeightState++;
			if(m_HeightState == HeightStep.Crest){
				m_IsIncrementing = false;
			}
		}
		else{
			m_HeightState--;
			if(m_HeightState == HeightStep.Trough){
				m_IsIncrementing = true;
			}
		}
		UpdateHeight();
	}

	public float GetTileHeight(){
		return m_TileHeight;
	}
	private void UpdateHeight(){
		m_TileHeight = m_DefaultTileHeight + (m_StepAmplitude * (sbyte)m_HeightState);
		float upOffset = (m_StepAmplitude * (sbyte)m_HeightState) / 2f;

		m_BaseModel.transform.localScale = new Vector3(1f, m_TileHeight, 1f);

		m_BaseModel.transform.localPosition = new Vector3(0, upOffset, 0);

		m_TopModel.transform.localPosition = new Vector3(0, 2 * upOffset, 0);
		/*
		float existingX = transform.localPosition.x;
		float existingZ = transform.localPosition.z;
		transform.localScale = new Vector3(1f, m_TileHeight, 1f);
		
		transform.localPosition = new Vector3(existingX, upOffset, existingZ);
		*/
	}

	
}
