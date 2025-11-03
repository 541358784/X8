#!/bin/sh

ORIGIN_PATH="$1"
OUTPUT_PATH="$2"

python -c """import urllib; import sys; sys.argv = ['transpy', '-i', '$ORIGIN_PATH', '-o', '$OUTPUT_PATH']; s = urllib.urlopen('https://raw.githubusercontent.com/sdkbox-doc/en/master/tools/generate_xml_from_google_services_json.py').read(); exec(s);"""