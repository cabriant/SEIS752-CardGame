<?php
	include_once '../../helpers/ez-global.php';

	header('application/json');
	
	$user = getUserFromSession();
	$authArray = array('isAuthenticated' => !empty($user), 'user' => $user);
	$response = json_encode($authArray);

	echo $response;
?>