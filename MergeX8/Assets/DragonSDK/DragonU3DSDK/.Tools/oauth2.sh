#!/bin/sh

CLIENT_ID="1020376486872-133o7dt3i534jq1h8sku9t0kq8dmrimt.apps.googleusercontent.com"
CLIENT_SECRET="7krNknCGmnZJtdS6mm-RUjJn"
REDIRECT_URL="urn:ietf:wg:oauth:2.0:oob"


node get_oauth2_permissions.js ${CLIENT_ID} ${CLIENT_SECRET} ${REDIRECT_URL}
 
# pause