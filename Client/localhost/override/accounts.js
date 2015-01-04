App.AccountsApi = new LocalData('accounts', [
	{
		'id': 1,
		'key': 'LocalUser',
		'name': 'Mike LocalUser',
		'email': 'local.user@email.com',
		'role': 'Admin',
		'image': 'https://localhost:44300/app/images/blank-profile-image.png',
		'contactsLastSyncDate': new Date('2014-08-13 13:00:00')
	}, {
		'id': 2,
		'key': 'TestUser',
		'name': 'Test Account Name',
		'email': 'test.user@email.com',
		'role': 'User',
		'image': 'https://localhost:44300/app/images/blank-profile-image.png',
		'contactsLastSyncDate': new Date('2014-08-13 13:00:00')
	}
]);