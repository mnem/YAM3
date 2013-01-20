using UnityEngine;
using System.Collections;

public class BoardState : MonoBehaviour {
	
	public GameObject template;
	public Vector2 cellCount;

	private Cell[,] cells;
	
	
	// Use this for initialization
	void Start () {
		initialiseCells();
		
		Vector3 current = new Vector3(-5, 0, -1);
		float displayWidth = template.renderer.bounds.size.x;
		float displayHeight = template.renderer.bounds.size.y;
		const float xGap = 0.1f;
		
		for(int y = 0; y < cellCount.y; ++y) {
			for(int x = 0; x < cellCount.x; ++x) {
				GameObject display = (GameObject)Instantiate(template, current, Quaternion.identity);
				Cell cell = new Cell() {item = display};
				cells[x,y] = cell;
				current.x += displayWidth + xGap;
			}
			current.y += displayHeight;
			current.x = -5;
		}
	}
	
	void Reset() {
		cellCount.Set(8, 8);
	}
	
	// Update is called once per frame
	void Update () {
		Cell foo = cells[0,0];
		if (foo.item.rigidbody.IsSleeping()) {
			Debug.Log("Snore");
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
