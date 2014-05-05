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

		factory.getTable = function(tableId) {
			var deferred = $q.defer();
			$http.get(serviceBase + 'table/' + tableId)
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.createTable = function (name, ante, maxRaise, maxPlayers) {
			var obj = JSON.stringify({ name: name, ante: ante, maxRaise: maxRaise, maxPlayers: maxPlayers });
			var deferred = $q.defer();
			$http.post(serviceBase + 'createtable', obj, { headers: { 'Content-Type': 'application/json' } })
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