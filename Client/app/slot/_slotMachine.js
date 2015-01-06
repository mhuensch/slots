(function ($) {

	$.jSlotMachine = function (machine_selector, options) {

		// --------------------------------------------------------------------- //
		// DEFAULT OPTIONS
		// --------------------------------------------------------------------- //

		$.jSlotMachine.defaultOptions = {
			lever: '',          // CSS Selector: element to bind the start event to
			reels: '',          // CSS Selector: list of slot symbols
			result: '',         // CSS Selector: list of symbols numbers
			loops: 5,          // Number: times it will spin during the animation
			minTime: 10000,      // Number: total time of spin animation
			maxTime: 11000,      // Number: total time of spin animation
			easing: 'swing'     // String: easing type for final spin
		};


		// --------------------------------------------------------------------- //
		// VARS
		// --------------------------------------------------------------------- //

		var self = this;
		self.$machine = $(machine_selector);
		self.isSpinning = false;
		self.spinSpeed = 0;
		self.doneCount = 0;
		self.results = [];
		self.slots = [];


		// --------------------------------------------------------------------- //
		// FUNCTIONS
		// --------------------------------------------------------------------- //

		self.init = function () {
			self.options = $.extend({}, $.jSlotMachine.defaultOptions, options);

			$(self.options.result).bind("DOMNodeInserted", function () {
				self.results = self.getSpinResult();
			});

			var lists = self.$machine.find(self.options.reels);
			for (var i = 0; i < lists.length; i++) {
				var $list = $(lists[i]);
				$list.data("jSlotMachine", self);
				self.slots.push(new self.Slot($list));
			}

			self.spinSlots();
		};


		self.spinSlots = function () {
			$(self.options.lever).prop("disabled", true);

			self.isSpinning = true;
			self.doneCount = 0;
			self.results = self.getSpinResult();

			$.each(self.slots, function (index, val) {
				var timeDif = self.options.maxTime - self.options.minTime;
				var timeRand = Math.floor((Math.random() * timeDif) + 1)
				var time = self.options.minTime + timeRand;
				var increment = (time / self.options.loops) / self.options.loops;

				this.increment = increment;
				this.spinSpeed = 0;
				this.loopCount = 0;
				this.spin();
			});

		};


		self.getSpinResult = function () {
			var result = [];
			$(self.options.result).find('li').each(function (index) {
				result[index] = $(this).text();
			});

			if (result.length !== self.slots.length) {
				return [];
			}

			return result;
		}


		self.slotStopped = function () {
			self.doneCount++;

			if (self.doneCount !== self.slots.length) return;

			$(self.options.lever).prop("disabled", false);
			self.isSpinning = false;
		};


		// --------------------------------------------------------------------- //
		// SLOT
		// --------------------------------------------------------------------- //
		self.Slot = function ($list) {

			this.spinSpeed = 0;
			this.loopCount = 0;
			this.increment = 0;
			this.startPositon = 0;

			$list.append($($list.find('li')[0]).clone());

			this.$list = $list;
			this.$items = $list.find('li');
			this.itemCount = this.$items.length;
			this.lineHeight = this.$items.first().outerHeight();
			this.listHeight = this.lineHeight * (this.itemCount - 1);


			this.spin = function () {
				var that = this;

				that.$list
					.css('top', -that.listHeight)
					.animate({ 'top': that.startPositon }, that.spinSpeed, 'linear', function () {
						that.accelerate();
					});
			},


			this.accelerate = function () {
				this.loopCount++;
				var result = self.results[self.slots.indexOf(this)];
				if (this.loopCount < self.options.loops) {
					this.spinSpeed += this.increment;
					this.spin();
				} else if (!result) {
					this.spin();
				} else {
					this.finish(result);
				}
			},


			this.finish = function (result) {
				var that = this;

				that.startPositon = -((that.lineHeight * result) - that.lineHeight);
				var finalSpeed = ((this.spinSpeed * 0.5) * (that.itemCount)) / result;

				that.$list
					.css('top', -that.listHeight)
					.animate({ 'top': that.startPositon }, finalSpeed, self.options.easing, function () {
						that.$items[that.$items.length - 1].remove();
						self.slotStopped();
					});
			}

		};


		self.init();
	};


	// --------------------------------------------------------------------- //
	// JQUERY FN
	// --------------------------------------------------------------------- //

	$.fn.jSlotMachine = function (options) {
		if (this.length) {
			return this.each(function () {
				(new $.jSlotMachine(this, options));
			});
		}
	};

})(jQuery);