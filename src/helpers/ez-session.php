<?php
	function storeInSession($key, $value) {
		if (!empty($key) && !empty($value)) {
			$_SESSION[$key] = $value;
		}
	}

	function getFromSession($key, $persist) {
		if (!empty($_SESSION[$key])) {
			$value = $_SESSION[$key];
	   		
	   		// erase it from session if we don't want it to persist
			if (!$persist) {
				unset($_SESSION[$key]);
			}
			return $value;
		}
	}
?>