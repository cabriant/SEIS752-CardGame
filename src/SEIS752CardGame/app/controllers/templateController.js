(function () {

	var TemplateController = function ($scope, $location, $routeParams, templateService) {
		// scope declarations


		function init() {
			
		}

		// scope functions


		// init
		init();
	};

	TemplateController.$inject = ['$scope', '$location', '$routeParams', 'templateService'];

	angular.module(appModule).controller('TemplateController', TemplateController);

}());

/*

$scope.action = function() {
	    	var result = tempService.tempAction($scope.val);
	    	result.then(function (data) {
	    		
		    });
	    };

*/