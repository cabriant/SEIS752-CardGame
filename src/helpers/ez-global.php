<?php
	include_once 'ez-session.php';

	function checkUserIsLoggedIn() {
		$user = getUserFromSession();
		return !empty($user);
	}

	function getUserFromSession() {
		$user = getFromSession('userProfile', true);
		return $user;
	}

	function setUserForSession($user) {
		storeInSession('userProfile', $user);
	}

	function logUserOff() {
		// remove user from session by retrieving and not persisting the value
		$user = getFromSession('userProfile', false);
		header('location: login.php');
		exit();
	}
?>