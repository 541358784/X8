#!/usr/bin/env node
var fs = require('fs');
var path = require('path');
var program = require('commander');
var Builder = require('./BuilderUtils');
var OAuth2Config = require('../oauth2.json');
var google = require('googleapis');

program
    .version('0.0.1')
    .usage('<spreadsheet-id> <unity-script-dir> <json-data-dir> <unity-script-manager-template> <manager-template-name> [options]')
    .option('-c, --hash [column]', 'Column to hash the final JSON')
    .option('-l, --list-only', 'Ignore headers and just list the values in arrays')
    .option('-b, --beautify', 'Json use beautify format')
    .parse(process.argv);

if (program.args.length < 3) {
    program.help();
}

var spreadsheetId = program.args[0];
var manTemplate = null;
var manTemplateName = undefined;
if (program.args.length > 3) {
    manTemplate = program.args[3];
}
if (program.args.length > 4) {
    manTemplateName = program.args[4];
}
var outputConfig = new Builder.OutputConfig(program.args[1], program.args[2], manTemplate, manTemplateName);

//queue to build
var gssClassList = [];
var running = false;

//use oauth2 mode
var GoogleSpreadsheet = require("google-spreadsheets");
var oauth2Client = new google.auth.OAuth2(OAuth2Config.CLIENT_ID, OAuth2Config.CLIENT_SECRET, OAuth2Config.REDIRECT_URL);

oauth2Client.setCredentials({
    access_token: 'DUMMY',
    expiry_date: 1,
    refresh_token: OAuth2Config.refresh_token,
    token_type: 'Bearer'
});

oauth2Client.getAccessToken(function(err, token) {
    if (err) {
        //done('Google OAuth2 Error: ' + err);
        throw err;
    } else {
        // done(null, {
        //     type: 'Bearer',
        //     token: token
        // });
        //console.log('1: ', token);

        spreadsheet = GoogleSpreadsheet({
            key: spreadsheetId,
            auth: oauth2Client
        }, function(err, spreadsheet) {
            if (err) {
                throw err;
            }
            //console.log('Begin: ', spreadsheet);
            runOAuth2(spreadsheet);
        });
        console.log('End...');
    }
});

//NOTE: use spreadsheet
function runOAuth2(spreadsheet) {
    if(running) {
        console.log('already running...');
        return;
    }
    running = true;

    Builder.Cleanup();

    var countSheet = spreadsheet.worksheets.length;
    console.log('google spreadsheet: the worksheet has ', countSheet, ' sheets. begin reading...');
    readsheetOAuth2(spreadsheet.worksheets, 0, countSheet);
}

//NOTE: use spreadsheet NOT spreadsheets

function transpositionTable(rows){
    var nrow = 0;
    var ncol = 0;
    nrow = rows.length;
    ncol = rows[0].length;
    var newRows = new Array(ncol);

    for(var c = 0;c<ncol;++c) {
        newRows[c] = [];
    }
    for(var c = 0;c<ncol;++c) {
        for(var r = 0;r<nrow;++r){
            var cell = rows[r][c];
            if(cell==null){
                cell = {row:r,col:c,value:""};
                //console.log("+++++++++++",r,c);
            }
            if(newRows[cell.col-1][cell.row - 1]==null){
                newRows[cell.col-1][cell.row - 1] = cell;
                if(cell!=null && cell.row !=null){
                    var tmp = cell.row;
                    cell.row = cell.col;
                    cell.col = tmp;
                }
            }
            //var cell = newRows[c][r] = rows[r][c];
        }
    }
    return newRows;
}

