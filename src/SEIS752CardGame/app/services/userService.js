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
                    changeAuth(loggedIn);
                    return loggedIn;
                });
        };

        factory.logout = function () {
            return $http.post(serviceBase + 'logout').then(
                function (results) {
                    var loggedIn = results.data.authenticated;
                    changeAuth(loggedIn);
                    return loggedIn;
                });
        };

        factory.getUser = function () {
            return asyncGetUser();
        }

        factory.redirectToLogin = function () {
            $rootScope.$broadcast('redirectToLogin', null);
        };

        function changeAuth(loggedIn) {
            factory.user = {
                isAuthenticated: loggedIn
            };
        }

        function asyncGetUser() {
            var deferred = $q.defer();

            setTimeout(function() {
                $rootScope.$apply(function() {
                    if (factory.user == null) {
                        $http.get(serviceBase + 'getuser').then(
                            function(results) {
                                var loggedIn = results.data.authenticated;
                                changeAuth(loggedIn);
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

        return factory;
    };

    userFactory.$inject = ['$http', '$q', '$rootScope'];

    angular.module(appModule).factory('userService', userFactory);

}());
