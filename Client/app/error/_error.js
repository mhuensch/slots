App.Router.map(function () {
	this.resource('error', { path: '/errors/:key' });
});

App.ErrorRoute = App.Route.extend({
	title: 'Error',
	model: function(params) {
		return params;
	},
	renderTemplate: function () {
		var controller = this.controllerFor('error');
		this.render(controller.model.key);
	}
});