
<?php
// =============================================================================
// Connections
error_reporting(E_ALL ^ E_DEPRECATED);
$con = mysql_connect("sql207.byethost33.com", "b33_19536195", "tmbjbj32") or die("Can't login");
mysql_select_db("b33_19536195_scores")or die("Cant get in database");
// =============================================================================
// Security, MD5 and SQL injection prevention
function sec_pass($sql)
{
    $sql = strip_tags(trim(preg_replace("/(from|select|insert|delete|where|drop table|show tables|,|'|#|\*|--|\\\\)/i","",$sql)));
    if(!get_magic_quotes_gpc())
        $sql = addslashes($sql);
    $sql = md5(trim($sql));
    return $sql;
}
// =============================================================================
// Security, SQL injection prevention
function sec_log($sql)
{
    $sql = strip_tags(trim(preg_replace("/(from|select|insert|delete|where|drop table|show tables|,|'|#|\*|--|\\\\)/i","",$sql)));
    if(!get_magic_quotes_gpc())
        $sql = addslashes($sql);
    return $sql;
}
// =============================================================================
// Checks to see if username and password match. Echos "success" if successful.
function login()
{
    $unityHash = sec_log($_POST["myform_hash"]);
    $phpHash = "hashcode";
    $username = sec_log($_POST["myform_nick"]);
    $password = sec_pass($_POST["myform_pass"]);


    if(($username && $password) && ($unityHash == $phpHash))
    {
        $SQL = "SELECT * FROM scores WHERE name = '" . $username . "'";
        $result_id = @mysql_query($SQL) or die("Failure");
        $total = mysql_num_rows($result_id);
        if(($total) && (!strcmp($password, @mysql_fetch_array($result_id)["password"]))) echo "success";
    }
}

// =============================================================================
// Registers a new user. Echos "success" if successful.
function regi($con)
{
    $username = sec_log($_POST["myform_nick"]);
    $password = sec_log($_POST["myform_pass"]);
    $SQL = "INSERT INTO `scores` ( `id` , `name` , `password`, `shop`, `highscore1`, `highscore2`, `achievement`, `experience`) VALUES (NULL , '" . $username . "', MD5( '" . $password . "' ), 0, 0, 0, 0, 0);";
    if (mysql_query($SQL, $con)) echo "success";
}
// =============================================================================
// Gets datafield and echos it
function getdata()
{
    $unityHash = sec_log($_POST["myform_hash"]);
    $phpHash = "hashcode";
    $type =$_POST["myform_type"];

    if(($type == "scoreboard1") or ($type == "scoreboard2")){
        getscores();
        return;
    }

    $username = sec_log($_POST["myform_nick"]);
    $SQL = "SELECT * FROM scores WHERE name = '" . sec_log($_POST["myform_nick"]) . "'";
    $result_id = @mysql_query($SQL) or die("DATABASE ERROR!");
    $total = mysql_num_rows($result_id);
    $datas = @mysql_fetch_array($result_id);
    echo $datas[$type];
}
// =============================================================================
// Updates datafield. Echos "success" if successful.
function updatedata($con)
{
    $unityHash = sec_log($_POST["myform_hash"]);
    $phpHash = "hashcode";
    $type =$_POST["myform_type"];
    $data =$_POST["myform_data"];

    $username = sec_log($_POST["myform_nick"]);

    $SQL = "SELECT * FROM scores WHERE name = '" . $username . "'";
    $result_id = @mysql_query($SQL) or die("DATABASE ERROR!");
    $total = mysql_num_rows($result_id);
    $datas = @mysql_fetch_array($result_id);


    $SQL = "UPDATE `scores` SET " . $type . " = '" . $data . "' WHERE name = '" . $username . "'";

    if (mysql_query($SQL, $con)) {echo "success";

    if($type == "highscore1" or $type == "highscore2") updatescores($con);

}
// =============================================================================
// Updates the scoreboard. Echos "success" if successful.
function updatescores($con)
{
    //Get data fields
    $data =$_POST["myform_data"];
    $username = sec_log($_POST["myform_nick"]);
    $type =$_POST["myform_type"];

    //Put current high scores into array
    $SQL = "SELECT * FROM high WHERE name = '" . $type . "'";
    $result_id = @mysql_query($SQL) or die("DATABASE ERROR!");
    $total = mysql_num_rows($result_id);
    $datas = @mysql_fetch_array($result_id);

    //Make arrays for dealing with stuff
    $topscore = array("score1", "score2", "score3", "score4", "score5", "score6", "score7", "score8", "score9", "score10");
    $topname = array("name1", "name2", "name3", "name4", "name5", "name6", "name7", "name8", "name9", "name10");
    $tops = array(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    $topn = array("", "", "", "", "", "", "", "", "", "");


    //Fill the tops and topn array with the current high scores
    for ($x=0; $x < 10; $x++) {
        $tops[$x] = $datas[$topscore[$x]];
        $topn[$x] = $datas[$topname[$x]];
    }


    //Put the users score in the right place
    for ($x=0; $x < 10; $x++) {             // For 1 through 10
        if($data > $tops[$x]){              // If the user's score is greater than a score at point x
            for ($y=9; $y > x; $y--) {     // For all scores at a place lower then point x
                $tops[$y+1] = $tops[$y];    // score x+1 = score x (score x is moved down)
                $topn[$y+1] = $topn[$y];    // Move the name to match the score
            }
            $tops[$x] = $data;              // Put the player's score at the x point
            $topn[$x] = $username;              // Put the player's name at the x point
            break;
        }
    }

    //Track Success
    $success = true;


    //Update all of the stuff
    for ($x=0; $x < 10; $x++)
    {
        $SQL = "UPDATE `high` SET " . $topscore[$x] . " = '" . $tops[$x] . "'WHERE name = '" . $type . "'";
        if (!mysql_query($SQL, $con)){echo "This Error: " . $SQL . $con->error; $success = false;}

        $SQL = "UPDATE `high` SET " . $topname[$x] . " = '" . $topn[$x] . "'WHERE name = '" . $type . "'";
        if (!mysql_query($SQL, $con)){echo "This Error: " . $SQL . $con->error; $success = false;}
    }

    //Report success
    if($success) echo "success";

}
// =============================================================================
// Echos the scoreboard.
function getscores()
{
    //Get data fields
    $type =$_POST["myform_type"];

    //Select which scoreboard
    if ($type == "scoreboard1") $type = "highscore1";
    if ($type == "scoreboard2") $type = "highscore2";

    //Put current high scores into array
    $SQL = "SELECT * FROM high WHERE name = '" . $type . "'";
    $result_id = @mysql_query($SQL) or die("DATABASE ERROR!");
    $total = mysql_num_rows($result_id);
    $datas = @mysql_fetch_array($result_id);

    //Make arrays for dealing with stuff
    $topscore = array("score1", "score2", "score3", "score4", "score5", "score6", "score7", "score8", "score9", "score10");
    $topname = array("name1", "name2", "name3", "name4", "name5", "name6", "name7", "name8", "name9", "name10");


    //Fill the tops and topn array with the current high scores
    for ($x=0; $x < 10; $x++) {
        echo $datas[$topscore[$x]];
        echo " ";
        echo $datas[$topname[$x]];
        echo " ";
    }

}
// =============================================================================
// Determine what function to run

$funct = $_POST["myform_funct"];

if($funct == "register") regi($con);
else if($funct == "login") login();
else if($funct == "update") updatedata($con);
else if($funct == "get") getdata();

mysql_close();
?>
