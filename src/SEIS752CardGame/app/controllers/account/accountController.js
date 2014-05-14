(function () {

    var AccountController = function ($scope, $location, $routeParams, accountService) {
    	$scope.email = null;
    	$scope.password = null;
	    $scope.confirmPassword = null;
    	$scope.displayName = null;
    	$scope.phoneNumber = null;
    	$scope.validationErrors = [];
    	$scope.code = null;
	    $scope.token = null;

    	$scope.codeValid = false;
    	$scope.codeSent = false;
	    $scope.passwordReset = false;

	    $scope.create = function() {
		    var result = accountService.createUser($scope.email, $scope.password, $scope.confirmPassword, $scope.displayName, $scope.phoneNumber);
			result.then(function (data) {
	                if (data.success) {
		                accountService.redirectToLogin();
	                } else {
		                $scope.validationErrors = data.errors;
	                }
                });
	    };

	    $scope.cancel = function() {
	    	accountService.cancelAction();
	    };

	    $scope.forgot = function() {
	    	var result = accountService.submitForgot($scope.email);
	    	result.then(function (data) {
	    		$scope.codeSent = data.success;
	    		$scope.validationErrors = data.errors;

				if (data.success) {
					$scope.code = null;
					$scope.validationErrors = [];
				}
		    });
	    };

	    $scope.verifyCode = function() {
	    	var result = accountService.verifyCode($scope.email, $scope.code);
	    	result.then(function (data) {
	    		$scope.codeValid = data.success;
	    		$scope.token = data.token;
			    $scope.validationErrors = data.errors;

				if (data.success) {
					$scope.code = null;
					$scope.password = null;
					$scope.confirmPassword = null;
					$scope.validationErrors = [];
				}
		    });
	    };

	    $scope.resetPassword = function() {
	    	var result = accountService.resetPassword($scope.email, $scope.token, $scope.password, $scope.confirmPassword);
	    	result.then(function (data) {
	    		$scope.passwordReset = data.success;
			    $scope.validationErrors = data.errors;

				if (data.success) {
					$scope.email = null;
					$scope.token = null;
					$scope.password = null;
					$scope.confirmPassword = null;
					$scope.validationErrors = [];
				}
		    });
	    };

	    $scope.loginWithNewPassword = function() {
		    accountService.cancelAction();
	    };

	    $scope.viewProfile = function() {
	    	var modalInstance = $modal.open({
				templateUrl: '/app/views/blackjack/_create.html',
				controller: BlackjackModalController
			});

			modalInstance.result.then(function (tableInfo) {
				var result = blackjackService.createTable(tableInfo.name, tableInfo.ante, tableInfo.maxRaise, tableInfo.maxPlayers);
				result.then(function (data) {
					if (data.success) {
						$scope.tables = data.tables;
					}
				});
			});
	    };
    };

    AccountController.$inject = ['$scope', '$location', '$routeParams', 'accountService'];

    angular.module(appModule).controller('AccountController', AccountController);

}());