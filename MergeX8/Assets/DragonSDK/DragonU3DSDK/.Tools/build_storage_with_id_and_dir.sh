#!/bin/sh

export NODE_PATH=/usr/local/lib/node_modules


# NOTE: OAuth2 used oauth2.json which was made by oauth.sh
# modified oauth.sh to change CLIENT_ID etc.

curDir=`pwd`

BASEDIR=$(dirname $0)
echo "Script location: ${BASEDIR}"
cd ${BASEDIR}

rootDir=`pwd`

#client script directory
unityScriptDir="$2"

if [ -z "$unityScriptDir" ]; then
	unityScriptDir="${rootDir}/../../../Scripts/Common/Storage/AutoGeneration"
	echo "Target unityScriptDir is NULL， use default: '${rootDir}/../../../Scripts/Common/Storage/AutoGeneration'"
fi

echo "Target unityScriptDir: ${unityScriptDir}"

#clean directory, some file may be already deleted
/usr/local/bin/node --inspect Js/Tools.js ${unityScriptDir} --meta --mk

# ck3
spreadsheet_id=$1
#unity script manager template name, such as TableManager[TableManager.template MUST in Template]
classTemplate="./Template/UnityStorage.template"
#Use OAuth2, defaults config is oauth2.json
/usr/local/bin/node --inspect Js/gDocBuilderTemplate.js ${spreadsheet_id} ${unityScriptDir} ${classTemplate}

# storage common
# /usr/local/bin/node Js/gDocBuilderTemplate.js 1uxeP2VVZ5l4f4j8yrS7jeAdY9QjSLspvU0Jx5UYht38 ${unityScriptDir} ${classTemplate}
# mv $unityScriptDir/StorageCommon.cs ../Framework/Storage/
# mv $unityScriptDir/StorageMarketing.cs ../Framework/Storage/


echo "Finished"
# pause
cd ${curDir}
