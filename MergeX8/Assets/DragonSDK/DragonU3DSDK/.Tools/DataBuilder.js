console.log("build excel data .. ");

var sys = require("sys");
var fs = require("fs")
var path = require("path")
var XLSX = require('xlsx')
var ncp = require('ncp').ncp;
var Handlebars = require('handlebars');

var outputConfig = require('./outputConfig.json')

Handlebars.registerHelper("quotes", function(str) {
	return str.replace(/\"/gi, "\\\"");
});

function Klass(filename, path) {
	this.typeName = filename.substring(0, filename.lastIndexOf("."));
	this.fklassName = upperFirstWord(this.typeName);
	this.filename = filename;
	this.dataFileName = this.typeName.toLowerCase() + ".txt"
	this.klassName = this.typeName;
	this.klassName = this.klassName.replace(/(?=\b)\w/g, function(e) {
		return e.toUpperCase();
	});;
	this.klassFilePath = path;
	// console.log("类文件 : " + this.klassName);
}

function build() {
	for (var i = 0; i < klassList.length; i++) {
		var klass = klassList[i];
		buildCode(klass, true)
	};
	for (var i = 0; i < klassListServer.length; i++) {
		var klass = klassListServer[i];
		buildCode(klass, false)
	};
	var content = Handlebars.compile(fs.readFileSync("./javaTableManager.template", "utf-8"));
	fs.writeFileSync(filepath + "TableManager.java", content({
		klassList: klassListServer
	}), "utf-8");


	var content = Handlebars.compile(fs.readFileSync("./csharpTableManager.template", "utf-8"));
	fs.writeFileSync(csharpFillpath + "TableManager.cs", content({
		klassList: klassList
	}), "utf-8");
}

function buildEnumFile(klass, sheet) {

	var lineContent = [];
	var lineContent2 = [];
	for (line in sheet) {
		if (line[0] === '!') continue;
		var value = "" + sheet[line].v;
		var x = to10Radix(carveEndNum(line));
		var y = carveStarStr(line) - 4;
		/*
		 	x 0; y 128; value 129
			x 1; y 128; value QUEST_NOT_AVAILABLE
			x 2; y 128; value exception_129
			x 3; y 128; value 129号异常*/
		// console.log("x " + x + "; y " + y + "; value " + value);
		if (y >= 0 && value && value != "undefined" && value.length > 0) {
			if (!lineContent[y]) {
				lineContent[y] = [];
				lineContent2[y] = {};
			}
			lineContent[y][x] = value;
			if (x == 0) {
				lineContent2[y].id = value;
			}
			if (x == 1) {
				lineContent2[y].name = value;
			}
			if (x == 2) {
				lineContent2[y].enumname = value;
			}
			if (x == 3) {
				lineContent2[y].msg = value ? value.replace(/\"/gi, "\\\"") : "";
			}
		}

	}

	var content = Handlebars.compile(fs.readFileSync("./javaTableEnum.template", "utf-8"));
	var tCSharpContent = Handlebars.compile(fs.readFileSync("./csharpTableEnum.template", "utf-8"));


	var codefile = filepath + "T" + klass.klassName + ".java";
	fs.writeFileSync(codefile, content({
		klassName: klass.klassName,
		lineContent: lineContent2
	}), "utf-8");

	var codefileCSharp = csharpFillpath + "T" + klass.klassName + ".cs";
	fs.writeFileSync(codefileCSharp, tCSharpContent({
		klassName: klass.klassName,
		lineContent: lineContent2
	}), "utf-8");

}

function Field() {
	this.fieldname = "";
	this.fieldnameUpder = "";
	this.fieldnameUpderFirstWord = "";

	this.idIndex = 0; //field顺序

	this.type = "";
	this.typeUpperCase = "";
	this.isINT = true;
	this.retType = "int";

	this.desc = "";
	this.isArray = false;
	this.arrayFieldname = "";
	this.arrayFieldnameUpderFirstWord = "";
	this.arrayIndex = 0;
	this.arrayLength = 1;

	this.setFieldname = function(name) {
		this.fieldname = name;
		this.fieldnameUpder = this.fieldname.toUpperCase();
		this.fieldnameUpderFirstWord = upperFirstWord(this.fieldname);
	}
	this.setArrayFieldname = function(name) {
		this.arrayFieldname = name;
		this.arrayFieldnameUpderFirstWord = upperFirstWord(this.arrayFieldname);
	}

	this.setType = function(t) {
		this.type = t;
		this.typeUpperCase = this.type.toUpperCase();
		this.isINT = this.typeUpperCase == "INT";
		if (t == "INT") {
			this.retType = "int";
		} else {
			this.retType = "String";
		}
	}
}
var buildError = [];

