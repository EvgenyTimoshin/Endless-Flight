using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity; 
using System.Collections.Generic;
using UnityEngine.UI;

public class TestLogin : MonoBehaviour {

	void Awake() {
		if (!FB.IsInitialized) {
			FB.Init (InitCallBack);
		}
	}

	void InitCallBack() {
		Debug.Log ("FB has been initialised");
	}

	public void Login() {
		if (FB.IsLoggedIn) {
			FB.LoginWithReadPermissions (new List<string> { "user_friends" }, LoginCallBack);

		}
	}

	void LoginCallBack(ILoginResult result) {
		if(result.Error == null) {
			Debug.Log ("FB logged in");
		}
		else {
			Debug.Log ("Error during login: " + result.Error);
		}
	}

}
