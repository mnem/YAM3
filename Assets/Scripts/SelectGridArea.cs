using UnityEngine;
using System.Collections;

public class SelectGridArea : MonoBehaviour {
	public GameObject selector;
	
	public Vector3 gridOrigin;
	public Vector3 cellSize;
	public Vector3 cellCount;
	public string layerName;
	public bool onlyThisGameObject;
	public bool onlySelectOnMouseDown;

	public Vector3 cell {
		get { return _cell; }
	}
	public Vector3 cellPenetration {
		get { return _cellPenetration; }
	}
	public bool beingSelected  {
		get { return  _beingSelected; }
	}
	
	void Reset () {
		gridOrigin.Set(0.0f, 0.0f, 0.0f);
		
		cellSize.Set(100.0f, 100.0f, 100.0f);
		cellCount.Set(2.0f, 2.0f, -2.0f);
		_cell.Set(0.0f, 0.0f, 0.0f);
		_cellPenetration.Set(0.0f, 0.0f, 0.0f);
		
		_beingSelected = false;
		onlyThisGameObject = true;
		onlySelectOnMouseDown = true;
		
		layerName = "";
	}

	// Use this for initialization
	void Start () {		
	}

	// Update is called once per frame
	void Update () {
		_beingSelected = false;
		
		if (!onlySelectOnMouseDown || Input.GetMouseButton(0)) {
			CastAndSelectArea(Input.mousePosition);
		}
	}
	
	private void CastAndSelectArea(Vector3 position) {
		Ray ray = Camera.main.ScreenPointToRay(position);
		int layerMask = 0xffffff;
		int layerIndex = LayerMask.NameToLayer(layerName);
		if (layerIndex > 0)
		{
			layerMask = 1 << (layerIndex -1);
		}
		RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);
		for (int i = 0; i < hits.Length; ++i) {
			if (!onlyThisGameObject || gameObject == hits[i].collider.gameObject) {
				SelectArea(hits[i].point);
				// Only dealing with the first hit for the
				// sake of sanity
				break;
			}
		}
	}
	
	private void SelectArea(Vector3 point) {
		Vector3 gridPoint = point - gridOrigin;
			
		_cell.Set(
			Mathf.Clamp(Mathf.Floor(gridPoint.x/cellSize.x), 0, cellCount.x > 0 ? cellCount.x - 1 : 0), 
			Mathf.Clamp(Mathf.Floor(gridPoint.y/cellSize.y), 0, cellCount.x > 0 ? cellCount.x - 1 : 0), 
			Mathf.Clamp(Mathf.Floor(gridPoint.z/cellSize.z), 0, cellCount.x > 0 ? cellCount.x - 1 : 0));
		
		Vector3 cellOrigin = gridOrigin + new Vector3(
			cell.x * cellSize.x, 
			cell.y * cellSize.y,
			cell.z * cellSize.z);
		
		_cellPenetration.Set(
			Mathf.Clamp01((point.x - cellOrigin.x) / cellSize.x), 
			Mathf.Clamp01((point.y - cellOrigin.y) / cellSize.y),
			Mathf.Clamp01((point.z - cellOrigin.z) / cellSize.z));
		
		PositionSelectorAtCenterOfCell(cellOrigin);
		
		_beingSelected = true;
	}

	void PositionSelectorAtCenterOfCell(Vector3 cellOrigin)
	{
		if (selector && selector.transform) {
			selector.transform.position = new Vector3(
				cellOrigin.x + (cellSize.x / 2.0f),
				cellOrigin.y + (cellSize.y / 2.0f),
				cellOrigin.z + (cellSize.z / 2.0f));
			
			//Color c = selector.renderer.material.color;
			//c.r = _cellPenetration.x;
			//c.g = _cellPenetration.y;
			//c.b = _cellPenetration.z;
			//selector.renderer.material.color = c;
		}
	}
	
	private Vector3 _cell;
	private Vector3 _cellPenetration;
	private bool _beingSelected;
}
