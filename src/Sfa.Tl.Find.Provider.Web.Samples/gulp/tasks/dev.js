const { src } = require('gulp');

var gulp = require('gulp');
var concat = require('gulp-concat');

const paths = require('../paths.json');

gulp.task('dev.js', () => {
    return gulp.src([
        //'node_modules/jquery/dist/jquery.js',
        (paths.src.JS)
    ])
        .pipe(gulp.dest(paths.dist.JS));
});
