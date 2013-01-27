using UnityEngine;
using System.Collections;

public class SelectCellOnClick : MonoBehaviour {

	private BoardState _boardState;
	
	// Use this for initialization
	void Start () {
		GameObject core = GameObject.Find("GameCore");
		_boardState = core.GetComponent<BoardState>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
			for (int i = 0; i < hits.Length; ++i) {
				if (gameObject == hits[i].collider.gameObject) {
					ActionOnClick();
					break;
				}
			}
		}
	}
	
	protected void ActionOnClick() {
		_boardState.selectCell(gameObject);
	}
	
}
