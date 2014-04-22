(function () {

    var LoginController = function ($scope, $location, $routeParams, userService) {
        var path = '/';
        $scope.email = null;
        $scope.password = null;
        $scope.errorMessage = null;
        $scope.isEmailValid = true;

        $scope.login = function () {
            userService.login($scope.email, $scope.password).then(function (status) {
                //$routeParams.redirect will have the route
                //they were trying to go to initially
                if (!status) {
                    $scope.errorMessage = 'Unable to login';
                    return;
                }

                if (status && $routeParams && $routeParams.redirect) {
                    path = path + $routeParams.redirect;
                }

                $location.path(path);
            });
        };
    };

    LoginController.$inject = ['$scope', '$location', '$routeParams', 'userService'];

    angular.module(appModule)
        .controller('LoginController', LoginController);

}());