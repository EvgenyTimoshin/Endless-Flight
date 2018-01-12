using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;

public class FacebookScript : MonoBehaviour {

	private void Awake()
	{
		if (!FB.IsInitialized) {
			FB.Init (() => {
				if (FB.IsInitialized)
					FB.ActivateApp ();
				else
					Debug.LogError ("Couldn't initialise");
			},

				isGameShown => {
					if (!isGameShown)
						Time.timeScale = 0;
					else
						Time.timeScale = 1;
				});
					
		} else
			FB.ActivateApp (); 
	}

	//Login Method
	#region Login / Logout
	public void FacebookLogin() 
	{
		var permissions = new List<string> () { "public_profile", "email", "user_friends" };
		FB.LogInWithReadPermissions (permissions);

	}

	public void FacebookLogout()
	{
		FB.LogOut ();
	}
}
