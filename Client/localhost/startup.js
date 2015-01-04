//NOTE: there should be little need to change this file.	The client, services, and db
//for foogenda are loaded automatically by this startup process.
var https = require('https');
var fs = require('fs');
var express = require('express');
var colors = require('colors');
var path = require('path');

// CLEAR screen
// process.stdout.write('\033c');

// Give a linebreak for readability
process.stdout.write('\n');


// TODO: Implement some sort of logging system that is not synchronous
// and does not lock the system in logging loops.
// console.log = function() { }
// SEE: https://github.com/trentm/node-bunyan


//SERVER: configure the environment variable and server options
// =============================================================================
var app = express();
var serverOptions = {};
var env = process.env.NODE_ENV || 'development';
if ('development' === env) {
	console.log('RUNNING DEVELOPMENT ENVIRONMENT'.red);
	var hscert = fs.readFileSync(__dirname + '/development-cert.pem');
	var hskey = fs.readFileSync(__dirname + '/development-key.pem');
	serverOptions = { cert: hscert, key: hskey };
};



//CLIENT: this line is all it takes to host the single page web client for foogenda.
// =============================================================================
var clientPath = path.join(__dirname, 'app/bin/');
console.log('USING:', clientPath);
app.use('/app/', express.static(clientPath));

// =============================================================================
//CATCH ALL: return a 404 whenever a page is missing.
// =============================================================================
app.use('/*', function (request, response) {
	response.statusCode = 404;
	response.end('404!');
});


// START THE SERVER
// =============================================================================
var port = process.env.PORT || 44300;
var apiUrl = process.env.API_URL || '';
var server = startServer(port, function(){
	process.stdout.write('MAGIC: ');
	process.stdout.write('foo');
	process.stdout.write('genda '.magenta);
	console.log('is listening on port', server.address().port);
});


function startServer(port, callback) {
	if ('development' === env) {
		return https.createServer(serverOptions, app).listen(port, callback);
	}	else {
		return app.listen(port, callback);
	}
}
