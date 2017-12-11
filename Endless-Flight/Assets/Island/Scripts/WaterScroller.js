//THIS SCRIPT 'SCROLLS' THE GAMEOBJECT'S MAIN TEXTURE (Tileable texture required)
var scrollSpeed = 0.1;

function Update () 
{
	if(GetComponent.<Renderer>().material.shader.isSupported)
		Camera.main.depthTextureMode |= DepthTextureMode.Depth;
	 
    var offset = Time.time * scrollSpeed;
    //Texture scrolling is instanced separately | Best if your scene contains multiple water planes of different speeds
	//GetComponent.<Renderer>().material.SetTextureOffset ("_MainTex", Vector2(offset/10.0, offset));
    
    //Share texture scrolling among objects with the same material | Best if your scene contains a single water plane, or multiple water of the same speed
    GetComponent.<Renderer>().sharedMaterial.SetTextureOffset ("_MainTex", Vector2(offset/10.0, offset));
}