App.MachinesApi = new LocalData('machines', [
	{
		'id': 1,
		'key': 'testmachine',
		'title': 'Test Slot Machine',
		'slots': [
			{
				'id': 1,
				'tiles': [
					{ 'id': 1, 'url': 'images/tile.01.svg' },
					{ 'id': 2, 'url': 'images/tile.02.svg' },
					{ 'id': 3, 'url': 'images/tile.03.svg' },
					{ 'id': 4, 'url': 'images/tile.04.svg' },
					{ 'id': 5, 'url': 'images/tile.05.svg' },
					{ 'id': 6, 'url': 'images/tile.06.svg' },
					{ 'id': 7, 'url': 'images/tile.07.svg' },
					{ 'id': 8, 'url': 'images/tile.08.svg' },
					{ 'id': 9, 'url': 'images/tile.09.svg' },
					{ 'id': 10, 'url': 'images/tile.10.svg' }
				]
			}, {
				'id': 2,
				'tiles': [
					{ 'id': 1, 'url': 'images/tile.01.svg' },
					{ 'id': 2, 'url': 'images/tile.02.svg' },
					{ 'id': 3, 'url': 'images/tile.03.svg' },
					{ 'id': 4, 'url': 'images/tile.04.svg' },
					{ 'id': 5, 'url': 'images/tile.05.svg' },
					{ 'id': 6, 'url': 'images/tile.06.svg' },
					{ 'id': 7, 'url': 'images/tile.07.svg' },
					{ 'id': 8, 'url': 'images/tile.08.svg' },
					{ 'id': 9, 'url': 'images/tile.09.svg' },
					{ 'id': 10, 'url': 'images/tile.10.svg' }
				]
			}, {
				'id': 3,
				'tiles': [
					{ 'id': 1, 'url': 'images/tile.01.svg' },
					{ 'id': 2, 'url': 'images/tile.02.svg' },
					{ 'id': 3, 'url': 'images/tile.03.svg' },
					{ 'id': 4, 'url': 'images/tile.04.svg' },
					{ 'id': 5, 'url': 'images/tile.05.svg' },
					{ 'id': 6, 'url': 'images/tile.06.svg' },
					{ 'id': 7, 'url': 'images/tile.07.svg' },
					{ 'id': 8, 'url': 'images/tile.08.svg' },
					{ 'id': 9, 'url': 'images/tile.09.svg' },
					{ 'id': 10, 'url': 'images/tile.10.svg' }
				]
			}
		]
	}
]);

App.MachinesApi.placeBet = function () {
	return new Promise(function (resolve, reject) {
		var result = {
			slots: [
				{ id: 1, tile: Math.floor(Math.random() * 7) },
				{ id: 2, tile: Math.floor(Math.random() * 7) },
				{ id: 3, tile: Math.floor(Math.random() * 7) },
			]
		};

		// on success
		resolve(result);
	});
}