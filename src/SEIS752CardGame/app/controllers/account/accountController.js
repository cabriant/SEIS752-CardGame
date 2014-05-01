(function () {

    var AccountController = function ($scope, $location, $routeParams, accountService) {
    	$scope.email = null;
    	$scope.password = null;
	    $scope.confirmPassword = null;
    	$scope.displayName = null;
	    $scope.phoneNumber = null;

	    $scope.create = function() {
		    accountService.createUser($scope.email, $scope.password, $scope.confirmPassword, $scope.displayName, $scope.phoneNumber);
	    };

	    $scope.cancel = function() {
	    	accountService.cancelCreate();
	    };
    };

    AccountController.$inject = ['$scope', '$location', '$routeParams', 'accountService'];

    angular.module(appModule).controller('AccountController', AccountController);

}());