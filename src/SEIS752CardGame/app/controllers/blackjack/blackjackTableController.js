(function () {

	var BlackjackTableController = function ($scope, $location, $routeParams, blackjackService) {
		// scope declarations


		function init() {
			
		}

		// scope functions


		// init
		init();
	};

	BlackjackTableController.$inject = ['$scope', '$location', '$routeParams', 'blackjackService'];

	angular.module(appModule).controller('BlackjackTableController', BlackjackTableController);

}());