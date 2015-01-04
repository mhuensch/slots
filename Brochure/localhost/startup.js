//NOTE: there should be little need to change this file.	The client, services, and db
//for foogenda are loaded automatically by this startup process.
var http = require('http');
var fs = require('fs');
var express = require('express');
var colors = require('colors');
var path = require('path');


//CLEAR screen
process.stdout.write('\033c');

// TODO: Implement some sort of loging system that is not synchronous
// and does not lock the system in logging loops.
// console.log = function() { }
// SEE: https://github.com/trentm/node-bunyan


//SERVER: configure the environment variable and server options
// =============================================================================
var app = express();
var serverOptions = {};


//CLIENT: this line is all it takes to host the single page web client for foogenda.
// =============================================================================
var clientPath = path.join(__dirname, 'brochure/bin/');
console.log('USING:', clientPath);
app.use('/', express.static(clientPath));


// =============================================================================
//CATCH ALL: return a 404 whenever a page is missing.
// =============================================================================
app.use("/*", function (request, response) {
	response.statusCode = 404;
	response.end("404!");
});


// START THE SERVER
// =============================================================================
var port = process.env.PORT || 19770;
//var logo = require("./ascii.logo.js");
//logo.write();
process.stdout.write('MAGIC: ');
process.stdout.write('foo');
process.stdout.write("genda ".magenta);
console.log("is listening on port", port);
app.listen(port);
