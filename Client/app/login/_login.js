App.Router.map(function () {
	this.route('login');
});

App.LoginRoute = Ember.Route.extend({
	setupController: function (controller, context) {
		controller.reset();
	}
});

App.LoginController = App.ObjectController.extend({
	isAuthenticated: false,
	username: null,
	attemptedTransition: null,

	model: function () {
		return { user: null, password: null, errormessage: null };
	},

	init: function () {
		var authentication = JSON.parse(localStorage.getItem('authentication'));
		if (authentication && authentication.token && authentication.username) {
			this.set('isAuthenticated', true);
			this.set('username', authentication.username);
		}
	},

	reset: function () {
		this.setProperties({
			user: null,
			password: null,
			errorMessage: null
		});
	},

	clearAuthentication: function (transition) {
		localStorage.removeItem('authentication');
		this.set('isAuthenticated', false);
		this.set('username', null);
		this.set('attemptedTransition', transition);
		this.transitionToRoute('login');
	},

	actions: {

		login: function () {
			var data = this.getProperties('user', 'password');
			var self = this;

			App.AuthenticationApi.login(data).then(function (response) {
				if (response.success) {
					localStorage.setItem('authentication', JSON.stringify({ username: response.username, token: response.token }));
					self.set('isAuthenticated', true);
					self.set('username', response.username);

					var attemptedTransition = self.get('attemptedTransition');
					if (attemptedTransition) {
						attemptedTransition.retry();
						self.set('attemptedTransition', null);
					} else {
						self.transitionToRoute('events');
					}
				}
			});
		},

		logout: function () {
			this.clearAuthentication();
		}
	}
});
