/// <binding BeforeBuild='dev' />

var gulp = require('gulp');

require('./gulp/tasks/default');
//require('./gulp/tasks/dev');

gulp.task('default', gulp.series('assets', 'css', 'js', 'fapJs', 'fapTileJs', 'favicon',
    (done) => {
        done();
    }));
//gulp.task('default', gulp.series('assets', 'js', 'favicon',
//    (done) => {
//        done();
//    }));


//gulp.task('dev', gulp.series('assets', 'dev.js', 'favicon',
gulp.task('dev', gulp.series('assets', 'css', 'js', 'fapJs', 'fapTileJs', 'favicon',
    (done) => {
        done();
    }));


// ReSharper disable CommentTypo
//gulp.task('devwatch', gulp.series('assets', 'dev.sass', 'dev.js', 'sitemap', 'dev.sass:watch',
// ReSharper restore CommentTypo
//    (done) => {
//        done();
//    }));