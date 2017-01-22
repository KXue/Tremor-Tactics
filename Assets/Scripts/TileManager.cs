using System;
using UnityEngine;

[Serializable]
public class TileManager{
	public int m_X;
	public int m_Y;
	public GameObject m_Instance;
	private TileHeightController m_HeightController;
	private TileHighlightController m_HighlightController;

	public void Setup(){
		m_HeightController = m_Instance.GetComponent<TileHeightController>();
		m_HighlightController = m_Instance.GetComponentInChildren<TileHighlightController>();
	}
	//lots of inefficiencies. Will have to flatten hierarchy after ship
	public void BeginCycle(){
		m_HeightController.BeginCycle();
	}
	public HeightStep GetHeightStep(){
		return m_HeightController.HeightState;
	}
	public void NextCycleStep(){
		m_HeightController.NextCycleStep();
	}
	public void SetHeightStep(HeightStep step){
		m_HeightController.HeightState = step;
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
	public float TileHeight(){
		return m_HeightController.GetTileHeight();
	}
	public void SetEnabled(bool enabled){
		m_HighlightController.Selectable = enabled;
	}
	public HighlightType GetHighlight(){
		return m_HighlightController.GetHightlight();
	}
}
