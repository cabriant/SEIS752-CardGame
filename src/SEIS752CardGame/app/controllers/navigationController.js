(function () {

    var NavigationController = function ($scope, $location, $window, userService) {
        $scope.isCollapsed = false;
        $scope.appTitle = 'Card Game';

        $scope.hide = function () {
            return shouldHide();
        };

        $scope.logout = function () {
            userService.logout().then(function () {
                    $location.path(userService.loginPath);
                    return;
                });
            redirectToLogin();
        };

        function redirectToLogin() {
            var path = '/login' + $location.$$path;
            $window.location.href = path;
        }
		
		function redirectToRoot() {
			var path = "/";
			$window.location.href = path;
		}

		function shouldHide() {
			return ($location.path().indexOf("/login") >= 0) ||
				($location.path().indexOf("/account/create") >= 0) ||
				($location.path().indexOf("/account/forgot") >= 0);
		}

        $scope.$on('redirectToLogin', function () {
            redirectToLogin();
        });

	    $scope.$on('redirectToRoot', function() {
		    redirectToRoot();
	    });
    };

    NavigationController.$inject = ['$scope', '$location', '$window', 'userService'];

    angular.module(appModule).controller('NavigationController', NavigationController);

}());
