App.MachineSlotComponent = Ember.Component.extend({
	//TODO: these things should be on the slot model
	stopOnIndex: null,

	slot: null,
	currentIndex: 0,
	rate: 1,
	acceleration: 0,

	speed: function () {
		var rate = 1/this.rate;
		return '-webkit-animation-duration: ' + rate + 's; animation-duration: ' + rate + 's;';
	}.property('rate'),

	start: function () {
		if (!this.slot.isSpinning) return;

		//Re-set the all the necessary variables back to their initial state
		this.set('rate', 1);
		this.set('stopOnIndex', null);

		//Set the acceleration to a random value between 1-5
		this.set('acceleration', Math.floor((Math.random() * 5) + 1));

		//Start the slot spinning
		this.spin();
	}.observes('slot.isSpinning'),

	spin: function () {
		var self = this;
		var index = self.currentIndex;
		var current = self.slot.tiles[index];

		if (self.currentIndex === self.slot.tiles.length - 1) {
			index = 0;
		} else {
			index++;
		}
		var next = self.slot.tiles[index];

		current.set('isActive', false);
		next.set('isActive', true);
		self.set('currentIndex', index);

		if (self.stopOnIndex !== null) {
			var distance = self.stopOnIndex - self.currentIndex;
			if (distance < 0) distance += self.slot.tiles.length;
			//console.log('distance', distance);

			if (distance !== 9 && self.rate === 10) {
				self.keepSpinning();
				return;
			}

			var rate = self.rate - 1;
			if (rate < 1) rate = 1;
			//console.log('slowing', rate);
			self.set('rate', rate);
		} else {
			var rate = this.rate + this.acceleration;
			if (rate > 10) rate = 10;
			self.set('rate', rate);
		}

		//TODO: for safety, remove check for rate === 1 - this is only here for dev purposes
		if (self.stopOnIndex === self.currentIndex && self.rate === 1) {
			Ember.run.later((function () {
				self.set('rate', 0);
				console.log('rate is 0');
			}), (1 / self.rate) * 1000);
			return;
		}
		self.keepSpinning();
	},

	keepSpinning: function () {
		var self = this;
		Ember.run.later((function () {
			self.spin();
		}), (1 / self.rate) * 1000);
	}

});

