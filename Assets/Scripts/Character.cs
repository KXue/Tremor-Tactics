using System;
using UnityEngine;
public class Character : MonoBehaviour {
	public bool m_Selected;
	// Use this for initialization
	void Start () {
	}
	void OnMouseDown(){
		Debug.Log("Character clicked");
		m_Selected = !m_Selected;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
