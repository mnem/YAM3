using UnityEngine;
using System.Collections;

public class BoardState : MonoBehaviour {
	
	public GameObject template;
	public Vector2 cellCount;

	private Cell[,] cells;
	private bool randomised;
	
	// Use this for initialization
	void Start () {
		randomised = false;
		initialiseCells();
		
		Vector3 origin = new Vector3(-4f, -3, 0);
		Vector3 current = new Vector3(origin.x, origin.y, origin.z);
		float displayWidth = template.renderer.bounds.size.x;
		float displayHeight = template.renderer.bounds.size.y;
		const float xGap = 1/8f;
		
		for(int y = 0; y < cellCount.y; ++y) {
			for(int x = 0; x < cellCount.x; ++x) {
				GameObject display = (GameObject)Instantiate(template, current, Quaternion.identity);
				Cell cell = new Cell() {item = display};
				cells[x,y] = cell;
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
		if (!randomised) {
			randomised = true;
			for(int y = 0; y < cellCount.y; ++y) {
				for(int x = 0; x < cellCount.x; ++x) {
					Cell cell = cells[x,y];
					BlockState state = cell.item.GetComponent<BlockState>();
					state.RandomFlavour();
					state.RandomColour();
				}
			}
		}
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
		public GameObject item;
	}
}
