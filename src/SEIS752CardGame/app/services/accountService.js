(function () {

	var accountFactory = function ($http, $q, $rootScope) {
		var serviceBase = '/api/accountservice/', factory = {};

		factory.createUser = function (email, password, confirmPassword, displayName, phoneNumber) {
			var obj = JSON.stringify({ email: email, password: password, confirmPassword: confirmPassword, displayName: displayName, phoneNumber: phoneNumber });
			var deferred = $q.defer();
			$http.post(serviceBase + 'create', obj, { headers: { 'Content-Type': 'application/json' } })
				.success(function (data) {
					if (data.success)
						redirectToRoot();
					else
						deferred.resolve(data);
				}).error(function(data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.cancelAction = function() {
			redirectToRoot();
		};

		factory.submitForgot = function(email) {
			var obj = JSON.stringify({ email: email });
			var deferred = $q.defer();
			$http.post(serviceBase + 'forgot', obj, { headers: { 'Content-Type': 'application/json' } })
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.verifyCode = function(email, code) {
			var obj = JSON.stringify({ email: email, code: code });
			var deferred = $q.defer();
			$http.post(serviceBase + 'verify', obj, { headers: { 'Content-Type': 'application/json' } })
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.resetPassword = function (email, token, newPassword, newPasswordConfirm) {
			var obj = JSON.stringify({ email: email, token: token, newPassword: newPassword, newPasswordConfirm: newPasswordConfirm });
			var deferred = $q.defer();
			$http.post(serviceBase + 'reset', obj, { headers: { 'Content-Type': 'application/json' } })
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		function redirectToRoot() {
			$rootScope.$broadcast('redirectToRoot', null);
		};

		return factory;
	};

	accountFactory.$inject = ['$http', '$q', '$rootScope'];

	angular.module(appModule).factory('accountService', accountFactory);

}());