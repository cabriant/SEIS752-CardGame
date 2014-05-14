(function () {

	var BlackjackTableController = function ($scope, $location, $routeParams, $interval, blackjackService) {
		// scope declarations
		$scope.game = null;
		$scope.bank = {
			currentBalance: null,
			newBalance: null
		};
		$scope.bet = {
			betValue: null,
			betErrors: null
		};

		var update;

		function init() {
			initTable();
		}

		function initTable() {
			var result = blackjackService.initTable($routeParams.tableId);
			result.then(function(data) {
				if (data.success) {
					$scope.game = data.game;
					$scope.bank.currentBalance = data.game.currentPlayersCashValue;
					$scope.bank.newBalance = data.game.currentPlayersCashValue;

					if (data.game.currentStage != 0 && data.game.currentStage != 4 && data.game.currentStage != 6) {
						if (!angular.isDefined(update)) {
							update = $interval(updateTable, 5000);
						}
					}
				}
			});
		}

		function updateTable() {
			var result = blackjackService.updateTable($scope.game.table, $scope.game.currentGame);
			result.then(function(data) {
				if (data.success) {
					$scope.game = data.game;

					if (data.game.currentStage != 0 && data.game.currentStage != 4 && data.game.currentStage != 6) {
						if (!angular.isDefined(update)) {
							update = $interval(updateTable, 5000);
						}
					} else {
						if (angular.isDefined(update)) {
							$interval.cancel(update);
							update = null;
						}
					}
				}
			});
		}

		// scope functions
		$scope.betChanged = function() {
			$scope.bank.newBalance = $scope.bank.currentBalance - $scope.bet.betValue;
		};

		$scope.leaveTable = function() {
			blackjackService.leaveTable($scope.game.table);
			$scope.game = null;
			$scope.bet = null;
			$scope.bank = null;
			table = null;
			$location.path('/blackjack');
		};

		$scope.placeBet = function() {
			var result = blackjackService.placeBet($scope.game.currentGame, $scope.bet.betValue);
			result.then(function(data) {
				if (!data.success) {
					$scope.bet.betErrors = data.errors;
				} else {
					updateTable();
				}
			});
		};

		$scope.splitHand = function () {
			var result = blackjackService.splitHand($scope.game.table, $scope.game.currentGame);
			result.then(function (data) {
				if (!data.success) {
					$scope.bet.playerActionErrors = data.errors;
				} else {
					updateTable();
				}
			});
		};

		$scope.standHand = function() {
			var result = blackjackService.standHand($scope.game.table, $scope.game.currentGame);
			result.then(function (data) {
				if (!data.success) {
					$scope.bet.playerActionErrors = data.errors;
				} else {
					updateTable();
				}
			});
		};

		$scope.hitHand = function() {
			var result = blackjackService.hitHand($scope.game.table, $scope.game.currentGame);
			result.then(function (data) {
				if (!data.success) {
					$scope.bet.playerActionErrors = data.errors;
				} else {
					updateTable();
				}
			});
		};

		$scope.playAgain = function() {
			blackjackService.playAgain();
		};

		// init
		init();
	};

	BlackjackTableController.$inject = ['$scope', '$location', '$routeParams', '$interval', 'blackjackService'];

	angular.module(appModule).controller('BlackjackTableController', BlackjackTableController);

}());