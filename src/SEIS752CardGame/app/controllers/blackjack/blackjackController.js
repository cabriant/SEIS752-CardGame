(function () {

	var BlackjackController = function ($scope, $location, $routeParams, $modal, userService, blackjackService) {
		// scope declarations
		$scope.tables = [];
		$scope.canCreate = false;

		// scope functions
		$scope.create = function() {
			var modalInstance = $modal.open({
				templateUrl: '/app/views/blackjack/_create.html',
				controller: BlackjackModalController
			});

			modalInstance.result.then(function (tableInfo) {
				var result = blackjackService.createTable(tableInfo.name, tableInfo.ante, tableInfo.maxRaise, tableInfo.maxPlayers);
				result.then(function (data) {
					if (data.success) {
						$scope.tables = data.tables;
					}
				});
			});
		};

		$scope.joinTable = function (tableId) {
			var result = blackjackService.checkJoinTable(tableId);
			result.then(function(data) {
				if (data.success) {
					$location.path('/blackjack/table/' + tableId);
				} else {
					alert(data.error);
				}
			});
		};

		// standard functions
		function init() {
			adminInit();
			getTables();
		}

		function adminInit() {
			var result = userService.getAccess();
			result.then(function (data) {
				if (data.success)
					$scope.canCreate = data.create;
			});
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

	BlackjackController.$inject = ['$scope', '$location', '$routeParams', '$modal', 'userService', 'blackjackService'];

	angular.module(appModule).controller('BlackjackController', BlackjackController);


	var BlackjackModalController = function($scope, $modalInstance) {

		$scope.tableInfo = {
			name: null,
			ante: null,
			maxRaise: null,
			maxPlayers: 2
		};

		$scope.ok = function () {
			$modalInstance.close($scope.tableInfo);
		};

		$scope.cancel = function () {
			$modalInstance.dismiss('cancel');
		};
	};
}());