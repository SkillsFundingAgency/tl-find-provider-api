/// <binding BeforeBuild='dev' />

var gulp = require('gulp');

require('./gulp/tasks/default');
//require('./gulp/tasks/dev');

gulp.task('default', gulp.series('assets', 'css', 'js', 'cryptoJs', 'fapJs', 'fapTileJs', 'favicon',
    (done) => {
        done();
    }));

gulp.task('dev', gulp.series('assets', 'css', 'js', 'cryptoJs', 'fapJs', 'fapTileJs', 'favicon',
    (done) => {
        done();
    }));
