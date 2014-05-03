(function () {

	var templateFactory = function ($http, $q, $rootScope) {
		var serviceBase = '/api/templateservice/', factory = {};

		return factory;
	};

	templateFactory.$inject = ['$http', '$q', '$rootScope'];

	angular.module(appModule).factory('templateService', templateFactory);

}());

/*

factory.tempAction = function(val) {
			var obj = JSON.stringify({ val: val });
			var deferred = $q.defer();
			$http.post(serviceBase + 'tempEndpoint', obj, { headers: { 'Content-Type': 'application/json' } })
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

factory.tempAction = function() {
			var deferred = $q.defer();
			$http.get(serviceBase + 'tempEndpoint')
				.success(function (data) {
					deferred.resolve(data);
				}).error(function (data) {
					deferred.reject(data);
				});

			return deferred.promise;
		};

*/