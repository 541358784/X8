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
unityScriptDir="${rootDir}/../../../Scripts/Common/Storage/AutoGeneration"

classTemplate="./Template/UnityStorage.template"

# storage common
/usr/local/bin/node --inspect Js/gDocBuilderTemplate.js 1uxeP2VVZ5l4f4j8yrS7jeAdY9QjSLspvU0Jx5UYht38 ${unityScriptDir} ${classTemplate}
mv $unityScriptDir/StorageCommon.cs ../Framework/Storage/
mv $unityScriptDir/StorageMarketing.cs ../Framework/Storage/
mv $unityScriptDir/StorageActiveData.cs ../Framework/Storage/
mv $unityScriptDir/StorageConfigHub.cs ../Framework/Storage/

echo "Finished"
# pause
cd ${curDir}
