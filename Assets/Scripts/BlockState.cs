using UnityEngine;
using System.Collections;

public class BlockState : MonoBehaviour {
	
	public Colour colour;
	public Flavour flavour;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public enum Colour {
		Red,
		Yellow,
		Blue
	}
	
	public enum Flavour {
		Cube,
		Sphere,
		Cone
	}
}
