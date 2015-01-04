App.Router.map(function () {
	this.resource('machines');
});

App.MachinesRoute = App.Route.extend({
	title: 'Machines',
	model: function (params) {
		return App.MachinesApi.query();
	}
});

App.MachinesController = App.ObjectController.extend({

});
