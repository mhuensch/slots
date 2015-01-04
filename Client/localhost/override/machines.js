App.MachinesApi = new LocalData('machines', [
	{
		'id': 1,
		'key': 'testmachine',
		'title': 'Test Slot Machine',
		'slots': [
			{
				'id': 1,
				'tiles': [
					{ 'id': 1, 'url': 'images/title.01.svg' },
					{ 'id': 2, 'url': 'images/title.02.svg' }
				]
			}, {
				'id': 2,
				'tiles': [
					{ 'id': 1, 'url': 'images/title.01.svg' },
					{ 'id': 2, 'url': 'images/title.02.svg' }
				]
			}, {
				'id': 3,
				'tiles': [
					{ 'id': 1, 'url': 'images/title.01.svg' },
					{ 'id': 2, 'url': 'images/title.02.svg' }
				]
			}
		]
	}
]);