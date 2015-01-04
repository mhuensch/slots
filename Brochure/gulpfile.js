var rimraf = require('rimraf');
var runSequence = require('run-sequence');

var gulp = require('gulp');
var gutil = require('gulp-util');

var plugins = require("gulp-load-plugins")({
		pattern: ['gulp-*', 'gulp.*'],
		replaceString: /\bgulp[\-.]/
});

console.log('with host', gutil.env.withHost);

var useCompression = gutil.env.debug ? false : true;
if (!useCompression) {
	console.log("Building without compression");
}

var dest = gutil.env.dest || "localhost/brochure/bin";

gulp.task('brochure', function (callback) {
	return gulp.src("static/**/*")
	.pipe(gulp.dest(dest));
});


gulp.task('clean', function (callback) {
	return rimraf(dest, callback);
});


gulp.task('rebuild', function(callback) {
	runSequence('clean', ['brochure'], callback);

});


gulp.task('watch', function () {

	gulp.watch("static/**/*", ['content-restart']);
	if (gutil.env.withHost) {
		plugins.supervisor("localhost/startup.js", { watch: [dest], extensions: ["json"] });
	}
	
	return true;
});


gulp.task('default', function (callback) {
	runSequence('rebuild', ['watch'], callback);
});

