/// <binding BeforeBuild='default' />

var gulp = require('gulp');

require('./gulp/tasks/default');

gulp.task('default', gulp.series('assets', 'sass', 'govjs', 'js', 'session-timeout-js', 'sitemap',
    (done) => {
        done();
    }));