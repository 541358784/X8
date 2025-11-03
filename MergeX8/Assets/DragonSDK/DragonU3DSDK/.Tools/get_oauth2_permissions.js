// Based on https://raw.githubusercontent.com/google/google-api-nodejs-client/master/examples/oauth2.js
// Adapted by Oystein Steimler <oystein@nyvri.net>

/**
 * Copyright 2012 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

var readline = require('readline');
var program = require('commander');
const { google } = require('googleapis');
var fs = require('fs');

var OAuth2Client = google.auth.OAuth2;

// Client ID and client secret are available at
// https://code.google.com/apis/console
// 1. Create or pick a project
// 2. Choose "API & Auth" and then "Credentials"
// 3. Click "Create new Client ID"
// 4. Select "Installed application" and "Other"
// 5. Copy your ClientID and Client Secret into the fields beneath

program
		.version('0.0.1')
		.usage('<CLIENT_ID> <CLIENT_SECRET> <REDIRECT_URL>')
		.option('-s --scope [scope]', 'permission scope, default use: https://spreadsheets.google.com/feeds')
		.parse(process.argv);

if(program.args.length < 3) {
	program.help();
}

var output = 'oauth2.json'
var CLIENT_ID = program.args[0];
var CLIENT_SECRET = program.args[1];
var REDIRECT_URL = program.args[2];

// 6. This scope 'https://spreadsheets.google.com/feeds' provides full access to all Spreadsheets in
// your Google Drive. Find more scopes here: https://developers.google.com/drive/web/scopes
// and https://developers.google.com/google-apps/spreadsheets/authorize
var PERMISSION_SCOPE = 'https://spreadsheets.google.com/feeds'; //space-delimited string or an array of scopes
if(program.scope) {
  PERMISSION_SCOPE = program.scope;
}

// 6. Run this script: `node get_oauth2_permissions.js'
// 7. Visit the URL printed, authenticate the google user, grant the permission
// 8. Copy the authorization code and paste it at the prompt of this program.
// 9. The refresh_token you get is needed with the client_id and client_secret when using edit-google-spreadsheet

var oauth2Client = new OAuth2Client(CLIENT_ID, CLIENT_SECRET, REDIRECT_URL);


var rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

const scopes = ['https://www.googleapis.com/auth/spreadsheets'];

// generate consent page url
const url = oauth2Client.generateAuthUrl({
  // 'online' (default) or 'offline' (gets refresh_token)
  access_type: 'offline',

  // If you only need one scope you can pass it as a string
  scope: scopes
});

console.log('Visit this url:\n%s\n', url); // provides a refresh token

rl.question('Enter the code here: ', function(code) {
  console.log('\nGetting token...');
  oauth2Client.getToken(code, function(err, tokens) {
    if(err) {
      return console.log("Error getting token: " + err);
    }
    var creds = { CLIENT_ID: CLIENT_ID, CLIENT_SECRET: CLIENT_SECRET, REDIRECT_URL : REDIRECT_URL, access_token : code,refresh_token: tokens.refresh_token };
    var json = JSON.stringify(creds, null, 4);
    console.log('Use this in your Spreadsheet.load():\n"oauth2": %s', json);
    console.log('token: ', tokens);

    fs.writeFile(output, json, "utf-8", function(err) {
      if (err) {
        throw err;
      }
      console.log('Finished!');
      process.exit(0);
    });
  });
});