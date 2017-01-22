using System;
using UnityEngine;

[Serializable]
public class TileManager{
	public GameObject m_Instance;
	private TileHeightController m_HeightController;
	private TileHighlightController m_HighlightController;

	public void Setup(){
		m_HeightController = m_Instance.GetComponent<TileHeightController>();
		m_HighlightController = m_Instance.GetComponentInChildren<TileHighlightController>();
	}
	//lots of inefficiencies. Will have to flatten hierarchy after ship
	public void NextStep(){
		m_HeightController.NextStep();
	}
	public bool IsSelected(){
		return m_HighlightController.Selected;
	}
	public void SetSelected(bool isSelected){
		m_HighlightController.Selected = isSelected;
	}
	public void Highlight(HighlightType type){
		m_HighlightController.Highlight(type);
	}
}
