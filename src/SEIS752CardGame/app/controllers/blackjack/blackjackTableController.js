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
		
		var table = null;

		function init() {
			//$scope.player = {
			//	bet: null,
			//	currentBank: null,
			//	bank: null
			//};
			initTable();
		}

		function initTable() {
			var result = blackjackService.initTable($routeParams.tableId);
			result.then(function(data) {
				if (data.success) {
					$scope.game = data.game;
					$scope.bank.currentBalance = data.game.currentPlayersCashValue;
					$scope.bank.newBalance = data.game.currentPlayersCashValue;
					table = data.game.table;
				}
			});
		}

		function updateTable() {
			var result = blackjackService.updateTable(table, $scope.game.currentGame);
			result.then(function(data) {
				if (data.success) {
					$scope.game = data.game;
				}
			});
		}

		// scope functions
		$scope.betChanged = function() {
			$scope.bank.newBalance = $scope.bank.currentBalance - $scope.bet.betValue;
		};

		$scope.leaveTable = function() {
			blackjackService.leaveTable(table);
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

		$scope.getImage = function(card) {
			var cardName = card.replace('c_', '');
			if (cardName == "back") {
				return "/Content/images/cards/back.png";
			}
			return "/Content/images/cards/" + cardName + ".gif";
		};

		// init
		init();
	};

	BlackjackTableController.$inject = ['$scope', '$location', '$routeParams', '$interval', 'blackjackService'];

	angular.module(appModule).controller('BlackjackTableController', BlackjackTableController);

}());