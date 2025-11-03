#!/usr/bin/env node
var fs = require('fs');
var path = require('path');
var program = require('commander');
var Builder = require('./BuilderUtils');
var OAuth2Config = require('./oauth2.json');
var google = require('googleapis');

program
    .version('0.0.1')
    .usage('<spreadsheet-id> <client-script-dir> <client-data-dir> <server-script-dir> <server-data-dir> [options]')
    .option('-u, --user [user]', 'Google account to login')
    .option('-p, --password [password]', 'Password to login')
    .option('-t, --token [token]', 'Auth token acquired externally')
    .option('-y, --tokentype [tokentype]', 'Type of the informed token (defaults to Bearer)')
    .option('-c, --hash [column]', 'Column to hash the final JSON')
    .option('-o, --oauth2 [oauth2]', 'Use OAuth2, defaults config is oauth2.json')
    .option('-l, --list-only', 'Ignore headers and just list the values in arrays')
    .option('-j, --json [json]', 'Output json directory')
    .option('-b, --beautify', 'Json use beautify format')
    .option('-f, --filename [json]', 'Json filename')
    .option('-s, --sheet', 'Output sheet')
    .parse(process.argv);

if (program.args.length < 5) {
    program.help();
}

var spreadsheetId = program.args[0];
var outputConfig = new Builder.OutputConfig(program.args[1], program.args[2], program.args[3], program.args[4]);

//queue to build
var gssClassList = [];
var running = false;

if (program.token) {
    var tokentype = program.tokentype || "Bearer";
    spreadsheet.setAuthToken({
        value: program.token,
        type: tokentype
    });
    run();
} else if (program.oauth2) {
    var GoogleSpreadsheet = require("google-spreadsheets");
    var oauth2Client = new google.auth.OAuth2(OAuth2Config.CLIENT_ID, OAuth2Config.CLIENT_SECRET, OAuth2Config.REDIRECT_URL);
    /*
    oauth2Client.setCredentials({
        access_token: OAuth2Config.access_token,
        refresh_token: OAuth2Config.refresh_token
    });
*/

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
    //console.log(OAuth2Config);
   
} else if (program.user && program.password) {
    var GoogleSpreadsheet = require("google-spreadsheet");
    var spreadsheet = new GoogleSpreadsheet(spreadsheetId);
    spreadsheet.setAuth(program.user, program.password, function(err) {
        if (err) {
            throw err;
        }
        run(spreadsheet);
    });
} else {
    var spreadsheet;
    run(spreadsheet);
}

//NOTE: use spreadsheet
function runOAuth2(spreadsheet) {
    if(running) {
        console.log('already running...');
        return;
    }
    running = true;

    var countSheet = spreadsheet.worksheets.length;
    console.log('google spreadsheet: the worksheet has ', countSheet, ' sheets. begin reading...');
    readsheetOAuth3(spreadsheet.worksheets, 0, countSheet);
}

//NOTE: use spreadsheet NOT spreadsheets
function run(spreadsheet) {
    if(running) {
        console.log('already running...');
        return;
    }
    running = true;

    console.log(spreadsheet);

    spreadsheet.getInfo(function(err, sheet_info) {
        if (err) {
            console.log(err);
            throw err;
        }

        var countSheet = sheet_info.worksheets.length;
        console.log('google spreadsheet: the worksheet has ', countSheet, ' sheets. begin reading...');
        readsheet(sheet_info.worksheets, 0, countSheet);        
    });
}

