using System;
using UnityEngine;
using UnityEngine.UI;

public enum BossStates{Attack, Wave, Idle}
[Serializable]
public class BossManager : CharacterManager{
	private BossStates m_CurrentState = BossStates.Attack;

	public BossStates CurrentState{
		get{return m_CurrentState;}}

	public void NextMove(){
		Debug.Log(m_CurrentState);
		if(m_CurrentState == BossStates.Wave){
			m_MapController.GenerateWave(m_X, m_Y);
			//play landing Animation
			m_CurrentState = BossStates.Attack;
		}
		else if(m_CurrentState == BossStates.Attack){
			CharacterManager[] targets = m_MapController.GetPlayersInRadius(m_X, m_Y, m_AtkRng);
			if(targets.Length == 0){
				m_CurrentState = BossStates.Idle;
			}
			else{
				//probably won't have any units with 1000 def :D
				int lowestDefense = 1000;
				int weakestTargetIndex = -1;
				for(int i = 0; i < targets.Length; i++){
					if(targets[i].m_Def < lowestDefense){
						lowestDefense = targets[i].m_Def;
						weakestTargetIndex = i;
					}
				}
				CharacterManager target = targets[weakestTargetIndex];
				target.TakeDamage(m_MapController.CalculateDamage(m_X, m_Y, target.m_X, target.m_Y, m_Atk));
			}
		}
		if(m_CurrentState == BossStates.Idle){
			m_CurrentState = BossStates.Wave;//play boss jump animation
		}
	}
}

[Serializable]
public class CharacterManager{
	public bool Selected{
		get{
			return m_CharacterScript.m_Selected;
		}set{
			m_CharacterScript.m_Selected = value;
		}
	}
	public int m_X;
	public int m_Y;
	public bool m_Alive = true;
	public bool m_IsDone = false;
	public bool Done{
		get{return m_IsDone;}
		set{m_IsDone = value;
		if(m_IsDone){
			SetEnabled(false);
		}
		else{
			SetEnabled(true);
		}}
	}
	public int m_MaxHP;
	public int m_Atk;
	public int m_Def;
	public int m_AtkRng;
	public int m_Spd;
	public float m_Acc;
	[HideInInspector] public GameObject m_Instance;
	[HideInInspector] public MapController m_MapController;
	public Sprite m_Sprite;
	private Slider m_HealthSlider;
	private Image m_FillImage;
	private int m_CurrentHP;
	private Character m_CharacterScript;
	public void Setup(){
		m_CurrentHP = m_MaxHP;
		m_CharacterScript = m_Instance.GetComponentInChildren<Character>();
		m_Instance.GetComponentInChildren<SpriteRenderer>().sprite = m_Sprite;
		m_HealthSlider = m_Instance.GetComponentInChildren<Slider>();
		m_FillImage = m_Instance.GetComponentInChildren<Image>();
		m_HealthSlider.maxValue = m_MaxHP;
		UpdateHealthUI();
	}
	public void TakeDamage(int atk){
		m_CurrentHP -= Mathf.Max(atk * 2 / m_Def, 1);
		UpdateHealthUI();
		CheckDeath();
	}
	public void HealDamage(int amount){
		m_CurrentHP = Mathf.Min(m_CurrentHP + amount, m_MaxHP);
	}
	public void TileHeightChanged(float height){
		Vector3 temp = new Vector3(0f, (height - 1 - m_Instance.transform.localPosition.y), 0f);
		m_Instance.transform.localPosition += temp;
	}
	private void CheckDeath(){
		if(m_CurrentHP <= 0){
			//Do Death stuff
		}
	}
	public void SetEnabled(bool enabled){
		m_CharacterScript.enabled = enabled;
		m_IsDone = false;
	}
	private void UpdateHealthUI(){
		m_HealthSlider.value = m_CurrentHP;
		m_FillImage.color = Color.Lerp (Color.red, Color.green, ((float)m_CurrentHP / (float)m_MaxHP));
	}
	public void MoveTo(TileManager m){
		m_X = m.m_X;
		m_Y = m.m_Y;
		Vector3 worldPosition = m_MapController.MapToWorldPosition(m_X, m_Y);
		Vector3 deltaPosition = new Vector3(worldPosition.x - m_Instance.transform.position.x,
			0f, worldPosition.z - m_Instance.transform.position.z);
		m_Instance.transform.position += deltaPosition;
		TileHeightChanged(m.TileHeight());
	}
}
