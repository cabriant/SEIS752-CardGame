(function () {

	var BlackjackController = function ($scope, $location, $routeParams, blackjackService) {
		// scope declarations
		$scope.tables = [];

		// scope functions
		

		// standard functions
		function init() {
			getTables();
		}

		function getTables() {
			var result = blackjackService.getTables();
			result.then(function (data) {
				if (data.success)
					$scope.tables = data.tables;
			});
		}

		// init
		init();
	};

	BlackjackController.$inject = ['$scope', '$location', '$routeParams', 'blackjackService'];

	angular.module(appModule).controller('BlackjackController', BlackjackController);

}());