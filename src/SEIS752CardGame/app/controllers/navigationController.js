(function () {

    var NavigationController = function ($scope, $location, userService) {
        $scope.isCollapsed = false;
        $scope.appTitle = 'Card Game';

        $scope.hide = function () {
            return ($location.path().indexOf("/login") >= 0);
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
            $location.replace();
            $location.path(path);
        }

        $scope.$on('redirectToLogin', function () {
            redirectToLogin();
        });

    };

    NavigationController.$inject = ['$scope', '$location', 'userService'];

    angular.module(appModule).controller('NavigationController', NavigationController);

}());
