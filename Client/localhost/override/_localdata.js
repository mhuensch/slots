function LocalData(key, data) {
	key = 'api_' + key;

	this.query = function (args) {
		return new Promise(function (resolve, reject) {
			var result = getStore();
			result = filter(result, args);
			resolve(result);
		});
	};

	this.get = function (id) {
		//params { 'id': $routeParams.id }
	}

	this.save = function (model) {
	}

	this.delete = function (id) {
	}

	function getStore() {
		var authentication = localStorage.getItem('authentication');
		if (!authentication) {
			var error = new Error("Un-authenticated.  Log in using any user or method.");
			error.status = 401;
			throw error;
		}

		var result = JSON.parse(localStorage.getItem(key));
		if (!result) {
			console.log('Initializing data for local ' + key);
			localStorage.setItem(key, JSON.stringify(data));
			result = JSON.parse(localStorage.getItem(key));
		}

		return result;
	}

	function filter(data, params) {
		if (!params || !params.args)
			return data;

		var query = params.args;
		return data.filter(function (item) {
			for (var propertyName in query) {
				if (typeof item[propertyName] === 'undefined') {
					continue;
				}

				if (jQuery.type(item[propertyName]) === "string" && jQuery.type(query[propertyName]) === "string") {
					if (item[propertyName].toLowerCase().indexOf(trimChar(query[propertyName].toLowerCase(), '*')) > -1) {
						continue;
					}
				}

				if (item[propertyName] != query[propertyName]) {
					return false;
				}
			}

			return true;
		});
	};

	function trimChar(string, charToRemove) {
		while (string.charAt(0) === charToRemove) {
			string = string.substring(1);
		}

		while (string.charAt(string.length - 1) === charToRemove) {
			string = string.substring(0, string.length - 1);
		}

		return string;
	};
}