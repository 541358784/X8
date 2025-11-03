var program = require('commander');
var fs = require('fs')
var path = require('path')

//exports
exports.rmDir = rmDir = function(dirPath) {
	try {
		if(! fs.existsSync(dirPath)) {
			return;
		}
		var files = fs.readdirSync(dirPath);
	} catch (e) {
		console.error(e);
		return;
	}
	if (files.length > 0) {
		for (var i = 0; i < files.length; i++) {
			var filePath = dirPath + '/' + files[i];
			if (fs.statSync(filePath).isFile()) {
				fs.unlinkSync(filePath);
			} else {
				rmDir(filePath);
			}
		}
	}
	fs.rmdirSync(dirPath);
};


//make directory recursive
exports.mkDir = mkDir = function(dirPath) {
	//console.log("mkDir is called, path : ", dirPath);
	try {
		if(fs.existsSync(dirPath)) {
			return;
		}
		mkDir(path.dirname(dirPath));
		fs.mkdirSync(dirPath);
	} catch(e) {
		console.error(e);
		return;
	}
}

exports.cleanMetaDir = cleanMetaDir = function(dirPath) {
	try {
		if(! fs.existsSync(dirPath)) {
			return;
		}
		var files = fs.readdirSync(dirPath);
	} catch (e) {
		console.error(e);
		return;
	}
	if (files.length > 0) {
		for (var i = 0; i < files.length; i++) {
			var filePath = dirPath + '/' + files[i];
			if (fs.statSync(filePath).isFile()) {
				//console.log('del file:  ', filePath);
				if(path.extname(files[i]) != '.meta'){
					fs.unlinkSync(filePath);
				}
			} else {
				cleanMetaDir(filePath);
			}
		}
	}
};



//main
program
    .version('0.0.1')
    .usage('<directory-path>')
    .option('-r, --rm [rm]', 'rm directory')
    .option('-m, --mk [mk]', 'mk directory')
    .option('-e, --meta [meta]', 'clean besides meta')
    .parse(process.argv);

//console.log('args.length = ', program.args.length);
//console.log('args[0] = ', program.args[0]);

if (program.args.length < 1) {
    program.help();
}

var cDir = program.args[0];
var isRm = false;
var isMk = false;
var isMeta = false;
if (program.rm) {
	isRm = true;
} 
if(program.mk) {
	isMk = true;
}
if(program.meta) {
	isMeta = true;
}

if(isMeta) {
	console.log('meta ', cDir);
	cleanMetaDir(cDir);
} else if(isRm) {
	console.log('rm ', cDir);
	rmDir(cDir);
}

if(isMk) {
	console.log('mk ', cDir);
	mkDir(cDir);
}
