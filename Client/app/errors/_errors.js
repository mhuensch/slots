App.Router.map(function () {
	this.resource('errors');
});

App.ErrorsRoute = App.Route.extend({
	title: 'Errors',
	model: function () {
		return App.ERRORS;
	}
});

App.ERRORS = [
	{
		'key': '000',
		'location': '#/errors/000'
	},{
		'key': '400',
		'location': '#/errors/400'
	}, {
		'key': '403',
		'location': '#/errors/403'
	}, {
		'key': '404',
		'location': '#/errors/404'
	}, {
		'key': '408',
		'location': '#/errors/408'
	}, {
		'key': '500',
		'location': '#/errors/500'
	}, {
		'key': '501',
		'location': '#/errors/501'
	}, {
		'key': '503',
		'location': '#/errors/503'
	}
];