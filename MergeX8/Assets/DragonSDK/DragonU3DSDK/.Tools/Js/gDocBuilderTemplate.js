#!/usr/bin/env node
var fs = require('fs');
var path = require('path');
var program = require('commander');
var Builder = require('./BuilderUtilsTemplate');
var OAuth2Config = require('../oauth2.json');
const { google } = require('googleapis');

program
    .version('0.0.1')
    .usage('<spreadsheet-id> <unity-script-dir> <unity-script-template> [options]')
    .parse(process.argv);

if (program.args.length < 3) {
    program.help();
}

var spreadsheetId = program.args[0];
var outputConfig = new Builder.OutputConfig(program.args[1], program.args[2], program.args[3], program.args[4], program.args[5]);


//queue to build
var gssClassList = [];
var running = false;

//use oauth2 mode
const oauth2Client = new google.auth.OAuth2(OAuth2Config.CLIENT_ID, OAuth2Config.CLIENT_SECRET, OAuth2Config.REDIRECT_URL);

oauth2Client.setCredentials({
    access_token: OAuth2Config.access_token,
    expiry_date: true,
    refresh_token: OAuth2Config.refresh_token,
    token_type: 'Bearer'
});



oauth2Client.getAccessToken(function(err, token) {
    if (err) {
        //done('Google OAuth2 Error: ' + err);
        throw err;
    } else {
        const sheets = google.sheets('v4');
        sheets.spreadsheets.get({
            auth: oauth2Client,
            spreadsheetId: spreadsheetId,
        }, function (err, response) {
            if (err) {
                console.log(err);
                return;
            }
           // console.log(response);
            
           const sheets = google.sheets('v4');
           getAllSheetNames(sheets, oauth2Client, spreadsheetId, function (err, sheetNames) {
               if (err) {
                   console.log(err);
                   return callback(err);
               }
               console.log(sheetNames);
               loadTables(spreadsheetId, sheetNames);
           });
        });

    }
});

function loadTables(spreadsheetId, sheetNames) {
    var spreadsheetId = spreadsheetId;
    console.log('spreadsheetId:' + spreadsheetId);

    // Load client secrets from a local file.
    var sheets = google.sheets('v4');
    sheets.spreadsheets.values.batchGet({
        auth: oauth2Client,
        spreadsheetId: spreadsheetId,
        ranges: sheetNames,
        valueRenderOption: 'FORMULA'
    }, function (err, formulaResponse) {
        if (err) {
            console.log('get formula sheets error');
            return ;
        }

        sheets.spreadsheets.values.batchGet({
            auth: oauth2Client,
            spreadsheetId: spreadsheetId,
            ranges: sheetNames
        }, function (err, response) {
            if (err) {
                console.log('get sheets error');
                return ;
            }

            var gssClassList = [];
            for (var i = 0; i < response.data.valueRanges.length; i++) {
                var sheetInfo = formulaResponse.data.valueRanges[i].values;
                console.log(sheetInfo[0]);
                var gssClass = new Builder.GssClass(sheetNames[i], sheetInfo[0], sheetInfo[1], sheetInfo[2], sheetInfo[3], sheetInfo[4],sheetInfo[5]);
                gssClassList[gssClassList.length] = gssClass;
            }

            Builder.buildSpreadsheet(gssClassList, outputConfig);
        });
    });
}


function getAllSheetNames(sheets, auth, spreadsheetId, callback) {
    sheets.spreadsheets.get({
        auth: auth,
        spreadsheetId: spreadsheetId,
    }, function (err, response) {
        if (err) {
            console.log(err);
            return callback(err);
        }
        console.log(response);
        var result = [];
        for (var i = 0; i < response.data.sheets.length; i++) {
            result[i] = response.data.sheets[i].properties.title;
        }
        callback(err, result);

    });
}
