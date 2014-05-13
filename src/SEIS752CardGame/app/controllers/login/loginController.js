(function () {

    var LoginController = function ($scope, $window, $location, $routeParams, userService) {
        var path = '/';
        $scope.email = null;
        $scope.password = null;
        $scope.errorMessage = null;
        $scope.isEmailValid = true;

        $scope.login = function () {
            userService.login($scope.email, $scope.password).then(function (response) {
                //$routeParams.redirect will have the route
                //they were trying to go to initially
            	if (!response.authenticated) {
            		$scope.errorMessage = response.error;
                    return;
                }

            	if (response.authenticated && $routeParams && $routeParams.redirect) {
                    path = path + $routeParams.redirect;
                }

                $location.path(path);
            });
        };

		$scope.googleOauth = function() {
			if ($routeParams && $routeParams.redirect)
				$window.location.href = '/oauth/login/google?redirect=' + $routeParams.redirect;
			else
				$window.location.href = '/oauth/login/google';
		}
    };

    LoginController.$inject = ['$scope', '$window', '$location', '$routeParams', 'userService'];

    angular.module(appModule)
        .controller('LoginController', LoginController);

}());