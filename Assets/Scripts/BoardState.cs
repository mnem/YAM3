using UnityEngine;
using System.Collections;

public class BoardState : MonoBehaviour {
	
	public GameObject template;
	public Vector2 cellCount;

	private Cell[,] _cells;
	
	private Cell _selected;
	
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
					existingSelection.swapVertical(cell);
				} else if (aY == bY && (xDiff == -1 || xDiff == 1)) {
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

	/// <summary>
	/// Cell. Contains all the information about a cell
	/// </summary>/
	private class Cell {
		public int x;
		public int y;
		public GameObject item;
		
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
		
		private void swapState(Cell other) {
			BlockState.Flavour tempFlavour = other.blockState.flavour;
			BlockState.Colour tempColour = other.blockState.colour;
			
			other.blockState.flavour = blockState.flavour;			
			other.blockState.colour = blockState.colour;	
			
			blockState.flavour = tempFlavour;
			blockState.colour = tempColour;
		}

		public void swapVertical(Cell cell) {
			swapState(cell);
		}

		public void swapHorizontal(Cell cell) {
			swapState(cell);
		}
		
		private BlockState blockState {
			get {
				return item.GetComponent<BlockState>();
			}
		}
	}
}
