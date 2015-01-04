App.ApplicationRoute = App.Route.extend({
	actions: {

		showOverlayPanel: function (panelName, model) {
			this.controllerFor(panelName).set('model', model);
			return this.render(panelName, {
				into: 'application',
				outlet: 'overlay'
			});
		},

		hideOverlayPanels: function () {
			return this.disconnectOutlet({
				outlet: 'overlay',
				parentView: 'application'
			});
		}
	}
});

App.ApplicationController = App.ObjectController.extend({
	title: SITE_NAME,
	setTitle: function (routeTitle) {
		var browserTitle = SITE_NAME;
		this.set('title', SITE_NAME);

		if (routeTitle) {
			this.set('title', routeTitle);
			browserTitle = routeTitle + ' : ' + browserTitle;
		}

		$(document).attr('title', browserTitle);
	}
});