function readsheetOAuth2(worksheets, idxSheet, countSheet) {
    var sheetName = worksheets[idxSheet].title;

    console.log('google spreadsheet: read the ', idxSheet, ' ', 'sheet: ', sheetName);
    //console.log('sheet: ', worksheets[idxSheet]);

    var jsTemplate = requireJsTemplate();
    var customerJs = requireCustomerJs();
    var bTranspositionTable = false;
    if(customerJs.TranspositionTable && customerJs.TranspositionTable[sheetName]){
        bTranspositionTable = true;
    }

    var jsTemplateMap = {};

    //console.log(jsTemplate);
    worksheets[idxSheet].cells(null, function(err, cells) {
        if (err) {
            throw err;
        }

        //program.vertical = true;
        var rowProp = program.vertical ? "col" : "row";
        var colProp = program.vertical ? "row" : "col";

        var rows = [];
        var datas = cells['cells'];
        for (key in datas) {
            var rowIndex = key - 1;
            if (typeof rows[rowIndex] === "undefined") {
                rows[rowIndex] = [];
            }
            var rowdata = datas[key];
            for (cKey in rowdata) {
                rows[rowIndex].push(rowdata[cKey]);
            };
        };

        //console.log(rows);
        //console.log("===============")
        if(bTranspositionTable){
            rows = transpositionTable(rows);
        }

        rows.forEach(function(col) {
            col.sort(function(cell1, cell2) {
                return cell1[colProp] - cell2[colProp];
            });
        });

        var isHashed = program.hash && !program.listOnly;
        var finalList = isHashed ? {} : [];
        var properties = rows[0].reduce(function(properties, cell) {
            if (cell.value === "") {
                return properties;
            }

            properties[cell[colProp]] = cell.value
                //.toLowerCase()
                .replace(/[- ]/ig, " ")
                .split(" ")
                .map(function(val, index) {
                    return !index ? val : val.charAt(0).toUpperCase() + val.slice(1);
                })
                .join("");

            return properties;
        }, {});

        var valueTypes = rows[1].reduce(function(valueTypes, cell) {
            if (cell.value === "") {
                return valueTypes;
            }
            //valueTypes[cell[colProp]] = cell.value.toUpperCase();
            var property = properties[cell[colProp]];
            valueTypes[property] = cell.value;
            return valueTypes;
        }, {});

        //console.log("rows[1]",rows[1]);
        //console.log("valueTypes:",valueTypes);
        //console.log('description flag: ', rows[2][0].value);
        var descriptions = rows[2].reduce(function(descriptions, cell) {
            if (cell.value === "") {
                return descriptions;
            }

            var property = properties[cell[colProp]];
            descriptions[property] = cell.value.toUpperCase();

            return descriptions;
        }, {});

        var fixedVT = {};
        for(var property in valueTypes) {
            fixedVT[property] = valueTypes[property];
        }

        // console.log("valueTypes:",valueTypes);
        rows.splice(0, 3);
        rows.forEach(function(col) {

            var newObject = program.listOnly ? [] : {};
            var hasValues = false;

            col.forEach(function(cell) {
                var val = "";

                var property = properties[cell[colProp]];
                var vtUpper = valueTypes[property].toUpperCase();
                //console.log("type:",cell[colProp],vtUpper);
                if(jsTemplate[vtUpper] != null) {
                    jsTemplateMap[valueTypes[property]] = jsTemplate[vtUpper];
                    val = jsTemplate[vtUpper].Parse(cell.value);
                    fixedVT[property] = jsTemplate[vtUpper].getClassName() || fixedVT[property];
                    hasValues = true;
                } else if (typeof cell.numericValue !== "undefined") {
                    val = parseFloat(cell.numericValue);
                    hasValues = true;
                    console.log("number:",val);
                } else if (vtUpper === "BOOL") {
                    if( cell.value.toUpperCase() === "TRUE") {
                        val = true;
                    } else {
                        val = false;
                    }
                    hasValues = true;
                } else if (vtUpper == "DOUBLE" || vtUpper == "FLOAT"){
                    val = parseFloat(cell.value);
                    hasValues = true;
                } else if(vtUpper === "NUMBER" || vtUpper == "INT"){
                    val = parseInt(cell.value);
                    hasValues = true;
                } else if (vtUpper === "STRING"){
                    val = cell.value.trim();
                    hasValues = true;
                } else if (vtUpper.indexOf("ENUM") == 0){
                    val = cell.value.trim();
                    hasValues = true;
                } else if (cell.value !== "") {
                    //console.log(cell.value);
                    //console.log(cell);
                    val = JSON.parse(cell.value);
                    hasValues = true;
                }

                var colNumber = cell[colProp];

                if (program.listOnly) {
                    newObject[colNumber - 1] = val;
                } else {
                    newObject[property] = val;
                }
            });

            if (isHashed) {
                finalList[newObject[program.hash]] = newObject;
            } else {
                finalList.push(newObject);
            }
        });


        for(var jsClassIdx in jsTemplateMap) {
            var jsClass = jsTemplateMap[jsClassIdx];
            if(jsClass && jsClass.needExport()) {
                Builder.buildJsTemplateClass(jsClassIdx, jsClass.getFields(), outputConfig);
            }
        }

        //console.log(sheetName);
        var gssClass = new Builder.GssClass(sheetName, properties, fixedVT, descriptions, finalList);
        gssClassList[gssClassList.length] = gssClass;
        if(idxSheet < countSheet - 1) {
            readsheetOAuth2(worksheets, idxSheet + 1, countSheet);
        } else {
            Builder.buildSpreadsheet(gssClassList, outputConfig);
            Builder.buildSpreadsheetToJson(gssClassList, outputConfig);
            running = false;
            console.log('google spreadsheet finished!');
        }

    });
}

function requireJsTemplate(){
    console.log("requireJsTemplate")
    var jsTemplate = {};
    var files = fs.readdirSync("./JsTemplate");
    for(var index in files){
        console.log(files[index]);
        if(!files[index].endsWith(".js")) {
            continue;
        }
        var name = path.basename(files[index], ".js");
        var template = require(path.join("../JsTemplate", name));
        var md = new template ();
        jsTemplate[name] = md;
        jsTemplate[name.toUpperCase()] = md;
    }
    return jsTemplate;
}

function requireCustomerJs(){
    console.log("requireCustomerJs")
    var customerJsArr = {};
    var files = fs.readdirSync("./CustomerJs");
    for(var index in files){
        console.log(files[index])
        var name = path.basename(files[index],".js");
        var md = require(path.join("../CustomerJs",name));
        customerJsArr[name] = md;
        customerJsArr[name.toUpperCase()] = md;
    }
    return customerJsArr;
}
