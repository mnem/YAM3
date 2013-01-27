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
	
	private Colour _currentColour;
	public Colour colour {
		get { return _colour; }
		set {
			if (value != _colour) {
				_colour = value;
			}	
		}
	}
	
	private Flavour _currentFlavour = Flavour.Null;
	public Flavour flavour {
		get { return _flavour; }
		set {
			if (value != _flavour) {
				_flavour = value;
			}	
		}
	}
	
	// Use this for initialization
	void Start () {
		GameObject core = GameObject.Find("GameCore");
		_map = core.GetComponent<BlockTypeVisualMap>();
		
		_currentColour = _colour = Colour.Null;
		_currentFlavour = _flavour = Flavour.Null;
		
		RandomFlavour();
		RandomColour();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateVisualMesh();
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
	
	private void UpdateVisualColour() {
		if (_currentColour == _colour) return;
		
		switch (_colour) {
		case Colour.Blue:
			_visual.renderer.material = _map.colourBlue;
			break;
		case Colour.Red:
			_visual.renderer.material = _map.colourRed;
			break;
		case Colour.Yellow:
			_visual.renderer.material = _map.colourYellow;
			break;
		default:
			Debug.Log("Unrecognised colour! " + _colour);
			break;
		}
		
		_currentColour = _colour;
	}
	
	private void UpdateVisualMesh() {
		if (_currentFlavour == _flavour) return;
		
		if (_visualContainer == null) {
			// Create the container for the visuals
			_visualContainer = new GameObject();
			_visualContainer.transform.parent = transform;
			_visualContainer.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		
		if (_visual != null) {
			Destroy(_visual);
			_visual = null;
		}
		
		switch (_flavour) {
		case Flavour.Cone:
			_visual = (GameObject)Instantiate(_map.flavourCone);
			break;
		case Flavour.Cube:
			_visual = (GameObject)Instantiate(_map.flavourCube);
			break;
		case Flavour.Sphere:
			_visual = (GameObject)Instantiate(_map.flavourSphere);
			break;
		default:
			Debug.Log("Unrecognised flavour! " + _flavour);
			break;
		}
		
		_visual.transform.parent = _visualContainer.transform;
		
		if (_flavour == Flavour.Cone) {
			_visual.transform.localPosition = new Vector3(0f, -0.45f, 0f);
		} else {
			_visual.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		
		_currentFlavour = _flavour;
		
		UpdateVisualColour();
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

	private Colour _colour;
	private Flavour _flavour;
	private GameObject _visualContainer;
	private GameObject _visual;
	private BlockTypeVisualMap _map;
}
