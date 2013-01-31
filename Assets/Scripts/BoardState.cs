using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
public class BoardState : MonoBehaviour {
	
	public GameObject template;
	public Vector2 cellCount;

	private Cell[,] _cells;
	
	private Cell _selected;
	
	private bool _swapping = false;
	private bool _cancellingSwap = false;
	
	// Use this for initialization
	void Start () {
		InitialiseCellStore();
		
		Vector3 origin = new Vector3(-4f, -3, 0);
		Vector3 current = new Vector3(origin.x, origin.y, origin.z);
		float displayWidth = template.renderer.bounds.size.x;
		float displayHeight = template.renderer.bounds.size.y;
		const float xGap = 1/8f;
		
		for(int y = 0; y < cellCount.y; ++y) {
			for(int x = 0; x < cellCount.x; ++x) {
				GameObject display = (GameObject)Instantiate(template, current, Quaternion.identity);
				Cell cell = new Cell() {item = display, x = x, y = y};
				_cells[x,y] = cell;
				current.x += displayWidth + xGap;
			}
			current.y += displayHeight;
			current.x = origin.x;
		}
	}
	
	void Reset() {
		cellCount.Set(8, 8);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void CellClicked(GameObject cellGameObject) {
		if (_swapping || _cancellingSwap) return;
		
		Cell existingSelection = _selected;
		Cell cell = GetCell(cellGameObject);
			
		if (_selected != null) {
			_selected.StopSelectedAnimation();	
			_selected = null;
		}
		
		// If we aren't de-selecting the current cell...
		if (existingSelection != cell) {
			if (existingSelection == null) {
				// No previous selection
				_selected = cell;
				_selected.PlaySelectedAnimation();
			} else {
				// Check if they're close enough to swap
				int aX = existingSelection.x;
				int aY = existingSelection.y;
				int bX = cell.x;
				int bY = cell.y;
				int xDiff = aX - bX;
				int yDiff = aY - bY;
				
				if (aX == bX && (yDiff == -1 || yDiff == 1)) {
					_swapping = true;
					existingSelection.swapVertical(cell);
				} else if (aY == bY && (xDiff == -1 || xDiff == 1)) {
					_swapping = true;
					existingSelection.swapHorizontal(cell);
				} else {
					// Too far, just select the new cell
					_selected = cell;
					_selected.PlaySelectedAnimation();
				}
			}
		}
	}
	
	private Cell GetCell(GameObject cell) {
		for(int y = 0; y < cellCount.y; ++y) {
			for(int x = 0; x < cellCount.x; ++x) {
				if (_cells[x,y].item == cell) {
					return _cells[x,y];
				}
			}
		}
		
		return null;
	}
	
	private void InitialiseCellStore() {
		_cells = new Cell[(int)cellCount.x, (int)cellCount.y];
	}

	public void swapTweenFinished(Cell[] cells) {
		Cell a = cells[0];
		Cell b = cells[1];
		
		a.swapState(b);
		a.resetRotationAndAnchor();
		b.resetRotationAndAnchor();
		
		// If it was a cancelling move, we stop
		// after swapping back
		if (_cancellingSwap) {
			_cancellingSwap = false;
			return;
		}
		
		List<Cell> aMatches = new List<Cell>();
		WalkMatches(a, aMatches);
		
		// If a and b match, getting a's matches will
		// already include b and co.
		List<Cell> bMatches = new List<Cell>();
		if (!a.blockState.Match(b.blockState)) {
			//WalkMatches(b, bMatches);
		}
		
		float scoreA = CalculateScore(aMatches);
		float scoreB = CalculateScore(bMatches);
		
		if (scoreA == 0 && scoreB == 0) {
			// Spin them back
			_cancellingSwap = true;
			if (a.x == b.x) {
				a.unswapVertical(b);
			} else {
				a.unswapHorizontal(b);
			}
		} else {
			int topY = (int)(cellCount.y - 1);
			Debug.Log("topY: " + topY);
			if (scoreA > 0) {
				foreach (Cell dead in aMatches) {
					for (int i = dead.y; i < topY; ++i) {
						_cells[dead.x, i] = _cells[dead.x, i+1];
						_cells[dead.x, i].y = i;
					}
					Destroy(dead.item);
					//dead.y = topY;
					//Vector3 temp = dead.item.transform.position;
					//temp.y = 0.0f;
					//dead.item.transform.position = temp;
				}
			}
			/*
			if (scoreB > 0) {
				foreach (Cell dead in aMatches) {
					for (int i = dead.y + 1; i < topY; ++i) {
						_cells[dead.x, i - 1] = _cells[dead.x, i];
					}
					Destroy(dead.item);
					//dead.y = topY;
					//Vector3 temp = dead.item.transform.position;
					//temp.y = 0.0f;
					//dead.item.transform.position = temp;
				}
			}
			*/
		}
		
		_swapping = false;
	}

	public float CalculateScore (List<Cell> matches)
	{
		if (matches == null || matches.Count < 3) {
			return 0f;
		} else {
			// TODO: Implement bonus for colour and flavour matches
			return matches.Count * 100f;
		}
	}

	public void WalkMatches(Cell a, List<Cell> matches)
	{
		if (a == null) {
			return;
		} else if (matches.Contains(a)) {
			// Already processed
			return;
		} else if (matches.Count == 0 || matches[0].blockState.Match(a.blockState)) {
			// Baddabing
			matches.Add(a);
		} else {
			// No a match
			return;
		}
		
		if (a.y > 0) WalkMatches(_cells[a.x, a.y - 1], matches);
		if (a.y < (cellCount.y - 1)) WalkMatches(_cells[a.x, a.y + 1], matches);
		if (a.x > 0) WalkMatches(_cells[a.x - 1, a.y], matches);
		if (a.x < (cellCount.x - 1)) WalkMatches(_cells[a.x + 1, a.y], matches);
	}
		
	/// <summary>
	/// Cell. Contains all the information about a cell
	/// </summary>/
	public class Cell {
		private int _x;
		private int _y;
		private GameObject _item;
		
		public GameObject item {
			get {
				return this._item;
			}
			set {
				_item = value;
				UpdateName();
			}
		}

		public int x {
			get {
				return this._x;
			}
			set {
				_x = value;
				UpdateName();
			}
		}

		public int y {
			get {
				return this._y;
			}
			set {
				_y = value;
				UpdateName();
			}
		}		
		public void PlaySelectedAnimation() {
			iTween.Stop(blockState.visualContainer);
			iTween.ScaleTo(blockState.visualContainer, iTween.Hash(
				"x", 1.4f,
				"y", 0.6f,
				"z", 1.4f,
				"time", 0.25f,
				"easetype", iTween.EaseType.easeInOutQuad,
				"looptype", iTween.LoopType.pingPong
			));
		}

		public void UpdateName()
		{
			if (_item != null) {
				_item.name = _x + ", " + y;
			}
		}
		
		public void StopSelectedAnimation() {
			iTween.Stop(blockState.visualContainer);
			iTween.ScaleTo(blockState.visualContainer, iTween.Hash(
				"x", 1.0f,
				"y", 1.0f,
				"z", 1.0f,
				"time", 0.15f,
				"easetype", iTween.EaseType.easeOutQuad
			));
		}
		
		public void swapState(Cell other) {
			blockState.Swap(other.blockState);
		}
		
		private Hashtable rotateVerticalHash(int direction) {
			return iTween.Hash(
				"x", 180.0f / 360.0f * direction,
				"y", 0.0f,
				"z", 0.0f,
				"time", 0.5f,
				"easetype", iTween.EaseType.easeOutQuad
			);
		}
		
		private Hashtable rotateHorizontalHash(int direction) {
			return iTween.Hash(
				"x", 0.0f,
				"y", 180.0f / 360.0f * direction,
				"z", 0.0f,
				"time", 0.5f,
				"easetype", iTween.EaseType.easeOutQuad
			);
		}

		private void swapVertical(Cell cell, int direction) {
			if (cell.y > y) {
				cell.blockState.visualPivotPoint = BlockState.VisualPivotPoint.BottomCentre;
				blockState.visualPivotPoint = BlockState.VisualPivotPoint.TopCentre;
			} else {
				cell.blockState.visualPivotPoint = BlockState.VisualPivotPoint.TopCentre;
				blockState.visualPivotPoint = BlockState.VisualPivotPoint.BottomCentre;
				direction *= -1;
			}
			
			iTween.RotateBy(blockState.visualContainer, rotateVerticalHash(direction));
			iTween.RotateBy(blockState.visual, rotateVerticalHash(-direction));
			
			Hashtable rotateAndCallback = rotateVerticalHash(direction);
			Cell[] cells = {this, cell};
			rotateAndCallback.Add("oncomplete", "swapTweenFinished");
			rotateAndCallback.Add("oncompletetarget", GameObject.Find("GameCore"));
			rotateAndCallback.Add("oncompleteparams", cells);
			iTween.RotateBy(cell.blockState.visualContainer, rotateAndCallback);
			iTween.RotateBy(cell.blockState.visual, rotateVerticalHash(-direction));
		}
		
		public void swapVertical(Cell cell) {
			swapVertical(cell, 1);
		}
		
		public void unswapVertical(Cell cell) {
			swapVertical(cell, -1);
		}
		
		public void swapHorizontal(Cell cell, int direction) {
			if (cell.x > x) {
				cell.blockState.visualPivotPoint = BlockState.VisualPivotPoint.LeftCentre;
				blockState.visualPivotPoint = BlockState.VisualPivotPoint.RightCentre;
				direction *= -1;
			} else {
				cell.blockState.visualPivotPoint = BlockState.VisualPivotPoint.RightCentre;
				blockState.visualPivotPoint = BlockState.VisualPivotPoint.LeftCentre;
			}
			
			iTween.RotateBy(blockState.visualContainer, rotateHorizontalHash(direction));
			iTween.RotateBy(blockState.visual, rotateHorizontalHash(-direction));
			
			Hashtable rotateAndCallback = rotateHorizontalHash(direction);
			Cell[] cells = {this, cell};
			rotateAndCallback.Add("oncomplete", "swapTweenFinished");
			rotateAndCallback.Add("oncompletetarget", GameObject.Find("GameCore"));
			rotateAndCallback.Add("oncompleteparams", cells);
			iTween.RotateBy(cell.blockState.visualContainer, rotateAndCallback);
			iTween.RotateBy(cell.blockState.visual, rotateHorizontalHash(-direction));
		}
		
		public void swapHorizontal(Cell cell) {
			swapHorizontal(cell, 1);
		}
		
		public void unswapHorizontal(Cell cell) {
			swapHorizontal(cell, -1);
		}

		public void resetRotationAndAnchor() {
			Vector3 unrotated = new Vector3(0.0f, 0.0f, 0.0f);
			blockState.visualContainer.transform.eulerAngles = unrotated;
			blockState.visual.transform.eulerAngles = unrotated;
			blockState.visualPivotPoint = BlockState.VisualPivotPoint.Centre;
		}

		
		public BlockState blockState {
			get {
				return item.GetComponent<BlockState>();
			}
		}
	}
}