function addError(table, msg) {
	console.log(msg);
	buildError[buildError.length] = table + " -> " + msg
}

function buildCode(klass, isClient) {
	console.log("解析 " + klass.filename);


	var enumType = false;
	if (klass.filename.match("enum_")) {
		enumType = true;
	}

	var xlsx = XLSX.readFile(klass.klassFilePath);
	var sheet = xlsx.Sheets[xlsx.SheetNames[0]];

	if (enumType) {
		buildEnumFile(klass, sheet);
	}
	var ids = {};

	function checkId(id) {
		if (parseInt(id) == NaN) {
			addError(klass.filename, "ID 必须为数字。id = " + id);
			return false;
		}

		if (empty(id)) {
			addError(klass.filename, "ID 为空。");
			return false;
		}
		if (ids[id] == 1) {
			addError(klass.filename, "ID 重复。id = " + id);
			return false;
		}

		ids[id] = 1;
		return true;
	}


	function empty(v) {
		switch (typeof v) {
			case 'undefined':
				return true;
			case 'string':
				var val = v.replace(/(^\s*)|(\s*$)/g, "");
				if (val.length == 0) return true;
				break;
			case 'boolean':
				if (!v) return true;
				break;
			case 'number':
				return false;
			case 'object':
				if (null === v) return true;
				if (undefined !== v.length && v.length == 0) return true;
				for (var k in v) {
					return false;
				}
				return true;
				break;
		}
		return false;
	}

	var classType, dataFileName;

	var fieldArray = [];
	var dataArray = [];
	var lastX = -1,
		lastY = -1;
	for (line in sheet) {
		if (line[0] === '!') continue;
		var index = to10Radix(carveEndNum(line));
		var indexY = carveStarStr(line);

		if (sheet[line].v === undefined) {
			continue;
		}
		var value = "" + sheet[line].v;

		if (!fieldArray[index]) {
			if (empty(sheet[line].v)) {
				continue;
			}
			fieldArray[index] = new Field();
			fieldArray[index].idIndex = index;
		}
		if (indexY == 1) {
			if (index == 1) {
				fieldArray[index].setFieldname("ignore_" + value);
			} else {
				fieldArray[index].setFieldname(value);
			}
		} else if (indexY == 2) {
			fieldArray[index].setType(value);
		} else if (indexY == 3) {
			fieldArray[index].desc = value;
		} else {
			var x = index;
			var y = indexY - 4;
			if (!dataArray[y] && x == 0) { //新行				
				if (!checkId(sheet[line].v)) { //ID检查出现问题，结束解析
					addError(klass.filename, "!!! Table Error(ID检查失败): line=" + line + "; ");
					console.log("可能会导致读取异常，需要检查表文件.\n\n");
					break;
				}
				dataArray[y] = [];
			}
			if ((x == 0 && y == lastY + 1) || (y == lastY && x == lastX + 1)) { //顺序下一个
				if (empty(sheet[line].v)) {
					if (x < fieldArray.length && dataArray[y]) {
						console.log("!!! Table Error. defalut Table value empty: line=" + line + "; \n");
						value = "0"; //default value;
					} else {
						continue;
					}
				}
			} else {
				if (dataArray[y] && !(lastX == 0 && x == 2)) { //备注名为空可以忽略其他为跳行
					// console.log("\nx = " + x + " y =" + y + "  lastY = " + lastY + " lastX = " + lastX);
					addError(klass.filename, "!!! Table Error(默认值为空): line=" + line + "; ");
					console.log("可能会导致读取异常，需要检查表文件.\n\n");
				} else {
					// console.log("多余字段: line=" + line + ",");
					// continue;
				}
			}
			if (dataArray[y]) {
				dataArray[y][x] = value;
			}
			lastY = y;
			lastX = x;
		}
	}
	// if(klass.filename.indexOf("quest") >= 0) {
	// console.log(JSON.stringify(fieldArray) + "\n");
	// }
	// sys.print("---------------\n" + JSON.stringify(dataArray) + "\n");

	for (var i = 0; i < fieldArray.length; i++) {
		if (i === 1) continue;

		var field = fieldArray[i];
		var fname = carveEndNum(field.fieldname);
		for (var j = i + 1; j < fieldArray.length; j++) {
			var targetField = fieldArray[j];
			if (targetField.isArray) {
				continue;
			}
			var tname = targetField.fieldname;
			if (startsWith(tname, fname)) {
				tname = tname.replace(fname, "");
				tname = tname.replace('_', "");
				if (isNumber(tname)) {
					//isArray
					field.isArray = true;
					field.setArrayFieldname(fname);
					field.arrayLength += 1;
					targetField.isArray = true;
					targetField.arrayIndex = field.arrayLength - 1;
					targetField.setArrayFieldname(fname);
				}
			}
		}
	}

	//update array field length
	for (var i = 0; i < fieldArray.length; i++) {
		var field = fieldArray[i];
		var fname = field.arrayFieldname;
		if (field.isArray) {
			for (var j = i + 1; j < fieldArray.length; j++) {
				var targetField = fieldArray[j];
				if (targetField.isArray && targetField.arrayFieldname == fname) {
					targetField.arrayLength = field.arrayLength;
				}
			}
		}
	}



	var content = Handlebars.compile(fs.readFileSync("./javaTableCode.template", "utf-8"));
	var contentCSharp = Handlebars.compile(fs.readFileSync("./CSharpCode.template", "utf-8"));

	if (!isClient) {
		var codefile = filepath + "Table_" + klass.klassName + ".java";
		fs.writeFileSync(codefile, content({
			klassName: klass.klassName,
			dataFileName: klass.dataFileName,
			fieldArray: fieldArray
		}), "utf-8");
	}

	if (isClient) {
		fieldArray.splice(1, 1);
		var codefileCSharp = csharpFillpath + "Table_" + klass.fklassName + ".cs";
		fs.writeFileSync(codefileCSharp, contentCSharp({
			klassName: klass.fklassName,
			dataFileName: klass.dataFileName,
			fieldArray: fieldArray
		}), "utf-8");
	}
	// console.log(codefileCSharp);
	var dataStr = "";
	var dataCsharpStr = "";
	for (var i = 0; i < dataArray.length; i++) {

		var line = dataArray[i];
		// sys.print("---------------\n" + JSON.stringify(line) + "\n");
		if (line) {
			dataStr += line.join("\t") + "\n";
			line.splice(1, 1);
			dataCsharpStr += line.join("\t") + "\n";
		}
	};
	var dataFile = dataPath + klass.klassName.toLowerCase() + ".txt";
	var dataCSharpFile = dataCSharpPath + klass.klassName.toLowerCase() + ".txt";
	if (!isClient) {
		fs.writeFileSync(dataFile, dataStr, "utf-8");
	}
	if (isClient) {
		fs.writeFileSync(dataCSharpFile, dataCsharpStr, "utf-8");
	}
	// console.log("数据文件： " + klass.klassName.toLowerCase() + ".txt");

}

