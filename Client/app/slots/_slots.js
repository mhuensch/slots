App.Router.map(function () {
	this.resource('slots');
});

App.SlotsRoute = App.Route.extend({
	title: 'Slots',
	model: function (params) {
		return App.SlotsApi.query();
	}
});

App.SlotsController = App.ObjectController.extend({

});
