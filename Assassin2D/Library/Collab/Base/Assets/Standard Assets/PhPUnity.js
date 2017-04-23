
private var formNick = ""; //this is the field where the player will put the name to login
private var formPassword = ""; //this is his password
var formText = ""; //this field is where the messages sent by PHP script will be in
 
var URL = "localhost/networktest1/check_scores.php"; //change for your URL
var hash = "hashcode"; //change your secret code, and remember to change into the PHP file too
var returnvalue = null;
var isWaiting = false;
var stringreturn = "";

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
function GetData(user, pass, type, data)
{
	JGetData(hash, user, pass, type, data);
	return stringreturn;
}
function getWaiting(){
	return isWaiting;
}
function getReturnValue(){
	return returnvalue;
}

function getStringValue(){
	return stringreturn;
}

function JLogin(hash, formNick, formPassword) {


    var form = new WWWForm(); //here you create a new form connection
    form.AddField( "myform_funct", "login" );
    form.AddField( "myform_hash", hash ); //add your hash code to the field myform_hash, check that this variable name is the same as in PHP file
    form.AddField( "myform_nick", formNick );
    form.AddField( "myform_pass", formPassword );
    var w = WWW(URL, form); //here we create a var called 'w' and we sync with our URL and the form
//    isWaiting = true;
    yield w; //we wait for the form to check the PHP file, so our game dont just hang
//  	isWaiting = false;
    if (w.error == null && w.text.IndexOf("success") > -1)
    {
    	returnvalue = true;
   		print("Login worked");
   		JUpdateData(hash, formNick, formPassword, "shop", "25");
    	return;
    }
   	print("Login Failed");
    formNick = ""; //just clean our variables
    formPassword = "";
    returnvalue = false;
}

function JRegister(hash, formNick, formPassword) {


    var form = new WWWForm(); //here you create a new form connection
    form.AddField( "myform_funct", "register" );
    form.AddField( "myform_hash", hash ); //add your hash code to the field myform_hash, check that this variable name is the same as in PHP file
    form.AddField( "myform_nick", formNick );
    form.AddField( "myform_pass", formPassword );
    var w = WWW(URL, form); //here we create a var called 'w' and we sync with our URL and the form
    isWaiting = true;
    yield w; //we wait for the form to check the PHP file, so our game dont just hang
//    isWaiting = false;
    if (w.error == null && w.text.IndexOf("success") > -1)
   {
   		returnvalue = true;
   		print("Registration worked");
      	JLogin(hash, formNick, formPassword);
      	isWaiting = false;
    	return;
    }
    print("Regristration failed");
    formNick = ""; //just clean our variables
    formPassword = "";
    returnvalue = false;
}

function JUpdateData(hash, formNick, formPassword, type, data) {

//	JLogin(hash, formNick, formPassword);
	if(!returnvalue) return;

    var form = new WWWForm(); //here you create a new form connection
    form.AddField( "myform_funct", "update" );
    form.AddField( "myform_hash", hash ); //add your hash code to the field myform_hash, check that this variable name is the same as in PHP file
    form.AddField( "myform_nick", formNick );
    form.AddField( "myform_type", type );
    form.AddField( "myform_data", data );
    var w = WWW(URL, form); //here we create a var called 'w' and we sync with our URL and the form
//    isWaiting = true;
    yield w; //we wait for the form to check the PHP file, so our game dont just hang
//    isWaiting = false;
    if (w.error == null && w.text.IndexOf("success") > -1)
    {
       	print("Update Data worked");
    	JGetData(hash, formNick, formPassword, type, data);
    	if (stringreturn.IndexOf(data) > -1)
    	{
	    	returnvalue = true;
	    	return;
    	}
    }
    print("String return is");
    print(stringreturn);
    print("data is");
    print(data);
    print(w.data);
    print(w.text);
    returnvalue = false;
}

function JGetData(hash, formNick, formPassword, type, data) {

//	JLogin(hash, formNick, formPassword);
	if(!returnvalue) return;

    var form = new WWWForm(); //here you create a new form connection
    form.AddField( "myform_funct", "get" );
    form.AddField( "myform_hash", hash ); //add your hash code to the field myform_hash, check that this variable name is the same as in PHP file
    form.AddField( "myform_nick", formNick );
    form.AddField( "myform_type", type );
    form.AddField( "myform_data", data );
    var w = WWW(URL, form); //here we create a var called 'w' and we sync with our URL and the form
//    isWaiting = true;
    yield w; //we wait for the form to check the PHP file, so our game dont just hang
    isWaiting = false;
    stringreturn = w.text;
    print("get data");
    print(stringreturn);
}