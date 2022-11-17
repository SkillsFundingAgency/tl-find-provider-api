﻿const { src } = require('gulp');

var gulp = require('gulp'),
    concat = require('gulp-concat'),
    minify = require('gulp-minify'),
    sass = require('gulp-sass')(require('node-sass')),
    wait = require('gulp-wait'),
    purgecss = require('gulp-purgecss');

const paths = require('../paths.json');
const sassOptions = require('../sassOptions.js');

gulp.task('assets', () => {
    return src([
        'node_modules/govuk-frontend/govuk/assets/**/*',
        (paths.src.Assets)
    ])
        .pipe(gulp.dest(paths.dist.Assets));
});

gulp.task('js', () => {
    return src([
        (paths.src.JS),
        '!Frontend/src/js/session-timeout.js'
    ])
        .pipe(concat('tlevels.js'))
        .pipe(minify({
            noSource: true,
            ext: {
                min: '.min.js'
            }
        }))
        .pipe(gulp.dest(paths.dist.JS));
});

gulp.task('govjs', () => {
    return src([
        'node_modules/jquery/dist/jquery.min.js',
        'node_modules/govuk-frontend/govuk/all.js',
        'node_modules/html5shiv/dist/html5shiv.js',
    ])
        .pipe(gulp.dest(paths.dist.JS));
});

gulp.task('session-timeout-js', function () {
    return src([
        'Frontend/src/js/session-timeout.js'
    ])
        .pipe(concat('session-timeout.js'))
        .pipe(minify({
            noSource: true,
            ext: {
                min: '.min.js'
            }
        }))
        .pipe(gulp.dest(paths.dist.JS));
});

gulp.task('sass', () => {
    return src(paths.src.SCSS)
            .pipe(wait(200))
            .pipe(sass(sassOptions))
            .pipe(gulp.dest(paths.dist.SCSS));
});

gulp.task('sitemap', () => {
    return src(paths.src.Assets + "sitemap.xml")
        .pipe(gulp.dest(paths.dist.default));
});