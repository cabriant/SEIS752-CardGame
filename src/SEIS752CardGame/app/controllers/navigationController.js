(function () {

    var NavigationController = function ($scope, $route, $location, $window, userService) {
        $scope.isCollapsed = false;
        $scope.appTitle = 'Card Game';
	    $scope.displayName = null;

		$scope.highlight = function(path) {
			return $location.path().substr(0, path.length) == path;
		}

        $scope.hide = function () {
            return shouldHide();
        };

        $scope.logout = function () {
        	var result = userService.logout();
        	result.then(function (data) {
		        userService.clearUserData();
	        	redirectToLogin();
	        });
        };

        function redirectToLogin() {
            var path = '/login';
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

		function setDisplayName(name) {
			$scope.displayName = name;
		}

		function reloadLocation() {
			$route.reload();
		}

		$scope.$on('redirectToLogin', function () {
            redirectToLogin();
        });

		$scope.$on('redirectToRoot', function () {
		    redirectToRoot();
		});

	    $scope.$on('userAuthenticated', function() {
		    setDisplayName(userService.user.profile.displayName);
	    });

	    $scope.$on('userDeauthenticated', function() {
		    setDisplayName(null);
	    });

	    $scope.$on('refreshLocation', function() {
		    reloadLocation();
	    });
    };

    NavigationController.$inject = ['$scope', '$route', '$location', '$window', 'userService'];

    angular.module(appModule).controller('NavigationController', NavigationController);

}());
