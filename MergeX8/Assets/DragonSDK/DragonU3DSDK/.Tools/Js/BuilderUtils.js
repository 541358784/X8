//console.log("build excel data .. ");

var fs = require("fs");
var path = require('path');
var ncp = require('ncp').ncp;
var Handlebars = require('handlebars');
var Tools = require("./Tools");

var exports = module.exports = {};

Handlebars.registerHelper("quotes", function(str) {
	return str.replace(/\"/gi, "\\\"");
});

var unityScriptDir = "./CodeTable/Unity/";
exports.Cleanup = function() {
	Tools.rmDir(unityScriptDir);
	Tools.mkDir(unityScriptDir);
	console.log('临时目录清空完毕!');
}

exports.OutputConfig = function(unityScriptDir, jsonDataDir, manTemplate, manTemplateName) {
	this.unityScriptDir = unityScriptDir;
	this.jsonDataDir = jsonDataDir;
	this.manTemplate = manTemplate;
	this.manTemplateName = manTemplateName;
	if(!this.manTemplate) {
		this.manTemplate = "./Template/UnityTableManager.template";
	}
}


///////////////////////////////////// Google Sheets /////////////////////////////////////
//for google spreadsheet
exports.GssClass = function(filename, properties, valueTypes, descriptions, contentList) {
	this.typeName = filename;
	this.className = upperFirstWord(this.typeName);
	this.filename = filename;
	this.dataFileName = this.typeName.toLowerCase();
	this.properties = properties;
	this.valueTypes = valueTypes;
	this.descriptions = descriptions;
	this.contentList = contentList;
}

exports.buildSpreadsheet = function(gssClassList, outputConfig) {
	//build
	gssClassList.forEach (function(gssClass) {
		buildGssClass(gssClass)
	});

	if(outputConfig.manTemplateName) {
		var content = Handlebars.compile(fs.readFileSync(outputConfig.manTemplate, "utf-8"));
		fs.writeFileSync(unityScriptDir + outputConfig.manTemplateName + ".cs", content({
			managerName: outputConfig.manTemplateName,
			classList: gssClassList
		}), "utf-8");
	}

	//check error
	if (buildError.length > 0) {
		console.log("发现" + buildError.length + "个错误: \n");
		for (var i = 0; i < buildError.length; i++) {
			console.log(buildError[i]);
		};
	} else {
		console.log("解析完成");
	}
		
	//copy
	mkDir(outputConfig.unityScriptDir);
		
	ncp(unityScriptDir, outputConfig.unityScriptDir, function(err) {
		if (err) {
			return console.error(err);
		}
		console.log('复制Unity Script文件至目标路径!');
	});
	//Cleanup();
}

function gBuildEnumFile(gssClass) {
	//console.log(gssClass);
	var idxValue = undefined;
	var idxKeyList = [];
	for(var key in gssClass.properties) {
		var property = gssClass.properties[key];
		var vtUpper = gssClass.valueTypes[property].toUpperCase();

		if(property.toUpperCase() == "ID") {
			idxValue = property;
		} else if(vtUpper == "ENUM") {
			//hit, make enum
			idxKeyList.push(property);
		}
	}

	for(var i = 0; i < idxKeyList.length; i++) {
		var idxKey = idxKeyList[i];
		var className = upperFirstWord(idxKey);
		var lineContent = [];

		for(var idxRow = 0; idxRow < gssClass.contentList.length; ++idxRow) {
			lineContent[idxRow] = {};
			lineContent[idxRow].value = gssClass.contentList[idxRow][idxValue];
			lineContent[idxRow].key = gssClass.contentList[idxRow][idxKey];
		}
		//console.log("lineContent: \n", lineContent);
		var unityContent = Handlebars.compile(fs.readFileSync("./Template/UnityEnum.template", "utf-8"));

		var codeFile = unityScriptDir + "Enum" + className + ".cs";
		fs.writeFileSync(codeFile, unityContent({
			className: className,
			lineContent: lineContent
		}), "utf-8");
	}
}

