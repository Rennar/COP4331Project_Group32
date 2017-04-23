using UnityEngine;
using System.Collections;

public class phpunity : MonoBehaviour
{
	private string username = ""; //Should store username
	private string password = ""; //Stores password so we can check login with other functions

	string url = "localhost/networktest1/check_scores.php"; //URL for .php thing
	string hash = "hashcode"; //Hashcode, must match 

	Rect textrect = new Rect (10, 150, 500, 500); // dumb thing for GUI test. Remove for final version.
	string formText = ""; // dumb thing for GUI test. Remove for final version.
	public bool success = false;

	//Returns true if a user successfully logs in
	//Returns false if they didn't
	public IEnumerator Login (string user, string pass)
	{
		username = user;
		password = pass;
		WWWForm connect = new WWWForm();
		connect.AddField("myform_funct", "login" );
		connect.AddField("myform_hash", hash);
		connect.AddField("myform_nick", username);
		connect.AddField("myform_pass", password);
		WWW connection = new WWW(url, connect);
		yield return connection;
		if (connection.error == null && connection.text.Contains("success")) success = true;
		else 
		{
			success = false;
		}
		username = "";
		password = "";

	}

	//Returns true if a user successfully registers and logs in
	//returns false otherwise
	public IEnumerator Register (string user, string pass)
	{
		username = user;
		password = pass;
		WWWForm connect = new WWWForm();
		connect.AddField("myform_funct", "register" );
		connect.AddField("myform_hash", hash);
		connect.AddField("myform_nick", username);
		connect.AddField("myform_pass", password);
		WWW connection = new WWW(url, connect);
		yield return connection;
		if (connection.error == null && connection.text.Contains("success")) Login(user, pass);
		else success = false;
	}

}