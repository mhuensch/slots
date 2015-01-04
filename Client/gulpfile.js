var rimraf = require('rimraf');
var runSequence = require('run-sequence');
var path = require('path');
var stream = require('stream');
var through2 = require('through2');
var str2js = require('string-to-js');

var gulp = require('gulp');
var gutil = require('gulp-util');

var plugins = require("gulp-load-plugins")({
	pattern: ['gulp-*', 'gulp.*'],
	replaceString: /\bgulp[\-.]/
});


var useCompression = gutil.env.debug ? false : true;

console.log('with data', gutil.env.withData);
console.log('with api', gutil.env.withApi);
console.log('with host', gutil.env.withHost);
console.log('with debugging', useCompression);
process.stdout.write('\n');


var dest = gutil.env.dest || "localhost/app/bin";
var paths = {
	contentFiles: function () {
		var result = ['vendor/**/*.*', 'app/**/*.html', 'app/**/*.eot', 'app/**/*.svg', 'app/**/*.ttf', 'app/**/*.woff', 'app/**/*.otf', 'app/**/*.png', 'app/**/*.ico'];
		if (gutil.env.withData) {
			result.push('localdata/images/*.*');
		}
		return result;
	},
	libFiles: function () {
		var result = require('bower-files')().js;
		result.push('libraries/**/*.js');
		return result
	},
	lessFiles: function () {
		//We are including _normalize.js and _vendor-prefixes.less here explicitly to avoid an out of order build issue
		return ['less/libraries/*.css', 'less/_normalize.less', 'less/_vendor-prefixes.less', 'less/*.less'];
	},
	applicationFiles: function () {
		//We are including _application.js here explicitly to avoid an out of order build issue
		var result = ['app/_app.js', 'app/**/*.js'];
		if (gutil.env.withData) {
			result.push('localhost/override/**/*.js');
		}

		return result;
	},
	templateFiles: function () {
		return ['app/**/*.hbs', 'app/error/*.html'];
	}
};


// TODO: add build step for compressing images
gulp.task('content', function () {
	return gulp.src(paths.contentFiles())
	.pipe(plugins.print(function (filepath) { return "content: " + filepath; }))
	.pipe(gulp.dest(dest));
});


gulp.task('libraries', function () {
	return gulp.src(paths.libFiles())
		.pipe(plugins.print(function (filepath) { return "lib: " + filepath; }))
		.pipe(plugins.concat('libraries.js'))
		.pipe(useCompression ? plugins.uglify() : gutil.noop())
		.pipe(gulp.dest(dest));
});


gulp.task('less', function () {
	return gulp.src(paths.lessFiles())
		.pipe(plugins.print(function (filepath) { return "less: " + filepath; }))
		//.pipe(useCompression ? plugins.sourcemaps.init() : gutil.noop())
		.pipe(plugins.concat('index.css'))
		.pipe(plugins.less())
		//.pipe(useCompression ? plugins.sourcemaps.write() : gutil.noop())
		.pipe(gulp.dest(dest));
});


gulp.task('application', function () {
	return gulp.src(paths.applicationFiles())
		.pipe(plugins.print(function (filepath) { return "app: " + filepath; }))
		//.pipe(useCompression ? plugins.sourcemaps.init() : gutil.noop())
		.pipe(plugins.concat('index.js'))
		.pipe(useCompression ? plugins.uglify() : gutil.noop())
		//.pipe(useCompression ? plugins.sourcemaps.write() : gutil.noop())
		.pipe(gulp.dest(dest));
});


gulp.task('templates', function () {
	return gulp.src(paths.templateFiles())
		.pipe(plugins.print(function (filepath) { return "template: " + filepath; }))
		.pipe(modify(wrapTemplate))
		.pipe(plugins.concat('templates.js'))
		.pipe(gulp.dest(dest));
});


gulp.task('test', function () {
	//TODO: add some ui tests
	return true;
});


gulp.task('clean', function (callback) { return rimraf(dest, callback); });

gulp.task('mark-rebuild', function (callback) {
	return string_src("lastbuild.js", "angular.module('app').run(function ($rootScope) { $rootScope.builtOn = " + Date.now() + "; });")
		//TODO: re-implement a triggering mechanism for updating data
		//.pipe(gulp.dest("localdata/"))
});

gulp.task('cleanAndMark', function (callback) {
	runSequence('clean', 'mark-rebuild', callback);
});


gulp.task('rebuild', function (callback) {
	runSequence('cleanAndMark', ['content', 'libraries', 'less', 'application', 'templates'], callback);
});


gulp.task('default', function (callback) {
	runSequence('rebuild', ['watch'], callback);
});


gulp.task('watch', function () {

	gulp.watch(paths.contentFiles(), ['content-restart']);
	gulp.watch(paths.libFiles(), ['libraries-restart']);
	gulp.watch(paths.lessFiles(), ['less-restart']);
	gulp.watch(paths.applicationFiles(), ['application-restart']);
	gulp.watch(paths.templateFiles(), ['templates-restart']);

	if (gutil.env.withHost) {
		plugins.supervisor("localhost/startup.js", { watch: [dest], extensions: ["json"] });
	}

	return true;
});

gulp.task('content-restart', ['content'], function () {
	return gulp.src("package.json", { base: "./" })
	.pipe(gulp.dest(dest));
});

gulp.task('libraries-restart', ['libraries'], function () {
	return gulp.src("package.json", { base: "./" })
	.pipe(gulp.dest(dest));
});

gulp.task('less-restart', ['less'], function () {
	return gulp.src("package.json", { base: "./" })
	.pipe(gulp.dest(dest));
});

gulp.task('application-restart', ['application'], function () {
	return gulp.src("package.json", { base: "./" })
	.pipe(gulp.dest(dest));
});

gulp.task('templates-restart', ['templates'], function () {
	return gulp.src("package.json", { base: "./" })
	.pipe(gulp.dest(dest));
});


function string_src(filename, string) {
	var src = stream.Readable({ objectMode: true })
	src._read = function () {
		this.push(new gutil.File({ cwd: "", base: "", path: filename, contents: new Buffer(string) }))
		this.push(null)
	}
	return src
}

function modify(modifiers) {

	return through2.obj(function (file, encoding, done) {

		var stream = this;

		function applyModifiers(content) {
			(typeof modifiers === 'function' ? [modifiers] : modifiers).forEach(function (modifier) {
				content = modifier(content, file);
			});
			return content;
		}

		function write(data) {
			file.contents = new Buffer(data);
			stream.push(file);
			done();
		}

		if (file.isBuffer()) {
			write(applyModifiers(String(file.contents)));
		} else if (file.isStream()) {
			var buffer = '';
			file.contents.on('data', function (chunk) {
				buffer += chunk;
			});
			file.contents.on('end', function () {
				write(applyModifiers(String(buffer)));
			});
		}
	});
}


function wrapTemplate(data, file) {
	var string = str2js(data).replace('module.exports = ', '').replace(';', '');

	var parts = file.path.split(path.sep);
	var folder = parts[parts.length - 2];
	var file = parts[parts.length - 1];
	var name = file.replace('.hbs', '').replace('.html', '');

	if (folder === 'components') {
		string = "Ember.TEMPLATES['components/" + name + "'] = Ember.Handlebars.compile(" + string + ");"
	} else {
		string = "Ember.TEMPLATES['" + name + "'] = Ember.Handlebars.compile(" + string + ");"
		
	}
	
	return string;
}