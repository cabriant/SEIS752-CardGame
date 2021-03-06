﻿(function () {

    var NavigationController = function ($scope, $route, $location, $window, userService, $modal, accountService) {
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

	    $scope.$on('refreshLocation', function () {
		    reloadLocation();
	    });

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

	            modalInstance.result.then(function (userInfo) {
	            	
	                var result = accountService.addCashMoney(userInfo.addCashValue);
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

	NavigationController.$inject = ['$scope', '$route', '$location', '$window', 'userService','$modal','accountService'];

    angular.module(appModule).controller('NavigationController', NavigationController);

    var AccountModalController = function($scope, $modalInstance, userCashValue) {
    	//alert(userCashValue);
    	console.log("3 "+userCashValue);
    	$scope.seevalue = userCashValue;
		$scope.userInfo = {
			cashValue: userCashValue,
			addCashValue: 0
		};

		$scope.ok = function () {
			$modalInstance.close($scope.userInfo);
		};

		$scope.cancel = function () {
			$modalInstance.dismiss('cancel');
		};
	};

}());