function to10Radix(data) {
	var ret = 0;
	for (var i = 0; i < data.length; i++) {
		ret += i * 26 + data[i].charCodeAt() - 'A'.charCodeAt();
	}
	return ret;
}

function upperFirstWord(val) {
	// val = val.toLowerCase();
	var ret = "";
	var upper = true;
	for (var i = 0; i < val.length; i++) {
		var c = val[i];
		if (upper) {
			upper = false;
			ret += c.toUpperCase();
		} else {
			if (c == '_') {
				upper = true;
			} else {
				ret += c;
			}
		}
	}
	return ret;
}

/**数值判断。允许数值面前增加+ -号，允许存在一个小数点。输入非法字符返回0
 * 使用：isNumber(this)
 */
function isNumber(str) {
	var oneDecimal = false;
	var oneChar = 0;
	for (var i = 0; i < str.length; i++) {
		oneChar = str.charAt(i).charCodeAt(0);

		if (oneChar == 45) {
			if (i == 0) {
				continue;
			} else {
				return false;
			}
		}
		// 小数点 
		if (oneChar == 46) {
			if (!oneDecimal) {
				oneDecimal = true;
				continue;
			} else {
				return false;
			}
		}
		// 数字只能在0和9之间 
		if (oneChar < 48 || oneChar > 57) {
			return false;
		}
	}
	return true;
}

function startsWith(str, prefix) {
	return str.indexOf(prefix) === 0;
}

/**
 * 去掉末尾的数字和下划线
 */
function carveEndNum(str) {
	var index = str.length;
	for (var i = str.length - 1; i >= 0; i--) {
		if (isNumber(str.charAt(i))) { // || str.charAt(i) == '_'
			index--;
		}
	}
	return str.substring(0, index);
}

function carveStarStr(str) {
	var index = 0;
	for (var i = 0; i < str.length; i++) {
		if (!isNumber(str.charAt(i))) {
			index++;
		}
	}
	return str.substring(index);
}


