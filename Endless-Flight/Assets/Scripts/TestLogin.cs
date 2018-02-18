using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity; 
using System.Collections.Generic;
using UnityEngine.UI;

public class TestLogin : MonoBehaviour {

	public GameObject LoggedInUI;
	public GameObject NotLoggedInUI;
	public GameObject Friend;

	void Awake() {
		if (!FB.IsInitialized) {
			FB.Init (InitCallBack);
		}


	}

	void InitCallBack() { 
		Debug.Log ("FB has been initialised");
		ShowUI (); 
	}

	public void Login() {

		if (!FB.IsLoggedIn) {
			FB.LogInWithReadPermissions (new List<string> { "user_friends" }, LoginCallBack);
		}
	}

	void LoginCallBack(ILoginResult result) {
		if(result.Error == null) {
			Debug.Log ("FB logged in");
			ShowUI ();
		}
		else {
			Debug.Log ("Error during login: " + result.Error);
		}
	}

	void ShowUI() {
		Debug.Log ("Start of show UI");
		if(FB.IsLoggedIn) {
			LoggedInUI.SetActive(true);
			NotLoggedInUI.SetActive (false);
			FB.API ("me/picture?width=100&height=100", HttpMethod.GET, PictureCallBack);
			FB.API ("me?fields=first_name", HttpMethod.GET, NameCallBack);
			Debug.Log ("before friend call back");
			FB.API ("me/friends",  HttpMethod.GET, FriendCallBack);
		}
		else {
			LoggedInUI.SetActive(false);
			NotLoggedInUI.SetActive (true);
		}
	}

	void PictureCallBack(IGraphResult result) {
		Texture2D image = result.Texture;
		LoggedInUI.transform.Find ("ProfilePicture").GetComponent<Image> ().sprite = Sprite.Create
			(image, new Rect (0, 0, 100, 100), new Vector2 (0.5f, 0.5f)); 	
	}

	void NameCallBack(IGraphResult result) {
		IDictionary<string, object> profile = result.ResultDictionary;
		LoggedInUI.transform.Find ("Name").GetComponent<Text>().text = "Hello " + profile["first_name"];	
	
	}

	public void LogOut() {
		FB.LogOut();
		ShowUI ();
	}


	//Link to Playstore, FB etc
	public void Share() {
		FB.ShareLink (new System.Uri("http://"), "This game is awesome!", "Description", new System.Uri("http://"));
		
	}

	public void Invite() {
		FB.AppRequest (message: "You should really play this game", title: "Check this game!");
	}

	void FriendCallBack(IGraphResult result) {
		Debug.Log ("FriendCallBack");
		IDictionary<string, object> data = result.ResultDictionary;
		List<object> friends = (List<object>)data["data"];
		Debug.Log (friends.Count);
		foreach (object obj in friends) {
			Debug.Log ("Entered friend loop");
			Dictionary<string, object> dicto = (Dictionary<string, object>)obj;
			CreateFriend(dicto ["name"].ToString (), dicto ["id"].ToString());
		}
		
	}

	void CreateFriend(string name, string id) {

		Debug.Log ("Friend yoke");
		GameObject myFriend = Instantiate (Friend);
		Transform parent = LoggedInUI.transform.Find ("ListContainer").Find("FriendsList");
		myFriend.transform.SetParent (parent);
		Debug.Log ("Logged in " + name);
		myFriend.GetComponentInChildren<Text>().text = name;
		FB.API (id + "/picture?width=100&height=100", HttpMethod.GET, delegate(IGraphResult result) {
			myFriend.GetComponentInChildren<Image>().sprite = Sprite.Create(result.Texture, new Rect (0, 0, 100, 100), new Vector2 (0.5f, 0.5f));
		});

		
	}


			 

}
