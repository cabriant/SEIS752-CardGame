(function () {

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

        factory.checkAuth = function () {
            if (factory.user != null && factory.user.isAuthenticated) {
                return true;
            }

            return $http.get(serviceBase + 'authuser').then(
                function(results) {
                    var loggedIn = results.data.authenticated;
                    changeAuth(loggedIn);
                }
            );
        }

        factory.redirectToLogin = function () {
            $rootScope.$broadcast('redirectToLogin', null);
        };

        function changeAuth(loggedIn) {
            factory.user = {
                isAuthenticated: loggedIn
            };
            $rootScope.$broadcast('loginStatusChanged', loggedIn);
        }

        return factory;
    };

    userFactory.$inject = ['$http', '$q', '$rootScope'];

    angular.module(appModule).factory('userService', userFactory);

}());

