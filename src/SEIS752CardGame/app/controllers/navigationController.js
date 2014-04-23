(function () {

    var NavigationController = function ($scope, $location, $window, userService) {
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
            $window.location.href = path;
        }

        $scope.$on('redirectToLogin', function () {
            redirectToLogin();
        });

    };

    NavigationController.$inject = ['$scope', '$location', '$window', 'userService'];

    angular.module(appModule).controller('NavigationController', NavigationController);

}());
