using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeightStep : sbyte {Trough = -2, Down, Middle, Up, Crest};
public class TileHeightController : MonoBehaviour {

	public GameObject m_BaseTile;
	public GameObject m_TileRing;
	/*[HideInInspector]*/ public float m_StepAmplitude;
	/*[HideInInspector]*/public HeightStep m_HeightState;

	private float m_TileHeight;
	private bool m_IsIncrementing = true;
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
		m_TileHeight = 1 + (m_StepAmplitude * (sbyte)m_HeightState);
		float upOffset = (m_StepAmplitude * (sbyte)m_HeightState) / 2f;
		float existingX = transform.localPosition.x;
		float existingZ = transform.localPosition.z;
		transform.localScale = new Vector3(1f, m_TileHeight, 1f);
		
		transform.localPosition = new Vector3(existingX, upOffset, existingZ);
		/*
		m_BaseTile.transform.localScale = new Vector3(1f, m_TileHeight, 1f);
		m_BaseTile.transform.localPosition = new Vector3(0f, upOffset, 0f);
		m_TileRing.transform.localPosition = new Vector3(0f, m_TileHeight - 0.5f, 0f);
		*/
	}

	
}
