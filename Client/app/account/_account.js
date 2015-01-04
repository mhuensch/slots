App.Router.map(function () {
	this.resource('account', { path: '/accounts/:key' });
});

App.AccountRoute = App.Route.extend({
	title: 'My Account',
	model: function (params) {
		var args = { args: { key: params.key } };
		return App.AccountsApi.query(args)
			.then(function (data) { return data[0] });
	}
});

App.AccountController = App.ObjectController.extend({
	needs: ['login'],
	actions: {
		logout: function () {
			this.get('controllers.login').send('logout');
		}
	}
});
