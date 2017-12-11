#pragma strict
//THIS SCRIPT IS USED TO KEEP THE TILLING OF WATER PLANES EQUAL RELATIVE TO SCALE
var tileScale : float = 0.1f;

function Start () {
	//INSTANCED TILING
	//Material tiling is instanced separately | Best if your scene contains multiple water planes of different scales
	//GetComponent.<Renderer>().material.mainTextureScale = new Vector2(transform.localScale.x * tileScale, transform.localScale.y * tileScale);
	
	//SHARED TILING
	//Share tiling among objects with the same material | Best if your scene contains a single water plane, or multiple water planes of the same scale
	GetComponent.<Renderer>().sharedMaterial.mainTextureScale = new Vector2(transform.localScale.x * tileScale, transform.localScale.y * tileScale);
}