function Field() {
	this.fieldName = "";
	this.idIndex = 0; //field顺序

	this.type = "";
	this.typeUpperCase = "";
	this.retType = "int";
	this.desc = "";

	this.isArray = false;
	this.arrayFieldName = "";
	this.arrayIndex = 0;
	this.arrayLength = 1;

	this.setFieldName = function(name) {
		this.fieldName = name;
	}
	this.setArrayFieldName = function(name) {
		this.arrayFieldName = name;
	}

	this.setType = function(t) {
		this.type = t;
		this.typeUpperCase = this.type.toUpperCase();
		if (this.typeUpperCase == "INT" || this.typeUpperCase == "NUMBER") {
			this.retType = "int";
		} else if(this.typeUpperCase == "UINT") {
			this.retType = "uint";
		} else if(this.typeUpperCase == "LONG") {
			this.retType = "long";
		} else if(this.typeUpperCase == "ULONG") {
			this.retType = "ulong";
		} else if(this.typeUpperCase == "FLOAT") {
			this.retType = "float";
		} else if(this.typeUpperCase == "DOUBLE") {
			this.retType = "double";
		} else if(this.typeUpperCase == "STRING" || this.typeUpperCase == "ENUM") {
			this.retType = "string";
		} else if (this.typeUpperCase == "ARRAYNUMBER" || this.typeUpperCase == "ARRAYINT") {
			this.retType = "int[]";
		} else if(this.typeUpperCase == "ARRAYFLOAT") {
			this.retType = "float[]";
		} else if(this.typeUpperCase == "ARRAYDOUBLE") {
			this.retType = "double[]";
		} else if(this.typeUpperCase == "ARRAYSTRING") {
			this.retType = "string[]";
		} else {
			//this.retType = "object";
			this.retType = t;
		}
	}
}
var buildError = [];

function addError(table, msg) {
	console.log(msg);
	buildError[buildError.length] = table + " -> " + msg
}

