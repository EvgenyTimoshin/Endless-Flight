using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;

public class FacebookScript : MonoBehaviour {

	public Text FriendsText; 

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
	#endregion

	public void FacebookShare()
	{
		FB.ShareLink (new System.Uri ("http://resocoder.com"));
	}

	#region Inviting
	public void FacebookGameRequest()
	{
		FB.AppRequest ("Hey, come and play this awesome game!", title: "Endless Flight");

	}

	public void FacebookInvite()
	{
		FB.Mobile.AppInvite(new System.Uri(""));
	}


	#endregion

	public void GetFriends()
	{
		string query =  "/me/friends";
		FB.API (query, HttpMethod.GET, result => {
			var dictionary = Facebook.MiniJSON.Json.Deserialize (result.RawResult);
			var friendsList = (List<object>)dictionary ["data"];
			FriendsText.text = string.Empty;

			foreach(var dict in friendsList)
				FriendsText.text += ((Dictionary<string, object>)dict)["name"];
		});


}
