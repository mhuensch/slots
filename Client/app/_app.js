var SITE_NAME = 'slots';

window.App = Ember.Application.create({
	LOG_TRANSITIONS: true,							// basic logging of successful transitions
	LOG_TRANSITIONS_INTERNAL: true,			// detailed logging of all routing steps
	LOG_ACTIVE_GENERATION: true,
	ready: function () {
		console.log('Application ready');
	}
});

App.Router.map(function () {
	this.route('404', { path: '/*path' });
});

App.IndexRoute = Ember.Route.extend({
	//beforeModel: function () {
	//	this.transitionTo('events');
	//}
});

App.Route = Ember.Route.extend({

	enter: function (router) {
		this.controllerFor("application").setTitle(this.title);
	},
	
	actions: {

		error: function (error, transition) {
			// error handler called in case of an error.
			// show the error message to user here
			// or transition to another route
			if (error && error.status === 401) {
				this.controllerFor('login').clearAuthentication(transition);
				return;
			}
			console.log('application route error', error);
		}

	}

});

App.ObjectController = Ember.ObjectController.extend({
	needs: ['login'],
	isAuthenticated: Ember.computed.alias('controllers.login.isAuthenticated'),
	username: Ember.computed.alias('controllers.login.username'),
});
