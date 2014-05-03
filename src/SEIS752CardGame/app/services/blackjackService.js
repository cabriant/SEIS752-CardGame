(function () {

	var blackjackFactory = function ($http, $q, $rootScope) {
		var serviceBase = '/api/blackjackservice/', factory = {};

		factory.getTables = function () {
			var deferred = $q.defer();
			$http.get(serviceBase + 'tables')
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		return factory;
	};

	blackjackFactory.$inject = ['$http', '$q', '$rootScope'];

	angular.module(appModule).factory('blackjackService', blackjackFactory);

}());