function readsheet(worksheets, idxSheet, countSheet) {
        var sheetName = worksheets[idxSheet].title;
        console.log('google spreadsheet: read the ', idxSheet, ' ', 'sheet: ', sheetName);
        worksheets[idxSheet].getCells(function(err, cells) {
            if (err) {
                throw err;
            }

            var rowProp = program.vertical ? "col" : "row";
            var colProp = program.vertical ? "row" : "col";

            var rows = cells.reduce(function(rows, cell) {
                var rowIndex = cell[rowProp] - 1;
                if (typeof rows[rowIndex] === "undefined")
                    rows[rowIndex] = [];
                rows[rowIndex].push(cell);
                return rows;
            }, []);

            //console.log(rows);


            rows.forEach(function(col) {
                col.sort(function(cell1, cell2) {
                    return cell1[colProp] - cell2[colProp];
                });
            });

            var isHashed = program.hash && !program.listOnly;
            var finalList = isHashed ? {} : [];
            var properties = rows[0].reduce(function(properties, cell) {
                if (cell.value === "")
                    return properties;

                properties[cell[colProp]] = cell.value
                    .toLowerCase()
                    .replace(/[- ]/ig, " ")
                    .split(" ")
                    .map(function(val, index) {
                        return !index ? val : val.charAt(0).toUpperCase() + val.slice(1);
                    })
                    .join("");
                
                return properties;
            }, {});

            var valueTypes = rows[1].reduce(function(valueTypes, cell) {
                if (cell.value === "")
                    return valueTypes;

                valueTypes[cell[colProp]] = cell.value.toUpperCase();

                return valueTypes;
            }, {});

            console.log('description flag: ', rows[2][0].value);
            var descriptions = rows[2].reduce(function(descriptions, cell) {
                if (cell.value === "")
                    return descriptions;

                descriptions[cell[colProp]] = cell.value.toUpperCase();

                return descriptions;
            }, {});

            rows.splice(0, 3);
            rows.forEach(function(col) {

                var newObject = program.listOnly ? [] : {};
                var hasValues = false;

                col.forEach(function(cell) {
                    var val = "";

                    if (typeof cell.numericValue !== "undefined") {
                        val = parseFloat(cell.numericValue);
                        hasValues = true;
                    } else if (cell.value === "TRUE") {
                        val = true;
                        hasValues = true;
                    } else if (cell.value === "FALSE") {
                        val = false;
                        hasValues = true;
                    } else if (cell.value !== "") {
                        val = cell.value;
                        hasValues = true;
                    } 

                    var colNumber = cell[colProp];

                    if (program.listOnly)
                        newObject[colNumber - 1] = val;
                    else
                        newObject[properties[colNumber]] = val;
                });

                if (! hasValues) {
                    if (program.listOnly)
                        newObject[colNumber - 1] = "";
                    else
                        newObject[properties[colNumber]] = "";
                }

                if (isHashed)
                    finalList[newObject[program.hash]] = newObject;
                else
                    finalList.push(newObject);
            });

            //console.log(sheetName);
            var gssClass = new Builder.GssClass(sheetName, properties, valueTypes, descriptions, finalList);
            gssClassList[gssClassList.length] = gssClass;
            if(idxSheet < countSheet - 1) {
                readsheet(worksheets, idxSheet + 1, countSheet); 
            } else {
                Builder.buildSpreadsheet(gssClassList, outputConfig);
                running = false;
                console.log('google spreadsheet finished!');
            }

            /**
            var json = JSON.stringify(finalList, null, program.beautify ? 4 : null);
            fs.writeFile(filename, json, "utf-8", function(err) {
                if (err)
                    throw err;
                process.exit(0);
            }); **/

        });
}

