using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeightStep : sbyte {Trough = -2, Down, Middle, Up, Crest};
public class TileHeightController : MonoBehaviour {
	public float m_StepAmplitude;
	public HeightStep m_HeightState;
	public HeightStep HeightState{
		get{return m_HeightState;}
		set{m_HeightState = value;
			if(m_HeightState == HeightStep.Crest){
				m_IsIncrementing = false;
			}
			else if(m_HeightState == HeightStep.Trough){
				m_IsIncrementing = true;
			}
		}
	}
	public GameObject m_BaseModel;
	public GameObject m_TopModel;
	private bool m_IsIncrementing = false;
	private bool m_CycleComplete = true;
	private static float m_DefaultTileHeight = 1f;
	// Use this for initialization
	void Start () {
		UpdateHeight();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void NextCycleStep(){
		if(!m_CycleComplete){
			if(m_IsIncrementing){
				HeightState++;
			}
			else{
				HeightState--;
			}
			UpdateHeight();
			if(HeightState == HeightStep.Middle && !m_IsIncrementing && !m_CycleComplete){
				m_CycleComplete = true;
				m_IsIncrementing = false;
			}
		}
		
	}
	public void BeginCycle(){
		m_CycleComplete = false;
	}

	public float GetTileHeight(){
		return  m_DefaultTileHeight + (m_StepAmplitude * (sbyte)HeightState);
	}
	private void UpdateHeight(){
		float tileHeight = m_DefaultTileHeight + (m_StepAmplitude * (sbyte)HeightState);
		float upOffset = (m_StepAmplitude * (sbyte)HeightState) / 2f;

		m_BaseModel.transform.localScale = new Vector3(1f, tileHeight, 1f);

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
