using UnityEngine;
using System.Collections;

public class BlockState : MonoBehaviour {
	public enum Flavour {
		Cone,
		Cube,
		Sphere
	}

	public enum Colour {
		Blue,
		Red,
		Yellow
	}
	
	public Colour colour {
		get { return _colour; }
		set {
			if (value != _colour) {
				_colour = value;
				UpdateVisualColour();
			}	
		}
	}
	
	public Flavour flavour {
		get { return _flavour; }
		set {
			if (value != _flavour) {
				_flavour = value;
				UpdateVisualMesh();
			}	
		}
	}
	
	// Use this for initialization
	void Start () {
		GameObject core = GameObject.Find("GameCore");
		_map = core.GetComponent<BlockTypeVisualMap>();
		
		UpdateVisualMesh();	
	}
	
	// Update is called once per frame
	void Update () {

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
	}
	
	private void UpdateVisualMesh() {
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
		
		_visual.transform.parent = transform;
		
		if (_flavour == Flavour.Cone) {
			_visual.transform.localPosition = new Vector3(0f, -0.45f, 0f);
		} else {
			_visual.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		
		UpdateVisualColour();
	}
	
	private Colour _colour;
	private Flavour _flavour;
	private GameObject _visual;
	private BlockTypeVisualMap _map;
}
