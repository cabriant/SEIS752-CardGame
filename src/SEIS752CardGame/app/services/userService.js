﻿(function () {

    var userFactory = function ($http, $q, $rootScope) {
        var serviceBase = '/api/userservice/',
            factory = {
                loginPath: '/login',
                user: null
            };

        factory.login = function(email, password) {
            var obj = JSON.stringify({ username: email, password: password });
            return $http.post(serviceBase + 'login', obj, { headers: { 'Content-Type': 'application/json' } }).then(
                function(results) {
                	var loggedIn = results.data.authenticated;
                    changeAuth(results.data);
                    return { authenticated: loggedIn, error: results.data.error };
                });
        };

        factory.logout = function () {
        	var deferred = $q.defer();
        	$http.post(serviceBase + 'logout')
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

        	return deferred.promise;
        };

        factory.getUser = function () {
            return asyncGetUser();
        }

		factory.getAccess = function() {
			var deferred = $q.defer();
			$http.get(serviceBase + 'useraccess')
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		}

        factory.redirectToLogin = function () {
            $rootScope.$broadcast('redirectToLogin', null);
        };

	    factory.clearUserData = function() {
	    	factory.user = {
	    		isAuthenticated: false,
	    		profile: null
	    	};
	    	broadcastAuth('userDeauthenticated');
	    };

        function changeAuth(response) {
			if (response.authenticated) {
				factory.user = {
					isAuthenticated: true,
					profile: response.user
				};
				broadcastAuth('userAuthenticated');
			} else {
				factory.user = {
					isAuthenticated: false,
					profile: null
				};
				broadcastAuth('userDeauthenticated');
			}
        }

        function asyncGetUser() {
            var deferred = $q.defer();

            setTimeout(function() {
                $rootScope.$apply(function() {
                    if (factory.user == null) {
                        $http.get(serviceBase + 'getuser').then(
                            function(results) {
                                changeAuth(results.data);
                                deferred.resolve(factory.user);
                            }
                        );
                    } else {
                        deferred.resolve(factory.user);
                    }
                });
            }, 25);

            return deferred.promise;
        }

		function broadcastAuth(authDeauth) {
			$rootScope.$broadcast(authDeauth, null);
		}

        return factory;
    };

    userFactory.$inject = ['$http', '$q', '$rootScope'];

    angular.module(appModule).factory('userService', userFactory);

}());

