App.Router.map(function () {
	this.resource('machine', { path: '/machines/:key' });
});

App.MachineRoute = App.Route.extend({
	title: 'Slot Machine',
	model: function (params) {
		var args = { args: { key: params.key } };
		return App.MachinesApi.query(args)
			.then(function (data) { return data[0] });
	},
	setupController: function(controller, model) {
		this._super(controller, model);


		//this.controllerFor('slots').set('currentPost', post);
	}
});

App.MachineController = App.ObjectController.extend({
	yin: 0,
	yang: 1,
	current: 0,
	spinning: false,
	stopping: false,
	isYangCurrent: false,
	url: "background-image:url('images/tile.02.svg')",

	actions: {
		start: function () {
			this.set('spinning', true);
			this.set('stopping', false);
			this.send('spin');
		},

		stop: function () {
			this.set('spinning', false);
			this.set('stopping', true);
		},

		spin: function () {
			if (this.get('current') === this.get('yin')) {
				this.set('yang', this.get('yin') + 1);
				this.set('current', this.get('yang'));
				this.set('isYangCurrent', true);
			} else {
				this.set('yin', this.get('yang') + 1);
				this.set('current', this.get('yin'));
				this.set('isYangCurrent', false);
			}

			var self = this;
			Ember.run.later((function () {
				if (self.get('stopping') === true) return;

				self.send('spin');
			}), 100);
		}
	}
});

App.Tile = Ember.Object.extend({
	url: "",
	style: function () {
		"background-image:url('" + this.get("url") + "')"
	}.property("url")
});