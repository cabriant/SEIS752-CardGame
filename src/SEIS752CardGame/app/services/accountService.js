(function () {

	var accountFactory = function ($http, $q, $rootScope) {
		var serviceBase = '/api/accountservice/', factory = {};

		factory.createUser = function (email, password, confirmPassword, displayName, phoneNumber) {
			var obj = JSON.stringify({ email: email, password: password, confirmPassword: confirmPassword, displayName: displayName, phoneNumber: phoneNumber });
			return $http.post(serviceBase + 'create', obj, { headers: { 'Content-Type': 'application/json' } }).then(
                function (result) {
                	if (result.success)
                		alert('success');
	                redirectToLogin();
                });
		};

		factory.cancelCreate = function() {
			redirectToRoot();
		};

		function redirectToRoot() {
			$rootScope.$broadcast('redirectToRoot', null);
		};

		return factory;
	};

	accountFactory.$inject = ['$http', '$q', '$rootScope'];

	angular.module(appModule).factory('accountService', accountFactory);

}());