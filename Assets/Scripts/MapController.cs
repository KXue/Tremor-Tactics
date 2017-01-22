using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DirectionEnum : byte {DownRight, UpRight, Up, UpLeft, DownLeft, Down};

public class WaveObject{
	public int m_X;
	public int m_Y;
	public int m_Radius = 1;
	public int m_MaxRadius = 15;
	public bool NextStep(){
		m_Radius++;
		if(m_Radius == m_MaxRadius){
			return true;
		}
		return false;
	}
}
public class MapController : MonoBehaviour {
	public static int[][] m_Directions = new int[][]{
		new int[]{1,0},
		new int[]{1,-1},
		new int[]{0,-1},
		new int[]{-1,0}, 
		new int[]{-1,1}, 
		new int[]{0,1}
	};
	public int m_Width;
	public int m_Height;
	public int[] m_Blanks;
	public float m_TurnDelay = 2f;
	public GameObject[] m_TilePrefabs;
	public GameObject m_BossPrefab;
	public GameObject m_CharacterPrefab;
	public CharacterManager[] m_Characters;
	public BossManager m_Boss;
	//src: http://www.redblobgames.com/grids/hexagons/
	private Dictionary<string, TileManager> m_Map;
	private Vector3 m_XOffset = new Vector3(1.5f, 0f, 0.866f);
	private Vector3 m_YOffset = new Vector3(0f, 0f, 1.732f);
	private TileManager m_SelectedTileManager = null;
	private CharacterManager m_SelectedCharacter = null;
	private List<WaveObject> m_Waves;
	private WaitForSeconds m_BetweenTurnsWait;
	// Use this for initialization
	void Start () {
		m_Waves = new List<WaveObject>();
		m_BetweenTurnsWait = new WaitForSeconds(m_TurnDelay);
		GenerateMap();
		SpawnCharacters();

		StartCoroutine(GameLoop());
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			ResolveClick();
		}
	}
	public void BossLanded(int x, int y){
		CharacterManager[] nearbyCharacters = GetPlayersInRadius(x, y, 1);
		foreach(CharacterManager character in nearbyCharacters){
			if(character.m_X == x && character.m_Y == y){
				character.TakeDamage(10000);
			}
			else{
				character.TakeDamage(20);
				int[] knockbackPosition = new int[2]{character.m_X + (character.m_X - x), character.m_Y + (character.m_Y - y)};
				//character.MoveTo(knockbackPosition[0], knockbackPosition[1]);
				if(!TileExists(knockbackPosition[0], knockbackPosition[1])){
					character.TakeDamage(10000);
				}
			}
		}
		GenerateWave(x, y);
	}
	private void DisableAllControls(){
		foreach(CharacterManager c in m_Characters){
			c.SetEnabled(false);
		}
		foreach(TileManager t in m_Map.Values){
			if(t != null){
				t.SetEnabled(false);
			}
		}
		m_SelectedCharacter = null;
		m_SelectedTileManager = null;
	}
	private void DisableTileControls(){
		foreach(TileManager t in m_Map.Values){
			if(t != null){
				t.SetEnabled(false);
			}
		}
	}
	private void EnablePlayerControls(){
		foreach(CharacterManager c in m_Characters){
			c.SetEnabled(true);
		}
	}
	public bool TryAddTile(int x, int y){
		string positionString = GeneratePositionString(x, y);
		bool retVal = false;
		if(m_Map.ContainsKey(positionString)){
			if(m_Map[positionString] == null){
				retVal = true;
				m_Map[positionString] = CreateTileManager(x, y);
			}
		}
		else{
			retVal = true;
			m_Map.Add(positionString, CreateTileManager(x, y));
		}
		return retVal;
	}
	public int HexDistance(int x1, int y1, int x2, int y2){
		return (Mathf.Abs(x1 - x2)
			+ Mathf.Abs(x1 + y1 - x2 - y2)
			+ Mathf.Abs(y1 - y2)) / 2;
	}
	private void ResolveClick(){
		bool clickResolved = false;
		foreach(CharacterManager character in m_Characters){
			if(character.Selected && !character.m_IsDone && m_SelectedCharacter == null){
				m_SelectedCharacter = character;
				EnableTraverseControls(character.m_X, character.m_Y, character.m_Spd);
				EnableAttackableControls(character.m_X, character.m_Y, character.m_Spd, character.m_AtkRng);
				clickResolved = true;
			}
		}
		Dictionary<string, TileManager>.ValueCollection valueColl =
			m_Map.Values;
		foreach(TileManager t in valueColl){
			//Another tile clicked
			if(t != null && t.IsSelected() && m_SelectedTileManager == null){
				//if previously selected value is not equal to new selected value
				if(m_SelectedTileManager != null && m_SelectedTileManager != t){
					m_SelectedTileManager.SetSelected(false);
				}
				clickResolved = true;
				m_SelectedTileManager = t;

				CharacterInteract();
			}
		}
		//Either clicked selected tile or outside of board
		if(!clickResolved){
			ResetSelection();
		}
	}
	private void ResetSelection(){
		if(m_SelectedCharacter != null){
			m_SelectedCharacter.Selected = false;
			m_SelectedCharacter = null;
		}
		m_SelectedTileManager = null;
		DisableTileControls();
	}
	private void CharacterInteract(){
		if(m_SelectedTileManager.GetHighlight() == HighlightType.Traverseable){
			m_SelectedCharacter.MoveTo(m_SelectedTileManager);
			DisableTileControls();
		}
		else if(m_SelectedTileManager.GetHighlight() == HighlightType.Attackable){
			TileManager tile = null;
			for (int i = m_SelectedCharacter.m_AtkRng; i >= 1; i--){
				int[][] tileRing = GetRing(m_SelectedTileManager.m_X, m_SelectedTileManager.m_Y, m_SelectedCharacter.m_AtkRng);
				foreach(int[] position in tileRing){
					if(TileExists(position[0], position[1])){
						tile = m_Map[GeneratePositionString(m_SelectedTileManager.m_X, m_SelectedTileManager.m_Y)];
						if(tile.GetHighlight() == HighlightType.Traverseable){
							break;
						}
					}
				}
				//Tile should never by null here
				if(tile != null){
					m_SelectedCharacter.MoveTo(tile);
					//Currently only 1 boss
					m_Boss.TakeDamage(CalculateDamage(m_SelectedCharacter.m_X, m_SelectedCharacter.m_Y, m_Boss.m_X, m_Boss.m_Y, m_SelectedCharacter.m_Atk));
					break;
				}
			}
		}
		DisableTileControls();
		m_SelectedCharacter.m_IsDone = true;
	}
	private void GenerateMap(){
		m_Map = new Dictionary<string, TileManager>();
		//fill in the blanks with null
		for(int i = 0; i < m_Blanks.Length; i+=2){
			m_Map.Add(GeneratePositionString(m_Blanks[i], m_Blanks[i+1]), null);
		}
		string positionString = "";
		for(int i = 0; i < m_Width; i++){
			for(int j = 0; j < m_Height; j++){
				positionString = GeneratePositionString(i, j);
				if(!m_Map.ContainsKey(positionString)){
					m_Map.Add(positionString,
						CreateTileManager(i, j));
				}
			}
		}
	}
	private void SpawnCharacters(){
		foreach (CharacterManager character in m_Characters){
			character.m_Instance = Instantiate(m_CharacterPrefab, 
				MapToWorldPosition(character.m_X, character.m_Y),
				Quaternion.identity) as GameObject;
			string charPositionString = GeneratePositionString(character.m_X, character.m_Y);
			float charHeight = m_Map[charPositionString].TileHeight();
			character.TileHeightChanged(charHeight);
			character.m_MapController = this;
			character.Setup();
		}
		m_Boss.m_Instance = Instantiate(m_BossPrefab, 
			MapToWorldPosition(m_Boss.m_X, m_Boss.m_Y),
			Quaternion.identity) as GameObject;
		string bossPositionString = GeneratePositionString(m_Boss.m_X, m_Boss.m_Y);
		float bossHeight = m_Map[bossPositionString].TileHeight();
		m_Boss.TileHeightChanged(bossHeight);
		m_Boss.m_MapController = this;
		m_Boss.Setup();


	}
	private TileManager CreateTileManager(int x, int y){
		TileManager newTileManager = new TileManager();
		newTileManager.m_X = x;
		newTileManager.m_Y = y;
		newTileManager.m_Instance = Instantiate(m_TilePrefabs[(int)(Random.value * m_TilePrefabs.Length)], 
			MapToWorldPosition(x, y), 
			Quaternion.identity) as GameObject;
		newTileManager.Setup();
		return newTileManager;
	}
	public Vector3 MapToWorldPosition(int x, int y){
		return ((x - (m_Width / 2)) * m_XOffset) + ((y - (m_Height / 2)) * m_YOffset);
	}
	private string GeneratePositionString(int x, int y){
		return (x + "," + y);
	}
	public void GenerateWave(int x, int y){
		string positionString = GeneratePositionString(x, y);
		if(m_Map.ContainsKey(positionString) && m_Map[positionString] != null){
			m_Map[positionString].SetHeightStep(HeightStep.Trough);
			m_Map[positionString].BeginCycle();

			WaveObject wave = new WaveObject();
			wave.m_X = x;
			wave.m_Y = y;
			m_Waves.Add(wave);
		}
	}
	public int CalculateDamage(int atkX, int atkY, int tarX, int tarY, int baseDamage){
		//First check if these tiles even exist
		if(TileExists(atkX, atkY) && TileExists(tarX, tarY)){
			float scaleFactor = 1 + (m_Map[GeneratePositionString(atkX, atkY)].TileHeight() - m_Map[GeneratePositionString(tarX, tarY)].TileHeight());
			return (int)(scaleFactor * baseDamage);
		}
		//ALSO SHOULD NEVER HAPPEN
		return 0;
	}
	public HeightStep GetHeighSteptOfTile(int x, int y){
		if(TileExists(x, y)){
			return m_Map[GeneratePositionString(x, y)].GetHeightStep();
		}
		//THIS SHOULD NEVER HAPPEN. I AM SHORTSIGHTED
		return HeightStep.Middle;
	}
	public CharacterManager[] GetPlayersInRadius(int x, int y, int radius){
		List<CharacterManager> searchResults = new List<CharacterManager>();
		foreach(CharacterManager character in m_Characters){
			if(GetDistance(x, y, character.m_X, character.m_Y) <= radius){
				searchResults.Add(character);
			}
		}
		return searchResults.ToArray();
	}
	private bool TileExists(int x, int y){
		return m_Map.ContainsKey(GeneratePositionString(x, y)) && m_Map[GeneratePositionString(x, y)] != null;
	}
	private void UpdateWaves(){
		for(int i = m_Waves.Count - 1; i >= 0; i--){
			int[][] ringPositionList = 
				GetRing(m_Waves[i].m_X,
					m_Waves[i].m_Y,
					m_Waves[i].m_Radius);
			foreach(int[] position in ringPositionList){
				string positionString = GeneratePositionString(position[0], position[1]);
				if(m_Map.ContainsKey(positionString) && m_Map[positionString] != null){
					m_Map[positionString].BeginCycle();
				}
			}
			if(m_Waves[i].NextStep() || ringPositionList.Length == 0){
				m_Waves.RemoveAt(i);
			}
		}
	}
	private void UpdateWaveCycle(){
		foreach(TileManager m in m_Map.Values){
			if(m != null){
				m.NextCycleStep();
			}
		}
		foreach(CharacterManager character in m_Characters){
			character.TileHeightChanged(m_Map[GeneratePositionString(character.m_X, character.m_Y)].TileHeight());
			m_Boss.TileHeightChanged(m_Map[GeneratePositionString(m_Boss.m_X, m_Boss.m_Y)].TileHeight());
		}
	}

	private int[] GetDirection(DirectionEnum direction){
		int[] directionVector = m_Directions[(int)direction];
		return new int[2]{directionVector[0], directionVector[1]};
		
	}
	private int[] ScaleDirection(DirectionEnum direction, int magnitude){
		int[] outPosition = GetDirection(direction);
		outPosition[0] *= magnitude;
		outPosition[1] *= magnitude;
		return outPosition;
	}
	private int[][] GetRing(int x, int y, int radius){
		List<int[]> positions = new List<int[]>();
		int[] firstPosition = AddHexPosition(new int[2]{x, y}, ScaleDirection(DirectionEnum.DownLeft, radius));
		for(int i = 0; i < 6; i++){
			for(int j = 0; j < radius; j++){
				positions.Add(firstPosition);
				firstPosition = GetNeighbor(firstPosition, (DirectionEnum)i);
			}
		}
		return positions.ToArray();
	}
	private int[] GetNeighbor(int[] position, DirectionEnum direction){
		return AddHexPosition(position, GetDirection(direction));
		
	}
	private int[] AddHexPosition(int[] position1, int[] position2){
		int[] outInt = new int[2];
		outInt[0] = position1[0] + position2[0];
		outInt[1] = position1[1] + position2[1];
		return outInt;
	}
	private IEnumerator GameLoop(){
		yield return StartCoroutine(PlayerTurn());
		if(IsGameOver()){
			yield return StartCoroutine(GameOver());
			Application.LoadLevel(Application.loadedLevel);
		}
		yield return StartCoroutine(BossTurn());
		if(IsGameOver()){
			yield return StartCoroutine(GameOver());
			Application.LoadLevel(Application.loadedLevel);
		}
		else{
			UpdateWaves();
			UpdateWaveCycle();
			yield return StartCoroutine(GameLoop());
		}
	}
	private IEnumerator PlayerTurn(){
		EnablePlayerControls();
		while(!PlayersDone()){
			yield return null;
		}
		DisableAllControls();
		yield return m_BetweenTurnsWait;
	}
	//Should definitely separate enemy logic and player management but no time
	private IEnumerator BossTurn(){
		m_Boss.NextMove();
		yield return m_BetweenTurnsWait;
	}
	private IEnumerator GameOver(){
		//Game Over Message here
		yield return m_BetweenTurnsWait;
	}
	private bool IsGameOver(){
		bool bossDead = !m_Boss.m_Alive;
		bool playersDead = true;
		foreach(CharacterManager c in m_Characters){
			playersDead = playersDead && !c.m_Alive;
		}
		return (bossDead || playersDead);
	}
	private bool PlayersDone(){
		bool playersDone = true;
		foreach(CharacterManager c in m_Characters){
			playersDone = playersDone && c.m_IsDone;
		}
		return playersDone;
	}
	private void EnableTraverseControls(int x, int y, int moveDistance){
		for(int i = 1; i <= moveDistance; i++){
			int [][] traversableRing = GetRing(x, y, i);
			for(int j = 0; j < traversableRing.Length; j++){
				if(TileExists(traversableRing[j][0], traversableRing[j][1])){
					TileManager tile = m_Map[GeneratePositionString(traversableRing[j][0], traversableRing[j][1])];
					tile.SetEnabled(true);
					tile.Highlight(HighlightType.Traverseable);
				}
			}
		}
		foreach (CharacterManager c in m_Characters){
			TileManager tile = m_Map[GeneratePositionString(c.m_X, c.m_Y)];
			if(tile.GetHighlight() == HighlightType.Traverseable){
				tile.Highlight(HighlightType.None);
				tile.SetEnabled(false);
			}
		}
	}
	private void EnableAttackableControls(int x, int y, int moveDistance, int attackRange){
		for(int i = moveDistance + 1; i <= moveDistance + attackRange; i++){
			int[][] attackableRing = GetRing(x, y, i);
			for(int j = 0; j < attackableRing.Length; j++){
				if(TileExists(attackableRing[j][0], attackableRing[j][1])){
					TileManager tile = m_Map[GeneratePositionString(attackableRing[j][0], attackableRing[j][1])];
					tile.SetEnabled(true);
					tile.Highlight(HighlightType.Attackable);
				}
			}
		}
		if( m_Map[GeneratePositionString(m_Boss.m_X, m_Boss.m_Y)].GetHighlight() == HighlightType.Traverseable){
			m_Map[GeneratePositionString(m_Boss.m_X, m_Boss.m_Y)].Highlight(HighlightType.Attackable);
		}
	}
	private int GetDistance(int x1, int y1, int x2, int y2){
		int[] firstCube = AxialToCube(x1, y1);
		int[] secondCube = AxialToCube(x2, y2);
		return (Mathf.Abs(firstCube[0] - secondCube[0]) 
		+ Mathf.Abs(firstCube[1] - secondCube[1]) 
		+ Mathf.Abs(firstCube[2] - secondCube[2])) / 2;
	}
	//Only used for distance calculations. Do not use int[3] for other operations
	private int[] AxialToCube(int x, int y){
		return new int[3]{x, -x-y, y};
	}
}