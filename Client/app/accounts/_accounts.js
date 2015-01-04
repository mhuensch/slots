App.Router.map(function () {
	this.resource('accounts');
});

App.AccountsRoute = App.Route.extend({
	title: 'Accounts',
	model: function () {
		return App.AccountsApi.query();
	}
});

App.AccountsController = App.ObjectController.extend({

});
