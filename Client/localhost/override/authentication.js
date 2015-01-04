App.AuthenticationApi = {

	login: function () {
		return new Promise(function (resolve, reject) {

			var result = {
				success: true,
				token: 'local-token',
				username: 'LocalUser'
			}

			// on success
			resolve(result);
		});
	}

}