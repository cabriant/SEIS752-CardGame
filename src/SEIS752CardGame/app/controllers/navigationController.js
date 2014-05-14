(function () {

    var NavigationController = function ($scope, $location, $window, userService, $modal, accountService) {
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
            userService.logout().then(function () {
                    $location.path(userService.loginPath);
                    return;
                });
            redirectToLogin();
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

	    function getUserCashValue() {

			var result 
	    }

	    $scope.viewProfile = function() {
	    	var userCashValue = 0;
	    	var accountValue = accountService.getAccountValue();
			accountValue.then(function(userInfo) { 
				//alert(data.userCashValue);
				console.log("1 "+userInfo.userCashValue);
				userCashValue = userInfo.userCashValue;

				var modalInstance = $modal.open({
				templateUrl: '/app/views/account/_viewprofile.html',
				controller: AccountModalController,
				resolve: {
					userCashValue: function() { 
						
						console.log("2 "+userCashValue);
							return userCashValue;
						}
					}
				});

				modalInstance.result.then(function (tableInfo) {
				var result = blackjackService.createTable(tableInfo.name, tableInfo.ante, tableInfo.maxRaise, tableInfo.maxPlayers);
				result.then(function (data) {
					if (data.success) {
						$scope.tables = data.tables;
					}
				});
			});

			}, function(data) {
				console.log(data);
				
			});

	    	

			
	    };
    };

    NavigationController.$inject = ['$scope', '$location', '$window', 'userService','$modal','accountService'];

    angular.module(appModule).controller('NavigationController', NavigationController);

    var AccountModalController = function($scope, $modalInstance, userCashValue) {
    	//alert(userCashValue);
    	console.log("3 "+userCashValue);
    	$scope.seevalue = userCashValue;
		$scope.userInfo = {
			cashValue: userCashValue
		};

		$scope.ok = function () {
			$modalInstance.close($scope.tableInfo);
		};

		$scope.cancel = function () {
			$modalInstance.dismiss('cancel');
		};
	};

}());
