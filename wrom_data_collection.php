<?php

$cors_whitelist = [
    'https://v6p9d9t4.ssl.hwcdn.net',
    'https://itch.io/',
    'https://itch.zone/',
];
  
if (in_array($_SERVER['HTTP_ORIGIN'], $cors_whitelist)) 
{
    header('Access-Control-Allow-Origin: ' . $_SERVER['HTTP_ORIGIN']);
}

define("DBHOST","localhost");
define("DBUSER","patternl_wrom_player");
define("DBPASS","gag22oki");
define("DBNAME","patternl_WRoM_DataLogging");

$connection = new mysqli(DBHOST,DBUSER,DBPASS, DBNAME);

if (isset($_POST['post_data']))
{	
	$timestamp = mysqli_escape_string($connection, $_POST['timestamp']);
    $timestamp = filter_var($timestamp, FILTER_SANITIZE_STRING, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
    
	$user_id = mysqli_escape_string($connection, $_POST['user_id']);
    $user_id = filter_var($user_id, FILTER_SANITIZE_STRING, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	
	$user_ip_address = mysqli_escape_string($connection, $_POST['user_ip_address']);
    $user_ip_address = filter_var($user_ip_address, FILTER_SANITIZE_STRING, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	
	$version_num = mysqli_escape_string($connection, $_POST['version_num']);
	$version_num = filter_var($version_num, FILTER_SANITIZE_STRING, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	
	$scene_name = mysqli_escape_string($connection, $_POST['scene_name']);
    $scene_name = filter_var($scene_name, FILTER_SANITIZE_STRING, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	
	$data_name = mysqli_escape_string($connection, $_POST['data_name']);
    $data_name = filter_var($data_name, FILTER_SANITIZE_STRING, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	
	$data_description = mysqli_escape_string($connection, $_POST['data_description']);
    $data_description = filter_var($data_description, FILTER_SANITIZE_STRING, FILTER_FLAG_STRIP_LOW | FILTER_FLAG_STRIP_HIGH);
	
	$value = $_POST['value'];
	 
	$statement = $connection->prepare("INSERT INTO wrom_playtest (timestamp, user_id, user_ip_address, version_num, scene_name, data_name, data_description, value) VALUES (?, ?, ?, ?, ?, ?, ?, ?)");
    $statement->bind_param("ssssssss", $timestamp, $user_id, $user_ip_address, $version_num, $scene_name, $data_name, $data_description, $value);
	
    $statement->execute();
    $statement->close();
}

$connection->close();

?>