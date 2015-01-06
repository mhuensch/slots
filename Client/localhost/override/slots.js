App.SlotsApi = new LocalData('slots', [
	{
		'id': 1,
		'key': 'testmachine',
		'title': 'Test Slot Machine',
		'reels': [
			{
				'id': 1,
				'symbols': [
					{ 'id': 1, 'url': 'images/tile.01.svg' },
					{ 'id': 2, 'url': 'images/tile.02.svg' },
					{ 'id': 3, 'url': 'images/tile.03.svg' },
					{ 'id': 4, 'url': 'images/tile.04.svg' },
					{ 'id': 5, 'url': 'images/tile.05.svg' },
					{ 'id': 6, 'url': 'images/tile.06.svg' },
					{ 'id': 7, 'url': 'images/tile.07.svg' },
					{ 'id': 8, 'url': 'images/tile.08.svg' },
					{ 'id': 9, 'url': 'images/tile.09.svg' }
				]
			}, {
				'id': 2,
				'symbols': [
					{ 'id': 1, 'url': 'images/tile.01.svg' },
					{ 'id': 2, 'url': 'images/tile.02.svg' },
					{ 'id': 3, 'url': 'images/tile.03.svg' },
					{ 'id': 4, 'url': 'images/tile.04.svg' },
					{ 'id': 5, 'url': 'images/tile.05.svg' },
					{ 'id': 6, 'url': 'images/tile.06.svg' },
					{ 'id': 7, 'url': 'images/tile.07.svg' },
					{ 'id': 8, 'url': 'images/tile.08.svg' },
					{ 'id': 9, 'url': 'images/tile.09.svg' }
				]
			}, {
				'id': 3,
				'symbols': [
					{ 'id': 1, 'url': 'images/tile.01.svg' },
					{ 'id': 2, 'url': 'images/tile.02.svg' },
					{ 'id': 3, 'url': 'images/tile.03.svg' },
					{ 'id': 4, 'url': 'images/tile.04.svg' },
					{ 'id': 5, 'url': 'images/tile.05.svg' },
					{ 'id': 6, 'url': 'images/tile.06.svg' },
					{ 'id': 7, 'url': 'images/tile.07.svg' },
					{ 'id': 8, 'url': 'images/tile.08.svg' },
					{ 'id': 9, 'url': 'images/tile.09.svg' }
				]
			}
		]
	}
]);

App.SlotsApi.placeBet = function () {
	return new Promise(function (resolve, reject) {
		var result = {
			symbols: [Math.floor(Math.random() * 7), Math.floor(Math.random() * 7), Math.floor(Math.random() * 7)]
		};

		// on success
		resolve(result);
	});
}