rmDir = function(dirPath) {
	try {
		if(! fs.existsSync(dirPath)) {
			return;
		}
		var files = fs.readdirSync(dirPath);
	} catch (e) {
		console.error(e);
		return;
	}
	if (files.length > 0)
		for (var i = 0; i < files.length; i++) {
			var filePath = dirPath + '/' + files[i];
			if (fs.statSync(filePath).isFile())
				fs.unlinkSync(filePath);
			else
				rmDir(filePath);
		}
};

// make directory recursive
mkDir = function(dirPath) {
	//console.log("mkDir is called!");
	try{
		if(fs.existsSync(dirPath)) {
			//console.log("mkDir: alreay exist! name = " + dirPath);
			return;
		} else {
			//try to mkdir parent directory, then self
			mkDir(path.dirname(dirPath));
			console.log("mkDir: try to mkdir! name = " + dirPath);
			fs.mkdirSync(dirPath);
		}
		
	} catch(e) {
		console.log(e);
		return;
	}
};


///////////////////////////////////////////////////////////////////////////////////////////////

rmDir("./Public/");
rmDir("./CodeTable/CSharp/");
rmDir("./CodeTable/Java/");


// var cpstate_1 = false,
// 	cpstate_2 = false,
// 	cpstate_3 = false;
// ncp("./Datas/client/", "./Public/", function(err) {
// 	if (err) {
// 		return console.error(err);
// 	}
// 	cpstate_1 = true;
// 	cpOk();
// });
// ncp("./Datas/common/", "./Public/", function(err) {
// 	if (err) {
// 		return console.error(err);
// 	}
// 	cpstate_2 = true;
// 	cpOk();
// });
// ncp("./Datas/server/", "./Public/", function(err) {
// 	if (err) {
// 		return console.error(err);
// 	}
// 	cpstate_3 = true;
// 	cpOk();
// });



var klassList = [];
var klassListServer = [];
var filepath = "./CodeTable/Java/game/table/";
var csharpFillpath = "./CodeTable/CSharp/";
var dataPath = "./tmp_server/";
var dataCSharpPath = "./tmp_client/";

rmDir(filepath);
rmDir(csharpFillpath);
rmDir(dataPath);
rmDir(dataCSharpPath);

mkDir(filepath);
mkDir(csharpFillpath);
mkDir(dataPath);
mkDir(dataCSharpPath);

mkDir(outputConfig.tableFolderClient);
mkDir(outputConfig.tableFolderServer);
mkDir(outputConfig.classFolderClient);
mkDir(outputConfig.classFolderServer);

console.log('目录清空完毕!');

run();

function run() {
		var path = './Datas/client/';
		var xlsxFileList = fs.readdirSync(path);
		xlsxFileList.forEach(function(item) {
			setKlassList(item, 0, path + item);
		});

		var path = './Datas/common/';
		var xlsxFileList = fs.readdirSync(path);
		xlsxFileList.forEach(function(item) {
			setKlassList(item, 1, path + item);
		});

		var path = './Datas/server/';
		var xlsxFileList = fs.readdirSync(path);
		xlsxFileList.forEach(function(item) {
			setKlassList(item, 2, path + item);
		});

		doBuild();

	function setKlassList(item, type, path) {
		if (item[0] === '~' || item[0] === '.' || item.indexOf(".xlsx") < 0) {
			console.log("跳过 " + item);
			return;
		}
		var klass = new Klass(item, path);
		if (type == 0 || type == 1) {
			klassList[klassList.length] = klass;
		}
		if (type == 1 || type == 2) {
			klassListServer[klassListServer.length] = klass;
		}
	}



	var runing = false;

	function doBuild() {
		if (runing) return;
		runing = true;
		build();
		if (buildError.length > 0) {
			console.log("发现" + buildError.length + "个错误: \n");
			for (var i = 0; i < buildError.length; i++) {
				console.log(buildError[i]);
			};
		} else {
			console.log("解析完成");
		}
		//ncp(filepath, "../server/lszj/src/game/table/", function(err) {
		ncp(filepath, outputConfig.classFolderServer, function(err) {
			if (err) {
				return console.error(err);
			}
			console.log('复制JAVA文件至目标路径!');
		});
		ncp(csharpFillpath, outputConfig.classFolderClient, function(err) {
			if (err) {
				return console.error(err);
			}
			console.log('复制C#文件至目标路径!');
		});

		ncp(dataPath, outputConfig.tableFolderServer, function(err) {
			if (err) {
				return console.error(err);
			}
			console.log('复制txt文件至服务器!');
		});
		ncp(dataCSharpPath, outputConfig.tableFolderClient, function(err) {
			if (err) {
				return console.error(err);
			}
			console.log('复制txt文件至客户端!');
		});
	}

}