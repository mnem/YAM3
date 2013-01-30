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
	void Update () {
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
		
		Renderer renderer = _visual.renderer;
		if (renderer == null) {
			renderer = _visual.GetComponentInChildren<Renderer>();
		}
		
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
		
		GameObject old = _visual;
		_visual = null;
		
		switch (flavour) {
		case Flavour.Cone:
			_visual = (GameObject)Instantiate(_map.flavourCone);
			break;
		case Flavour.Cube:
			_visual = (GameObject)Instantiate(_map.flavourCube);
			break;
		case Flavour.Sphere:
			_visual = (GameObject)Instantiate(_map.flavourSphere);
			break;
		}
		
		_visual.transform.parent = _visualContainer.transform;
		if (old != null) {
			_visual.transform.localScale = old.transform.localScale;
		}
		
		if (old != null) {
			Destroy(old);
		}
		
		visualPivotPoint = VisualPivotPoint.ForceRefresh;
		_currentColour = Colour.Null;
		
		_currentFlavour = flavour;
	}

	public void swap(BlockState other)
	{
		GameObject oldVisual = _visual;
		Colour oldColour = colour;
		Flavour oldFlavour = flavour;
		
		_visual = other._visual;
		_visual.transform.parent = _visualContainer.transform;
		_currentColour = colour = other.colour;
		_currentFlavour = flavour = other.flavour;
		
		other._visual = oldVisual;
		other._visual.transform.parent = other._visualContainer.transform;
		other._currentColour = other.colour = oldColour;
		other._currentFlavour = other.flavour = oldFlavour;
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
	private BlockTypeVisualMap _map;
}
