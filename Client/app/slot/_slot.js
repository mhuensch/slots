App.Router.map(function () {
	this.resource('slot', { path: '/slots/:key' });
});

App.SlotRoute = App.Route.extend({
	title: 'Slot Machine',

	model: function (params) {
		var args = { args: { key: params.key } };
		return App.SlotsApi.query(args)
			.then(function (data) { return data[0] });
	},

	setupController: function(controller, model){
		this._super(controller, model);
		Ember.run.scheduleOnce('afterRender', this, function () {
			$('#lever').click(function () {
				controller.send('spin');
				$('#machine-window').jSlotMachine({
					lever: '#lever',        // CSS Selector: element disable
					reels: '.slot',         // CSS Selector: list of slot symbols
					result: '#result'       // CSS Selector: list of symbols numbers
				});
			});
		});
	}

});

App.SlotController = App.ObjectController.extend({
	result: null,

	actions: {

		spin: function () {
			var self = this;

			self.set('result', null);
			App.SlotsApi.placeBet().then(function (data) {
				self.set('result', data);
			});
		},

		stop: function () {
			console.log('stopping');
		}

	}

});