function readsheetOAuth2(worksheets, idxSheet, countSheet) {
        var sheetName = worksheets[idxSheet].title;
        console.log('google spreadsheet: read the ', idxSheet, ' ', 'sheet: ', sheetName);
        //console.log('sheet: ', worksheets[idxSheet]);

        worksheets[idxSheet].cells(null, function(err, cells) {
            if (err) {
                throw err;
            }

            //console.log('cells: ', cells['cells'].keys);

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

            /**
            var rows = cells.cells.reduce(function(rows, cell) {
                var rowIndex = cell[rowProp] - 1;
                if (typeof rows[rowIndex] === "undefined")
                    rows[rowIndex] = [];
                rows[rowIndex].push(cell);
                return rows;
            }, []);
            **/
            //console.log(rows);

            rows.forEach(function(col) {
                col.sort(function(cell1, cell2) {
                    return cell1[colProp] - cell2[colProp];
                });
            });

            var isHashed = program.hash && !program.listOnly;
            var finalList = isHashed ? {} : [];
            var properties = rows[0].reduce(function(properties, cell) {
                if (cell.value === "")
                    return properties;

                properties[cell[colProp]] = cell.value
                    .toLowerCase()
                    .replace(/[- ]/ig, " ")
                    .split(" ")
                    .map(function(val, index) {
                        return !index ? val : val.charAt(0).toUpperCase() + val.slice(1);
                    })
                    .join("");
                
                return properties;
            }, {});

            var valueTypes = rows[1].reduce(function(valueTypes, cell) {
                if (cell.value === "")
                    return valueTypes;

                valueTypes[cell[colProp]] = cell.value.toUpperCase();

                return valueTypes;
            }, {});

            console.log('description flag: ', rows[2][0].value);
            var descriptions = rows[2].reduce(function(descriptions, cell) {
                if (cell.value === "")
                    return descriptions;

                descriptions[cell[colProp]] = cell.value.toUpperCase();

                return descriptions;
            }, {});

            rows.splice(0, 3);
            rows.forEach(function(col) {

                var newObject = program.listOnly ? [] : {};
                var hasValues = false;

                col.forEach(function(cell) {
                    var val = "";

                    if (typeof cell.numericValue !== "undefined") {
                        val = parseFloat(cell.numericValue);
                        hasValues = true;
                    } else if (cell.value === "TRUE") {
                        val = true;
                        hasValues = true;
                    } else if (cell.value === "FALSE") {
                        val = false;
                        hasValues = true;
                    } else if (cell.value !== "") {
                        val = cell.value;
                        hasValues = true;
                    } 

                    var colNumber = cell[colProp];

                    if (program.listOnly)
                        newObject[colNumber - 1] = val;
                    else
                        newObject[properties[colNumber]] = val;
                });

                if (! hasValues) {
                    if (program.listOnly)
                        newObject[colNumber - 1] = "";
                    else
                        newObject[properties[colNumber]] = "";
                }

                if (isHashed)
                    finalList[newObject[program.hash]] = newObject;
                else
                    finalList.push(newObject);
            });

            if(program.json && program.json !== "") {
                Builder.mkDir(program.json);
                var filename = Builder.joinDir(program.json, sheetName + '.json');
                console.log('Output json begin: ', filename);
                var json = JSON.stringify(finalList, null, program.beautify ? 4 : null);
                fs.writeFile(filename, json, "utf-8", function(err) {
                    if (err) {
                        throw err;
                    }
                    //process.exit(0);
                    console.log('Output json end: ', filename);
                });
            }

            //console.log(sheetName);
            var gssClass = new Builder.GssClass(sheetName, properties, valueTypes, descriptions, finalList);
            gssClassList[gssClassList.length] = gssClass;
            if(idxSheet < countSheet - 1) {
                readsheetOAuth2(worksheets, idxSheet + 1, countSheet); 
            } else {
                Builder.buildSpreadsheet(gssClassList, outputConfig);
                running = false;
                console.log('google spreadsheet finished!');
            }

        });
}

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

