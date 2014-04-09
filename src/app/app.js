/// <reference path="../js/angular/1.2.16/angular.js" />

var app = angular.module('card-game-app', ['ngRoute']);

//This configures the routes and associates each route with a view and a controller
app.config(function ($routeProvider) {
    $routeProvider
        .when('/login',
            {
                controller: 'LoginController',
                templateUrl: 'app/partials/login.html'
            })
        //Define a route that has a route parameter in it (:customerID)
        // .when('/customerorders/:customerID',
        //     {
        //         controller: 'CustomerOrdersController',
        //         templateUrl: '/app/partials/customerOrders.html'
        //     })
        //Define a route that has a route parameter in it (:customerID)
        // .when('/orders',
        //     {
        //         controller: 'OrdersController',
        //         templateUrl: '/app/partials/orders.html'
        //     })
        .otherwise({ redirectTo: '/login' });
});
