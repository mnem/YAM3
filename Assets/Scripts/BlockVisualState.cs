using UnityEngine;
using System.Collections;
using YAM3;

public class BlockVisualState : MonoBehaviour {
	public enum VisualPivotPoint {
		Centre,
		LeftCentre,
		RightCentre,
		TopCentre,
		BottomCentre,
		ForceRefresh // Special value which just cause anchor recalculation
	}
	
	public VisualPivotPoint visualPivotPoint  { get; set; }

	private BlockState _state = new BlockState();
	public BlockState state { get { return this._state; }  }
	
	// These fields store the actual state. This is used
	// to switch to the requested state on the next update
	// as necessary.
	private BlockState _currentState = new BlockState();
	private VisualPivotPoint _currentVisualPivotPoint;
	
	// Use this for initialization
	void Start () {
		GameObject core = GameObject.Find("GameCore");
		_map = core.GetComponent<BlockTypeVisualMap>();
		
		_currentVisualPivotPoint = VisualPivotPoint.Centre;
		_currentState.colour = BlockState.Colour.Null;
		_currentState.flavour = BlockState.Flavour.Null;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		UpdateVisualMesh();
		UpdatePivotPoint();
		UpdateVisualColour();
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
		if (_currentState.colour == state.colour) return;
		
		Renderer renderer = _item.GetComponentInChildren<Renderer>();
		
		switch (state.colour) {
		case BlockState.Colour.Blue:
			renderer.material = _map.colourBlue;
			break;
		case BlockState.Colour.Red:
			renderer.material = _map.colourRed;
			break;
		case BlockState.Colour.Yellow:
			renderer.material = _map.colourYellow;
			break;
		}
		
		_currentState.colour = state.colour;
	}
	
	private void UpdateVisualMesh() {
		if (_currentState.flavour == state.flavour) return;
		
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
		
		switch (state.flavour) {
		case BlockState.Flavour.Cone:
			_item = (GameObject)Instantiate(_map.flavourCone);
			break;
		case BlockState.Flavour.Cube:
			_item = (GameObject)Instantiate(_map.flavourCube);
			break;
		case BlockState.Flavour.Sphere:
			_item = (GameObject)Instantiate(_map.flavourSphere);
			break;
		case BlockState.Flavour.Ring:
			_item = (GameObject)Instantiate(_map.flavourRing);
			break;
		case BlockState.Flavour.Dodecahedron:
			_item = (GameObject)Instantiate(_map.flavourDodecahedron);
			break;
		}

		_item.transform.parent = _visual.transform;
		_item.transform.localPosition = new Vector3(0f, 0f, 0f);
		
		visualPivotPoint = VisualPivotPoint.ForceRefresh;
		_currentState.colour = BlockState.Colour.Null;
		
		_currentState.flavour = state.flavour;
	}

	public void Swap(BlockVisualState other)
	{
		state.Swap(other.state);
	}
	
	public bool Match(BlockVisualState other) {
		return state.flavour == other.state.flavour;
	}
	
	public bool ExactMatch(BlockVisualState other) {
		return state.flavour == other.state.flavour && state.colour == other.state.colour;
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