function readsheetOAuth3(worksheets, idxSheet, countSheet) {
        var sheetName = worksheets[idxSheet].title;

        console.log('google spreadsheet: read the ', idxSheet, ' ', 'sheet: ', sheetName);
        //console.log('sheet: ', worksheets[idxSheet]);

        var jsTemplate = requireJsTemplate();
        var customerJs = requireCustomerJs();
        var bTranspositionTable = false;
        if(customerJs.TranspositionTable && customerJs.TranspositionTable[sheetName]){
            bTranspositionTable = true;
        }
        //console.log(jsTemplate);
        worksheets[idxSheet].cells(null, function(err, cells) {
            if (err) {
                throw err;
            }

            //console.log('cells: ', cells['cells'].keys);

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
            //console.log(rows);
            /**
            var rows = cells.cells.reduce(function(rows, cell) {
                var rowIndex = cell[rowProp] - 1;
                if (typeof rows[rowIndex] === "undefined")
                    rows[rowIndex] = [];
                rows[rowIndex].push(cell);
                return rows;
            }, []);
            **/
            //console.log(rows);

            rows.forEach(function(col) {
                col.sort(function(cell1, cell2) {
                    return cell1[colProp] - cell2[colProp];
                });
            });

            var isHashed = program.hash && !program.listOnly;
            var finalList = isHashed ? {} : [];
            var properties = rows[0].reduce(function(properties, cell) {
                if (cell.value === "")
                    return properties;

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
                if (cell.value === "")
                    return valueTypes;


                valueTypes[cell[colProp]] = cell.value.toUpperCase();

                return valueTypes;
            }, {});

            // console.log("rows[1]",rows[1]);
            // console.log("valueTypes:",valueTypes);
            // console.log('description flag: ', rows[2][0].value);
            var descriptions = rows[2].reduce(function(descriptions, cell) {
                if (cell.value === "")
                    return descriptions;

                descriptions[cell[colProp]] = cell.value.toUpperCase();

                return descriptions;
            }, {});

            // console.log("valueTypes:",valueTypes);
            rows.splice(0, 3);
            rows.forEach(function(col) {

                var newObject = program.listOnly ? [] : {};
                var hasValues = false;


                col.forEach(function(cell) {
                    var val = "";

                    var t = valueTypes[cell[colProp]];
                    //console.log("type:",cell[colProp],t);
                    if(jsTemplate[t]!=null){
                        val = jsTemplate[t].Parse(cell.value);
                        hasValues = true;
                    } else if (typeof cell.numericValue !== "undefined") {
                        val = parseFloat(cell.numericValue);
                        hasValues = true;
                        console.log("number:",val);
                    } else if (cell.value === "TRUE") {
                        val = true;
                        hasValues = true;
                    } else if (cell.value === "FALSE") {
                        val = false;
                        hasValues = true;
                    } else if (t === "NUMBER" || t == "INT" || t == "DOUBLE" || t == "FLOAT"){
                        val = parseFloat(cell.value);
                        hasValues = true;
                    } else if (t === "STRING"){
                        val = cell.value;
                        hasValues = true;
                    } else if (t === "NOTE") {
                        val = "";
                        hasValues = true;
                    }
                    else if (cell.value !== "") {
                        //console.log(cell.value);
                        //console.log(cell);
                        val = JSON.parse(cell.value);
                        hasValues = true;
                    } 

                    var colNumber = cell[colProp];

                    if (program.listOnly)
                        newObject[colNumber - 1] = val;
                    else
                        newObject[properties[colNumber]] = val;
                });

                if (! hasValues) {
                    if (program.listOnly)
                        newObject[colNumber - 1] = "";
                    else
                        newObject[properties[colNumber]] = "";
                }

                if (isHashed)
                    finalList[newObject[program.hash]] = newObject;
                else
                    finalList.push(newObject);
            });

            //if(program.json && program.json !== "") {
            //    Builder.mkDir(program.json);
            //    var filename = Builder.joinDir(program.json, sheetName + '.json');
            //    console.log('Output json begin: ', filename);
            //    var json = JSON.stringify(finalList, null, program.beautify ? 4 : null);
            //    fs.writeFile(filename, json, "utf-8", function(err) {
            //        if (err) {
            //            throw err;
            //        }
            //        //process.exit(0);
            //        console.log('Output json end: ', filename);
            //    });
            //}

            //console.log(sheetName);
            var gssClass = new Builder.GssClass(sheetName, properties, valueTypes, descriptions, finalList);
            gssClassList[gssClassList.length] = gssClass;
            if(idxSheet < countSheet - 1) {
                readsheetOAuth3(worksheets, idxSheet + 1, countSheet);
            } else {
                //Builder.buildSpreadsheet(gssClassList, outputConfig);
                Builder.buildSpreadsheetToJson(gssClassList, outputConfig);
                running = false;
                console.log('google spreadsheet finished!');
            }

        });
}

function requireJsTemplate(){
    //console.log("requireJsTemplate")
    var jsTemplate = {};
    var files = fs.readdirSync("./JsTemplate");
    for(var index in files){
        //console.log(files[index])
        var name = path.basename(files[index],".js");
        var md = require("./"+path.join("./JsTemplate",name));
        jsTemplate[name] = md;
        jsTemplate[name.toUpperCase()] = md;
    }
    return jsTemplate;
}

function requireCustomerJs(){
    //console.log("requireCustomerJs")
    var customerJsArr = {};
    var files = fs.readdirSync("./CustomerJs");
    for(var index in files){
        //console.log(files[index])
        var name = path.basename(files[index],".js");
        var md = require("./"+path.join("./CustomerJs",name));
        customerJsArr[name] = md;
        customerJsArr[name.toUpperCase()] = md;
    }
    return customerJsArr;
}
