App.Router.map(function () {
	this.resource('reset');
});
App.ResetRoute = App.Route.extend({
	title: 'Reset Local Data',
	beforeModel: function () {
		console.log('Resetting local data ...');
		for (var i = 0; i < localStorage.length; ++i) {
			var key = localStorage.key(i);
			if (key.substr(0, 4) !== 'api_') continue;
			localStorage.removeItem(key);
		}
		console.log('Local data reset');

		this.transitionTo('slots');
	}
});