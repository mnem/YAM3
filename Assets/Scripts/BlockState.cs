using UnityEngine;
using System.Collections;

public class BlockState : MonoBehaviour {
	public enum Flavour {
		Null,
		Cone,
		Cube,
		Sphere
	}

	public enum Colour {
		Null,
		Blue,
		Red,
		Yellow
	}
	
	public enum VisualPivotPoint {
		Centre,
		LeftCentre,
		RightCentre,
		TopCentre,
		BottomCentre,
		ForceRefresh // Special value which just cause anchor recalculation
	}
	
	public Colour colour { get; set; }
	public Flavour flavour { get; set; }
	public VisualPivotPoint visualPivotPoint  { get; set; }
	
	// These fields store the actual state. This is used
	// to switch to the requested state on the next update
	// as necessary.
	private Colour _currentColour;
	private Flavour _currentFlavour;
	private VisualPivotPoint _currentVisualPivotPoint;
	
	
	// Use this for initialization
	void Start () {
		GameObject core = GameObject.Find("GameCore");
		_map = core.GetComponent<BlockTypeVisualMap>();
		
		_currentVisualPivotPoint = VisualPivotPoint.Centre;
		
		RandomFlavour();
		RandomColour();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		UpdateVisualMesh();
		UpdatePivotPoint();
		UpdateVisualColour();
	}

	public void RandomColour() {
		switch(Random.Range(1, 4)) {
		case 1:
			colour = Colour.Blue;
			break;
		case 2:
			colour = Colour.Red;
			break;
		case 3:
			colour = Colour.Yellow;
			break;
		}
	}

	public void RandomFlavour() {
		switch(Random.Range(1, 4)) {
		case 1:
			flavour = Flavour.Cone;
			break;
		case 2:
			flavour = Flavour.Cube;
			break;
		case 3:
			flavour = Flavour.Sphere;
			break;
		}
	}

	void UpdatePivotPoint ()
	{
		if (visualPivotPoint == _currentVisualPivotPoint) return;
		
		if (visualPivotPoint == VisualPivotPoint.ForceRefresh) {
			// Restore the real pivot. This is a little
			// race condition-y but will probably work ok
			visualPivotPoint = _currentVisualPivotPoint;
		}
		
		Vector3 pivot = new Vector3(0f, 0f, 0f);
		
		switch (visualPivotPoint) {
		case VisualPivotPoint.Centre:
			// By default it's centered, 0, 0, 0
			break;
		case VisualPivotPoint.LeftCentre:
			pivot.x = -renderer.bounds.extents.x - (1/16f); // Hack in the half the gap of the board
			break;
		case VisualPivotPoint.RightCentre:
			pivot.x = renderer.bounds.extents.x + (1/16f); // Hack in the half the gap of the board
			break;
		case VisualPivotPoint.TopCentre:
			pivot.y = renderer.bounds.extents.y;
			break;
		case VisualPivotPoint.BottomCentre:
			pivot.y = -renderer.bounds.extents.y;
			break;
		}
		
		_visualContainer.transform.localPosition = pivot;

		// Offset the visuals so they are back in the
		// centre of the cell
		pivot.x *= -1;
		pivot.y *= -1;
		pivot.z *= -1;
		_visual.transform.localPosition = pivot;
		
		_currentVisualPivotPoint = visualPivotPoint;
	}
	
	private void UpdateVisualColour() {
		if (_currentColour == colour) return;
		
		Renderer renderer = _item.GetComponentInChildren<Renderer>();
		
		switch (colour) {
		case Colour.Blue:
			renderer.material = _map.colourBlue;
			break;
		case Colour.Red:
			renderer.material = _map.colourRed;
			break;
		case Colour.Yellow:
			renderer.material = _map.colourYellow;
			break;
		}
		
		_currentColour = colour;
	}
	
	private void UpdateVisualMesh() {
		if (_currentFlavour == flavour) return;
		
		if (_visualContainer == null) {
			// Create the container for the visuals
			_visualContainer = new GameObject();
			_visualContainer.transform.parent = transform;
			_visualContainer.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		
		if (_visual == null) {
			// Create the container for the visuals
			_visual = new GameObject();
			_visual.transform.parent = _visualContainer.transform;
			_visual.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		
		if (_item) Destroy(_item);
		
		switch (flavour) {
		case Flavour.Cone:
			_item = (GameObject)Instantiate(_map.flavourCone);
			break;
		case Flavour.Cube:
			_item = (GameObject)Instantiate(_map.flavourCube);
			break;
		case Flavour.Sphere:
			_item = (GameObject)Instantiate(_map.flavourSphere);
			break;
		}

		_item.transform.parent = _visual.transform;
		_item.transform.localPosition = new Vector3(0f, 0f, 0f);
		
		visualPivotPoint = VisualPivotPoint.ForceRefresh;
		_currentColour = Colour.Null;
		
		_currentFlavour = flavour;
	}

	public void Swap(BlockState other)
	{
		Flavour oldFlavour = flavour;
		flavour = other.flavour;
		other.flavour = oldFlavour;
		
		Colour oldColour = colour;
		colour = other.colour;
		other.colour = oldColour;
	}
	
	public bool Match(BlockState other) {
		return flavour == other.flavour;
	}
	
	public bool ExactMatch(BlockState other) {
		return flavour == other.flavour && colour == other.colour;
	}

	public GameObject visualContainer {
		get {
			return _visualContainer;
		}
	}	

	public GameObject visual {
		get {
			return this._visual;
		}
	}

	private GameObject _visualContainer;
	private GameObject _visual;
	private GameObject _item;
	private BlockTypeVisualMap _map;
}
