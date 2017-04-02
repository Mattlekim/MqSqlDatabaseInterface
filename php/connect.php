<?php
if (!isset($_SESSION)) session_start();

if (isset($_SESSION['user'])) //make suer we have a user
{	
	if (time() - $_SESSION['lastActivity'] >  600) //if longer than 10 minutes
	{
		//destroy session
		session_unset();
		session_destroy();
		die("#error-=#login required"); //kill paGE
	}
	else
		$_SESSION['lastActivity'] = time(); //update time last loged in
}
else
{
	die("#error-=#login required"); //KILL PAGE
}


//database logins
$powerUserName = "";
$powerUserPassword = "";

$normalUserName = "";
$normalUserName = "";

//set to normal user
$dbUserName = $normalUserName;
$dbPassword = $normalUserPassword;

if ($_SESSION['PowerUser'] == true) //if we are a poweruser
{
	$dbUserName = $powerUserName;
	$dbPassword = $powerUserPassword;
}

//if all is well connect to database
mysql_connect("localhost", $dbUserName, $dbPassword) or die("did not connect");

mysql_select_db("enter database name") or die(mysql_error());
?>
