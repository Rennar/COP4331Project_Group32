// Blake Loeb

private var formNick = ""; //Username
private var formPassword = ""; //Password
var formText = ""; 
 
var URL = "localhost/networktest1/check_scores.php"; 
var hash = "hashcode";
var returnvalue = false;
var stringret = "20";
var intret = -1;
var isWaiting = false;

// Return Functions - used for C# integration
// =====================================================
function Login(user, pass)
{
	JLogin(hash, user, pass);
	return returnvalue;	
}

function Register(user, pass)
{
	JRegister(hash, user, pass);
	return returnvalue;	
}
function UpdateData(user, pass, type, data)
{
	JUpdateData(hash, user, pass, type, data);
	return returnvalue;
}
function GetData(user, pass, type)
{
	JGetData(hash, user, pass, type);
}

function getWaiting(){
	return isWaiting;
}
function getReturnValue(){
	return returnvalue;
}
function getIntValue(){
	intret = parseInt(stringret);
	return intret;
}
function getStringValue(){
	return stringret;
}
// =====================================================

// Validates Username/Password
// Sets returnvalue to true if valid 
function JLogin(hash : String, formNick : String, formPassword : String) {


    var form = new WWWForm();
    form.AddField( "myform_funct", "login" );
    form.AddField( "myform_hash", hash );
    form.AddField( "myform_nick", formNick );
    form.AddField( "myform_pass", formPassword );
    var w = WWW(URL, form);
    isWaiting = true;
    yield w;
  	isWaiting = false;
    if (w.error == null && w.text.IndexOf("success") > -1)
    {
    	returnvalue = true;
   		print("Login worked");
    	return;
    }
   	print("Login Failed");
    formNick = "";
    formPassword = "";
    returnvalue = false;
}

// Adds a username/password combination to the database
// Sets returnvalue to true if successful
function JRegister(hash : String, formNick : String, formPassword : String) {

    var form = new WWWForm(); 
    form.AddField( "myform_funct", "register" );
    form.AddField( "myform_hash", hash );
    form.AddField( "myform_nick", formNick );
    form.AddField( "myform_pass", formPassword );
    var w = WWW(URL, form); 
    isWaiting = true;
    yield w;
    isWaiting = false;
    if (w.error == null && w.text.IndexOf("success") > -1)
   {
   		returnvalue = true;
   		print("Registration worked");
   		JLogin(hash, formNick, formPassword);
    	return;
    }
    print("Regristration failed");
    formNick = ""; 
    formPassword = "";
    returnvalue = false;
}

// Updates the data for a specific user
// Sets returnvalue to true if successful
function JUpdateData(hash : String, formNick : String, formPassword : String, type : String, data : String) {
	 print("Updating " + type + " to " + data);

    var form = new WWWForm(); 
    form.AddField( "myform_funct", "update" );
    form.AddField( "myform_hash", hash ); 
    form.AddField( "myform_nick", formNick );
    form.AddField( "myform_type", type );
    form.AddField( "myform_data", data );
   
    var w = WWW(URL, form);
    isWaiting = true;
    yield w;
    isWaiting = false;
    if (w.error == null && w.text.IndexOf("success") > -1)
    {
	    	returnvalue = true;
	    	print("Updated data without error");
	    	return;
    }

    returnvalue = false;
    return;
}

//Gets data from the database
//Sets stringret to a string reflecting the gotten data
function JGetData(hash : String, formNick : String, formPassword : String, type : String) {

    var form = new WWWForm(); 
    form.AddField( "myform_funct", "get" );
    form.AddField( "myform_hash", hash ); 
    form.AddField( "myform_nick", formNick );
    form.AddField( "myform_type", type );
    var w = WWW(URL, form); 
    isWaiting = true;
    yield w;
    isWaiting = false;
    stringret = w.text;
    print(stringret);
    return;
}