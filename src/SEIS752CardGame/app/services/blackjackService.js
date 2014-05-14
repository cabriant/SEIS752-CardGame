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

		factory.checkJoinTable = function(tableId) {
			var deferred = $q.defer();
			$http.get(serviceBase + 'checkjointable/' + tableId)
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.initTable = function(tableId) {
			var deferred = $q.defer();
			$http.get(serviceBase + 'table/' + tableId)
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.updateTable = function(tableId, gameId) {
			var deferred = $q.defer();
			$http.get(serviceBase + 'updatetable?tableId=' + tableId + '&gameId=' + gameId)
				.success(function(data) {
					deferred.resolve(data);
				}).error(function(data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.placeBet = function(gameId, bet) {
			var obj = JSON.stringify({ gameId: gameId, bet: bet });
			var deferred = $q.defer();
			$http.post(serviceBase + 'placebet', obj, { headers: { 'Content-Type': 'application/json' } })
				.success(function(data) {
					deferred.resolve(data);
				}).error(function(data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.splitHand = function (tableId, gameId) {
			var obj = JSON.stringify({ tableId: tableId, gameId: gameId });
			var deferred = $q.defer();
			$http.post(serviceBase + 'splithand', obj, { headers: { 'Content-Type': 'application/json' } })
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.standHand = function (tableId, gameId) {
			var obj = JSON.stringify({ tableId: tableId, gameId: gameId });
			var deferred = $q.defer();
			$http.post(serviceBase + 'standhand', obj, { headers: { 'Content-Type': 'application/json' } })
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.hitHand = function (tableId, gameId) {
			var obj = JSON.stringify({ tableId: tableId, gameId: gameId });
			var deferred = $q.defer();
			$http.post(serviceBase + 'hithand', obj, { headers: { 'Content-Type': 'application/json' } })
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.leaveTable = function(tableId) {
			var obj = JSON.stringify({ tableId: tableId });
			var deferred = $q.defer();
			$http.post(serviceBase + 'leavetable', obj, { headers: { 'Content-Type': 'application/json' } })
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

		factory.playAgain = function() {
			$rootScope.$broadcast('refreshLocation', null);
		};

		return factory;
	};

	blackjackFactory.$inject = ['$http', '$q', '$rootScope'];

	angular.module(appModule).factory('blackjackService', blackjackFactory);

}());