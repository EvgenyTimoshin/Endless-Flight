#pragma strict
var speed : float = 7; //player's movement speed
var gravity : float = 10; //amount of gravitational force applied to the player
private var controller : CharacterController; //player's CharacterController component
private var moveDirection : Vector3 = Vector3.zero;

function Start () {
	controller = transform.GetComponent(CharacterController);
}

function Update () {
	//APPLY GRAVITY
	if(moveDirection.y > gravity * -1) {
		moveDirection.y -= gravity * Time.deltaTime;
	}
	controller.Move(moveDirection * Time.deltaTime);
	var left = transform.TransformDirection(Vector3.left);
	
	if(controller.isGrounded) {
		if(Input.GetKeyDown(KeyCode.Space)) {
			moveDirection.y = speed;
		}
		else if(Input.GetKey("w")) {
			if(Input.GetKey(KeyCode.LeftShift)) {
				controller.SimpleMove(transform.forward * speed * 2);
			}
			else {
				controller.SimpleMove(transform.forward * speed);
			}
		}
		else if(Input.GetKey("s")) {
			if(Input.GetKey(KeyCode.LeftShift)) {
				controller.SimpleMove(transform.forward * -speed * 2);
			}
			else {
				controller.SimpleMove(transform.forward * -speed);
			}
		}
		else if(Input.GetKey("a")) {
			if(Input.GetKey(KeyCode.LeftShift)) {
				controller.SimpleMove(left * speed * 2);
			}
			else {
				controller.SimpleMove(left * speed);
			}
		}
		else if(Input.GetKey("d")) {
			if(Input.GetKey(KeyCode.LeftShift)) {
				controller.SimpleMove(left * -speed * 2);
			}
			else {
				controller.SimpleMove(left * -speed);
			}
		}
	}
	else {
		if(Input.GetKey("w")) {
			var relative : Vector3;
			relative = transform.TransformDirection(0,0,1);
			if(Input.GetKey(KeyCode.LeftShift)) {
				controller.Move(relative * Time.deltaTime * speed * 2);
			}
			else {
				controller.Move(relative * Time.deltaTime * speed);
			}
		}
	}
}