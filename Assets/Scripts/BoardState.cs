using UnityEngine;
using System.Collections;

public class BoardState : MonoBehaviour {

	Vector2 cellCount;

	private Cell[,] cells;
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	void Reset() {
		cellCount.Set(8, 8);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void initialiseCells() {
		cells = new Cell[(int)cellCount.x, (int)cellCount.y];
	}

	/// <summary>
	/// Cell. Contains all the information about a cell
	/// </summary>/
	private class Cell {
		public enum Type {
			Null,
			Cube,
			Cone,
			Sphere
		};
		
		public enum Flavour {
			Null,
			Red,
			Green,
			Blue
		};
		
		public Type type = Type.Null;
		public Flavour flavour = Flavour.Null;
	}
}
