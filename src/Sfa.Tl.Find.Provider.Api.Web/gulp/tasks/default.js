const { src } = require('gulp');

var gulp = require('gulp'),
    concat = require('gulp-concat'),
    minify = require('gulp-minify');

const paths = require('../paths.json');

gulp.task('assets', () => {
    return src(paths.src.Assets)
        .pipe(gulp.dest(paths.dist.Assets));
});

gulp.task('js', () => {
    return src([
        (paths.src.JS)
    ])
        .pipe(gulp.dest(paths.dist.JS));
});

gulp.task('fapJs', () => {
    return src([
            'Frontend/src/fapJs/findProvider.js',
            'Frontend/src/fapJs/locationAutocomplete.js',
            'node_modules/accessible-autocomplete/dist/accessible-autocomplete.min.js',
            'node_modules/crypto-js/core.js',
            'node_modules/crypto-js/enc-base64.js',
            'node_modules/crypto-js/sha256.js',
            'node_modules/crypto-js/hmac.js',
            'node_modules/crypto-js/hmac-sha256.js'
        ])
        .pipe(concat('fap.js'))
        .pipe(gulp.dest(paths.dist.JS));
});

//gulp.task('fapJs', () => {
//    return src([
//            'Frontend/src/fapJs/findProvider.js',
//            'Frontend/src/fapJs/locationAutocomplete',
//            'node_modules/crypto-js/core.js',
//            'node_modules/crypto-js/enc-base64.js',
//            'node_modules/crypto-js/sha256.js',
//            'node_modules/crypto-js/hmac.js',
//            'node_modules/crypto-js/hmac-sha256.js',
//            'node_modules/accessible-autocomplete/dist/accessible-autocomplete.min.js'
//            //(paths.src.fapJS)
//        ])
//        .pipe(concat('fap.js'))
//        .pipe(gulp.dest(paths.dist.JS));
//});

gulp.task('css', () => {
        return src(paths.src.CSS)
            .pipe(gulp.dest(paths.dist.CSS));
});

gulp.task('favicon', () => {
    return src(paths.src.default + "/favicon.ico")
        .pipe(gulp.dest(paths.dist.default));
});