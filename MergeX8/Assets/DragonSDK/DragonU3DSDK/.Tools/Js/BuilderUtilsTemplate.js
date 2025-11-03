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

exports.OutputConfig = function(unityScriptDir, classTemplate) {
	this.unityScriptDir = unityScriptDir;
	this.classTemplate = classTemplate;
}


///////////////////////////////////// Google Sheets /////////////////////////////////////
//for google spreadsheet
exports.GssClass = function(filename, properties, valueTypes, descriptions, defaultValues, others, syncForceRemote) {
	this.typeName = filename;
	this.className = upperFirstWord(this.typeName);
	this.filename = filename;
	this.properties = properties;
	this.valueTypes = valueTypes;
	this.descriptions = descriptions;
	this.defaultValues = defaultValues;
	this.others = others;
	this.syncForceRemote = syncForceRemote;
}

exports.buildSpreadsheet = function(gssClassList, outputConfig) {
	//build
	gssClassList.forEach (function(gssClass) {
		buildGssClass(gssClass, outputConfig);
	});

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
		Tools.rmDir(unityScriptDir);
	});
}

function Field() {
	this.fieldName = "";
	this.idIndex = 0; //field顺序

	this.type = "";
	this.typeUpperCase = "";
	this.retType = "int";
	this.isStorage = false;
	this.isStorageList = false;
	this.realType = "";
	this.getterName = "";

	this.desc = "";

	this.defaultValue = undefined;
	this.defaultIsNew = false;
	this.hasDefaultValue = false;
	this.syncForce = false;
	this.syncForceRemote = false;
	this.isNormalType = false;
	this.isListNormalType = false;
	this.isDictionaryNormalType = false;
	this.isListStorageType = false;
	this.isDictionaryStorageType = false;
	this.isStorageType = false;
	this.isProtectedValue = false;
	this.isString = false;

	this.others = undefined;

	this.setDefaultValue = function(defaultValue) {
		this.defaultValue = defaultValue;
		if(defaultValue){
			this.syncForce = true;
		}
	}

	this.setSyncForceRemote = function(syncForceRemote) {
		if(syncForceRemote){
			this.syncForce = true;
			this.syncForceRemote = true;
		}
	}

	this.setFieldName = function(name) {
		this.fieldName = name;
		this.getterName = name[0].toUpperCase() + name.substring(1);
	}

	this.setType = function(t) {
		this.type = t;
		this.realType = t;
		this.typeUpperCase = this.type.toUpperCase();
		if (this.others == "protected")
		{
			this.isProtectedValue = true;
		}
		else if(this.realType == "uint" || this.realType == "ulong" || this.realType == "int" || this.realType == "long" || this.realType == "float" || this.realType == "double" || this.realType == "string" || this.realType == "bool")
		{
			this.isNormalType = true;
			if (this.realType == "string")
			{
				this.isString = true;
			}
		}
		else if (this.realType == "List<uint>" || this.realType == "List<ulong>" || this.realType == "List<int>" || this.realType == "List<long>" || this.realType == "List<float>" || this.realType == "List<double>" || this.realType == "List<string>" || this.realType == "List<bool>")
		{
			this.isListNormalType = true;
			this.realType = "Storage"+this.realType;
		}
		else if (this.realType == "Dictionary<string,uint>" || this.realType == "Dictionary<string,ulong>" || this.realType == "Dictionary<string,int>" || this.realType == "Dictionary<string,long>" || this.realType == "Dictionary<string,float>" || this.realType == "Dictionary<string,double>" || this.realType == "Dictionary<string,string>" || this.realType == "Dictionary<string,bool>" || 
			this.realType == "Dictionary<int,uint>" || this.realType == "Dictionary<int,ulong>" || this.realType == "Dictionary<int,int>" || this.realType == "Dictionary<int,long>" || this.realType == "Dictionary<int,float>" || this.realType == "Dictionary<int,double>" || this.realType == "Dictionary<int,string>" || this.realType == "Dictionary<int,bool>" || this.realType == "Dictionary<long,bool>")
		{
			this.isDictionaryNormalType = true;
			this.realType = "Storage"+this.realType;
		}
		else if (this.realType.indexOf("List") != -1 && this.realType.indexOf("Storage") != -1 )
		{
			this.isListStorageType = true;
			this.realType = "Storage"+this.realType;
		}
		else if (this.realType.indexOf("Dictionary") != -1 && this.realType.indexOf("Storage") != -1 )
		{
			this.isDictionaryStorageType = true;
			this.realType = "Storage"+this.realType;
		}
		else if (this.realType.indexOf("List") == -1 && this.realType.indexOf("Dictionary") == -1 &&  this.realType.indexOf("Storage") == 0)
		{
			this.isStorageType = true;
		}
		else
		{
			throw 'Error Type : ' + this.realType;
		}
	}
}
var buildError = [];

function addError(table, msg) {
	console.log(msg);
	buildError[buildError.length] = table + " -> " + msg
}

function buildGssClass (gssClass, outputConfig) {
	//console.log(gssClass.contentList);
	console.log("export google spreadsheet: " , gssClass.filename);
	var fieldArray = [];
	var useSplit = false;
	var useSplitSub = false;
	for (var i = 0; i < gssClass.properties.length; i++) {
		if (i == 0) {
			continue;
		}

		var property = gssClass.properties[i];
		console.log(" =========== propertys " );
		console.log(property);
		console.log(gssClass.valueTypes);
		var idxCol = i - 1;
		if (! fieldArray[idxCol]) {
			fieldArray[idxCol] = new Field();
			fieldArray[idxCol].idIndex = idxCol;
			fieldArray[idxCol].setFieldName(property);
			fieldArray[idxCol].desc = gssClass.descriptions[i];
			if (fieldArray[idxCol].desc)
			{
				fieldArray[idxCol].desc = fieldArray[idxCol].desc.toUpperCase();
			}
			fieldArray[idxCol].others = gssClass.others[i];
			fieldArray[idxCol].useSplitSub = false;
			fieldArray[idxCol].useSplit = false;
			fieldArray[idxCol].useDirtyFlag = false;
			fieldArray[idxCol].setType(gssClass.valueTypes[i]);
			fieldArray[idxCol].setDefaultValue(gssClass.defaultValues[i]);
			if (gssClass.syncForceRemote){
				fieldArray[idxCol].setSyncForceRemote(gssClass.syncForceRemote[i]);
			} else {
				fieldArray[idxCol].setSyncForceRemote(false);
			}
			
			console.log(fieldArray);
	}
}
	console.log(outputConfig.classTemplate);
	var contentUnity = Handlebars.compile(fs.readFileSync(outputConfig.classTemplate, "utf-8"));
	//class file
	var codeFile = unityScriptDir + gssClass.className + ".cs";
	var code = contentUnity({
		className: gssClass.className,
		fieldArray: fieldArray,
		useSplit: (useSplit||useSplitSub)
	});
	if (!fs.existsSync(unityScriptDir)){
		fs.mkdirSync(unityScriptDir);
	}
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