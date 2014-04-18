﻿var appModule = "cardGameApp";

(function () {

    var app = angular.module(appModule, ['ngRoute', 'ngAnimate', 'ui.bootstrap']);

    app.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {

        $routeProvider
            .when('/home', {
                controller: 'HomeController',
                templateUrl: '/app/views/home/home.html',
                reqSession: true
            })
            .when('/login/:redirect*', {
                controller: 'LoginController',
                templateUrl: '/app/views/login/login.html'
            })
            .when('/login', {
                controller: 'LoginController',
                templateUrl: '/app/views/login/login.html'
            })
            .otherwise({ redirectTo: '/home' });

        // use the HTML5 History API
        $locationProvider.html5Mode(true);
    }]);

    app.run(['$q', '$rootScope', '$location', 'userService',
        function ($q, $rootScope, $location, userService) {

            $rootScope.$on("$routeChangeStart", function (event, next, current) {
                if (next && next.$$route && next.$$route.reqSession) {
                    if (!userService.checkAuth()) {
                        userService.redirectToLogin();
                    }
                }
            });

        }]);
}());

