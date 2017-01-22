using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public float m_ScreenEdgeBuffer = 10f;
	public float m_BottomClamp;
	public float m_TopClamp;
	public float m_LeftClamp;
	public float m_RightClamp;
	public float m_CameraSpeed = 0.5f;
	private float m_HOffset = 0;
	private float m_VOffset = 0;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.mousePosition.x < m_ScreenEdgeBuffer){
			m_HOffset = -1;
		}
		else if(Input.mousePosition.x > (Screen.width - m_ScreenEdgeBuffer)){
			m_HOffset = 1;
		}
		else{
			m_HOffset = 0;
		}

		if (Input.mousePosition.y < m_ScreenEdgeBuffer){
			m_VOffset = -1;
		}
		else if(Input.mousePosition.y > (Screen.height - m_ScreenEdgeBuffer)){
			m_VOffset = 1;
		}
		else{
			m_VOffset = 0;
		}
    	//transform.LookAt(mycam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mycam.nearClipPlane)), Vector3.up);
	}
	void FixedUpdate()
	{
		Vector3 directionVector = new Vector3(
			m_HOffset, 
			0, 
			m_VOffset);
		directionVector.Normalize();
		Vector3 newPositionVector = transform.position + (directionVector * m_CameraSpeed);
		Vector3 clampedPositionVector = new Vector3(
			Mathf.Clamp(newPositionVector.x, m_LeftClamp, m_RightClamp), 
			0, 
			Mathf.Clamp(newPositionVector.z, m_BottomClamp, m_TopClamp));

		transform.position = clampedPositionVector;
	}
}
