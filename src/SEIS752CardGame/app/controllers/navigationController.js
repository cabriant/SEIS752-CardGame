(function () {

    var NavigationController = function ($scope, $location, userService) {
        $scope.isCollapsed = false;
        $scope.appTitle = 'Card Game';

        $scope.highlight = function (path) {
            return $location.path().substr(0, path.length) == path;
        };

        $scope.loginOrOut = function () {
            setLoginLogoutText();
            var isAuthenticated = userService.checkAuth();
            if (isAuthenticated) { //logout 
                userService.logout().then(function () {
                    $location.path(userService.loginPath);
                    return;
                });                
            }
            redirectToLogin();
        };

        function redirectToLogin() {
            var path = '/login' + $location.$$path;
            $location.replace();
            $location.path(path);
        }

        $scope.$on('loginStatusChanged', function (loggedIn) {
            setLoginLogoutText(loggedIn);
        });

        $scope.$on('redirectToLogin', function () {
            redirectToLogin();
        });

        function setLoginLogoutText() {
            $scope.loginLogoutText = (userService.checkAuth()) ? 'Logout' : 'Login';
        }

        setLoginLogoutText();

    };

    NavigationController.$inject = ['$scope', '$location', 'userService'];

    angular.module(appModule).controller('NavigationController', NavigationController);

}());
