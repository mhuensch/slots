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

	afterModel: function (model, transition) {
		//Covert the raw data model to an ember model
		var slots = model.slots.map(function (slot) {
			return App.Slot.create({
				id: slot.id,
				tiles: slot.tiles.map(function (tile) {
					return App.Tile.create({
						id: tile.id,
						url: tile.url,
						isActive: slot.tiles.indexOf(tile) === 0
					});
				})
			})
		});
		model.slots = slots;
	}
});

App.MachineController = App.ObjectController.extend({
	isSpinning: false,
	result: null,

	canSpin: function () {
		console.log('can spin running');
		var model = this.get('model');
		for (var i = 0; i < model.slots.length; i++) {
			if (model.slots[i].isSpinning) return false;
		}
		return true;
	}.observes('model.slots.@each.isSpinning').property('isSpinning'),

	spin: function () {
		var model = this.get('model');
		model.slots.map(function (slot) {
			slot.set('isSpinning', true);
		});
	}.observes('isSpinning'),

	bet: function() {
		var self = this;
		App.MachinesApi.placeBet().then(function (data) {
			console.log('stopping on', data.slots.map(function (slot) {
				return slot.tile + 1;
			}).toString());
			self.set('result', data);
		});
	}.observes('isSpinning'),

	actions: {

		start: function () {
			this.set('isSpinning', true);
		},

		stop: function () {
			this.set('isSpinning', false);
			var result = this.get('result').slots;
			var slots = this.get('model').slots;

			for (var i = 0; i < result.length; i++) {
				slots[i].set('stopOnIndex', result[i].tile);
			}
		}

	}
});


App.Slot = Ember.Object.extend({
	id: null,
	isSpinning: false,
	stopOnIndex: null,
	rate: 0,
	tiles: []
});

App.Tile = Ember.Object.extend({
	id: null,
	url: null,
	isActive: false,
	backgroundStyle: function () {
		return 'background-image:url("' + this.get('url') + '")';
	}.property('url')
});