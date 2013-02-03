using UnityEngine;
using System.Collections;

namespace YAM3
{
	public class BlockState
	{
		public enum Flavour {
			Null,
			Cone,
			Cube,
			Sphere,
			Ring,
			Dodecahedron
		}
	
		public enum Colour {
			Null,
			Blue,
			Red,
			Yellow
		}
		
		public Colour colour { get; set; }
		public Flavour flavour { get; set; }
		
		public BlockState ()
		{
			RandomColour();
			RandomFlavour();
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
			switch(Random.Range(1, 6)) {
			case 1:
				flavour = Flavour.Cone;
				break;
			case 2:
				flavour = Flavour.Cube;
				break;
			case 3:
				flavour = Flavour.Sphere;
				break;
			case 4:
				flavour = Flavour.Ring;
				break;
			case 5:
			default:
				flavour = Flavour.Dodecahedron;
				break;
			}
		}
		
		public void NextFlavour() {
			switch (flavour) {
			case Flavour.Cone:
				flavour = Flavour.Cube;
				break;
			case Flavour.Cube:
				flavour = Flavour.Sphere;
				break;
			case Flavour.Sphere:
				flavour = Flavour.Ring;
				break;
			case Flavour.Ring:
				flavour = Flavour.Dodecahedron;
				break;
			case BlockState.Flavour.Dodecahedron:
				flavour = Flavour.Cone;
				break;
			default:
				RandomFlavour();
				break;
			}
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
	}
}