function buildGssClass (gssClass) {
	//console.log(gssClass.contentList);
	console.log("export google spreadsheet: " , gssClass.filename);

	//build enum if property type is enum
	gBuildEnumFile(gssClass);

	var ids = {};

	function checkId(id) {
		if (parseInt(id) == NaN) {
			addError(gssClass.filename, "ID 必须为数字。id = " + id);
			return false;
		}

		if (empty(id)) {
			addError(gssClass.filename, "ID 为空。");
			return false;
		}
		if (ids[id] == 1) {
			addError(gssClass.filename, "ID 重复。id = " + id);
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

	var fieldArray = [];
	var dataArray = [];

	//console.log('property length: ', gssClass.properties.length);
	//console.log('property key: ', gssClass.properties.key);

	for(var idxRow = 0; idxRow < gssClass.contentList.length; ++idxRow) {
		for(key in gssClass.properties) {
			var property = gssClass.properties[key];
			var idxCol = key - 1;
			if (! fieldArray[idxCol]) {
				fieldArray[idxCol] = new Field();
				fieldArray[idxCol].idIndex = idxCol;
				fieldArray[idxCol].setFieldName(property);
				fieldArray[idxCol].setType(gssClass.valueTypes[property]);
				fieldArray[idxCol].desc = gssClass.descriptions[property];
			}
		}
	}
	//console.log(fieldArray);
	for (var i = 0; i < fieldArray.length; i++) {
		var field = fieldArray[i];
		for (var j = i + 1; j < fieldArray.length; j++) {
			var targetField = fieldArray[j];
			if (targetField.isArray) {
				continue;
			}
			if (field.type == targetField.type && isNumber(field.fieldName) && isNumber(targetField.fieldName)) {
				var arrayName = field.type.toLowerCase() + "s";
				field.isArray = true;
				field.setArrayFieldName(arrayName);
				field.arrayLength += 1;
				targetField.isArray = true;
				targetField.arrayIndex = field.arrayLength - 1;
				targetField.setArrayFieldName(arrayName);
			} else {
				break;
			}
		}
	}

	//update array field length
	for (var i = 0; i < fieldArray.length; i++) {
		var field = fieldArray[i];
		var arrayName = field.arrayFieldName;
		if (field.isArray) {
			for (var j = i + 1; j < fieldArray.length; j++) {
				var targetField = fieldArray[j];
				if (targetField.isArray && targetField.arrayFieldName == arrayName) {
					targetField.arrayLength = field.arrayLength;
				}
			}
		}
	}


	var contentUnity = Handlebars.compile(fs.readFileSync("./Template/UnityTable.template", "utf-8"));
	//class file
	var codeFile = unityScriptDir + "Table" + gssClass.className + ".cs";
	var code = contentUnity({
		className: gssClass.className,
		dataFileName: gssClass.dataFileName,
		fieldArray: fieldArray
	});
	fs.writeFileSync(codeFile, code, "utf-8");

	//console.log("gss: ", fieldArray);
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
	if(str == undefined) {
		return false;
	}

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

exports.joinDir = joinDir = function(dirPath, filename) {
	return path.join(dirPath, filename);
}

exports.buildSpreadsheetToJson = function(gssClassList, outputConfig){

	var findGssClassContentItem = function(propertyName,propertyValue,gssClass){

		var bHas = false;
		var truePropertyName = null;
		for(var k in gssClass.properties){
			var property = gssClass.properties[k];
			if(property.toUpperCase()==propertyName.toUpperCase()){
				truePropertyName = property;
				bHas = true;
				break;
			}
		}
		if(bHas){
			for(var row = 0;row<gssClass.contentList.length;++row){
				var content = gssClass.contentList[row];
				if(content && content[truePropertyName] == propertyValue){
					return content;
				}
			}
		}
		return null;
	};

	var sheetNameMap = {};
	for(var i = 0;i<gssClassList.length;++i){
		/**
		 * @type {GssClass}
		 */
		var gssClass = gssClassList[i];
		sheetNameMap[gssClass.typeName.toUpperCase()] = gssClassList[i];
	}

	gssClassList.forEach(function(gssClass){
		var valueTypes = gssClass.valueTypes;
		gssClass._vt = [];
		for(var col in gssClass.properties){
			gssClass._vt.push(col);

			var propertyName = gssClass.properties[col];
			var valueType = valueTypes[propertyName];
			var vtArray = valueType.split(":");
			if(vtArray.length == 2){
				var requireSheetName = vtArray[0];
				var requirePropertyName = vtArray[1].toLowerCase();
				var requireGssClass = sheetNameMap[requireSheetName.toUpperCase()];

				if(requireGssClass != null){
					for(var i = 0; i < gssClass.contentList.length; ++i){
						var content = gssClass.contentList[i];
						content[propertyName] = findGssClassContentItem(requirePropertyName, content[propertyName], requireGssClass);
					}
				}
			}
		}
	});

	gssClassList.forEach(function(gssClass){
		var valueTypes = gssClass.valueTypes;
		for(var i = 0; i < gssClass._vt.length; i++){
			var ii = gssClass._vt[i];
			var propertyName = gssClass.properties[ii];
			var valueType = valueTypes[propertyName];
			if(! valueType) {
				continue;
			}

			var content = [];
			for(var k = 0; k < gssClass.contentList.length; k++) {
				if(gssClass.contentList[k][propertyName]) {
					content.push([gssClass.contentList[k][propertyName]]);
				} else {
					content.push([]);
				}
			}

			var isFindArray = false;
			for(var j = i + 1; j < gssClass._vt.length; j++) {
				var jj = gssClass._vt[j];
				var jProperty = gssClass.properties[jj];
				if(valueType && valueTypes[jProperty] && valueTypes[jProperty] == valueType && isNumber(propertyName) && isNumber(jProperty)) {
					isFindArray = true;
					delete gssClass.valueTypes[jProperty];
					delete gssClass.properties[jj];
					for(var k = 0; k < gssClass.contentList.length; k++) {
						if(gssClass.contentList[k][jProperty]) {
							content[k].push(gssClass.contentList[k][jProperty]);
						}
						delete gssClass.contentList[k][jProperty];
					}
				}
			}
			if(isFindArray) {
				//array
				delete gssClass.valueTypes[propertyName];
				delete gssClass.properties[ii];
				var fieldName = valueType.toLowerCase() + "s";
				for(var k = 0; k < gssClass.contentList.length; k++) {
					delete gssClass.contentList[k][propertyName];
					gssClass.contentList[k][fieldName] = content[k];
				}
				gssClass.properties[fieldName] = fieldName;
			}
		}
	});


	gssClassList.forEach(function(gssClass){
		//console.log(gssClass.contentList);
		var program = require('commander');
		mkDir(outputConfig.jsonDataDir);
		var filename = joinDir(outputConfig.jsonDataDir, gssClass.typeName + '.json');
		console.log('Output json begin: ', filename);
		var json = JSON.stringify(gssClass.contentList, null, program.beautify ? 4 : null);
		fs.writeFile(filename.toLowerCase(), json, "utf-8", function(err) {
			if (err) {
				throw err;
			}
			//process.exit(0);
			console.log('Output json end: ', filename);
		});
	});

///////////////////////////////////////////////////////////////////////////////////////////////

}

exports.buildJsTemplateClass = function(className, fieldArray, outputConfig) {
	var scriptDir = unityScriptDir + "TableItems/";
	var dstScriptDir = outputConfig.unityScriptDir + "/TableItems/";

	mkDir(scriptDir);
	var contentUnity = Handlebars.compile(fs.readFileSync("./Template/UnityTableItem.template", "utf-8"));
	//class file
	var codeFile = scriptDir + className + ".cs";
	var fileContent = contentUnity({
		className: className,
		fieldArray: fieldArray
	});
	//console.log(codeFile);
	//console.log(fileContent);
	fs.writeFileSync(codeFile, fileContent, "utf-8");

	ncp(scriptDir, dstScriptDir, function(err) {
		if (err) {
			return console.error(err);
		}
		console.log('复制Unity Template Script文件至目标路径!');
	});
};
