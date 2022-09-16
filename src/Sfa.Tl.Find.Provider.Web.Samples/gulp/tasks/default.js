const { src } = require('gulp');

var gulp = require('gulp'),
    concat = require('gulp-concat'),
    minify = require('gulp-minify'),
    replace = require('gulp-replace');

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
            'node_modules/crypto-js/core.js',
            'node_modules/crypto-js/enc-base64.js',
            'node_modules/crypto-js/sha256.js',
            'node_modules/crypto-js/hmac.js',
            'node_modules/crypto-js/hmac-sha256.js',
            'node_modules/accessible-autocomplete/dist/accessible-autocomplete.min.js',
            'Frontend/src/fapJs/findProvider.js',
            'Frontend/src/fapJs/locationAutocomplete.js'
        ])
        .pipe(concat('findProvider.js'))
        .pipe(gulp.dest(paths.dist.JS));
});

gulp.task('fapTileJs', () => {
    return src([
            'node_modules/accessible-autocomplete/dist/accessible-autocomplete.min.js',
            'Frontend/src/fapJs/findProviderTile.js',
            'Frontend/src/fapJs/locationAutocomplete.js'
        ])
        .pipe(concat('findProviderTile.js'))
        .pipe(gulp.dest(paths.dist.JS));
});

gulp.task('css', () => {
    return src(paths.src.CSS)
        .pipe(replace('$assets-arrowupdown-png', '/assets/arrowupdown.png'))
        .pipe(replace(/\$assets-(.*)-woff2/g, '/assets/$1.woff2'))
        .pipe(replace(/\$assets-(.*)-woff/g, '/assets/$1.woff'))
        .pipe(gulp.dest(paths.dist.CSS));
});

gulp.task('favicon', () => {
    return src(paths.src.default + "/favicon.ico")
        .pipe(gulp.dest(paths.dist.default));
});