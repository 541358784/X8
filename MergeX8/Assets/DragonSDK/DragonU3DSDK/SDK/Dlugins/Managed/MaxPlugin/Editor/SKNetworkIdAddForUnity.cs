#if UNITY_IPHONE || UNITY_IOS
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace DragonPlus.Ad.Max.Editor
{
    public static class SKNetworkIdAddForUnity
    {
        private static List<string> skNetworkIds = new List<string>()
        {
            "mj797d8u6f.skadnetwork",
            "a8cz6cu7e5.skadnetwork",
            "5f5u5tfb26.skadnetwork",
            "vhf287vqwu.skadnetwork",
            "mqn7fxpca7.skadnetwork",
            "a2p9lx4jpn.skadnetwork",
            "g6gcrrvk4p.skadnetwork",
            "xga6mpmplv.skadnetwork",
            "6yxyv74ff7.skadnetwork",
            "k6y4y55b64.skadnetwork"
        };

        private const string KEY_SK_ADNETWORK_ITEMS = "SKAdNetworkItems";
        private const string KEY_SK_ADNETWORK_ID    = "SKAdNetworkIdentifier";


        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            string plistPath = Path.Combine(path, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            if (skNetworkIds.Count > 0)
            {
                AddSKAdNetworkIdentifier(plist, skNetworkIds);
            }

            string unityVersion = Application.unityVersion;
            if (!string.IsNullOrEmpty(unityVersion))
            {
                plist.root.SetString("GADUUnityVersion", unityVersion);
            }

            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static PlistElementArray GetSKAdNetworkItemsArray(PlistDocument document)
        {
            PlistElementArray array;
            if (document.root.values.ContainsKey(KEY_SK_ADNETWORK_ITEMS))
            {
                try
                {
                    document.root.values.TryGetValue(KEY_SK_ADNETWORK_ITEMS, out var element);
                    array = element.AsArray();
                }
                catch (Exception e)
                {
                    // The element is not an array type.
                    array = null;
                }
            }
            else
            {
                array = document.root.CreateArray(KEY_SK_ADNETWORK_ITEMS);
            }

            return array;
        }

        private static void AddSKAdNetworkIdentifier(PlistDocument document, List<string> skAdNetworkIds)
        {
            PlistElementArray array = GetSKAdNetworkItemsArray(document);
            if (array != null)
            {
                foreach (string id in skAdNetworkIds)
                {
                    if (!ContainsSKAdNetworkIdentifier(array, id))
                    {
                        PlistElementDict added = array.AddDict();
                        added.SetString(KEY_SK_ADNETWORK_ID, id);
                    }
                }
            }
            else
            {
                NotifyBuildFailure("SKAdNetworkItems element already exists in Info.plist, but is not an array.", false);
            }
        }

        private static bool ContainsSKAdNetworkIdentifier(PlistElementArray skAdNetworkItemsArray, string id)
        {
            foreach (PlistElement elem in skAdNetworkItemsArray.values)
            {
                try
                {
                    PlistElementDict elemInDict = elem.AsDict();
                    bool identifierExists = elemInDict.values.TryGetValue(KEY_SK_ADNETWORK_ID, out var value);

                    if (identifierExists && value.AsString().Equals(id))
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    // Do nothing
                }
            }

            return false;
        }

        private static void NotifyBuildFailure(string message, bool showOpenSettingsButton = true)
        {
            ThrowBuildException("[GoogleMobileAds] " + message);
        }

        private static void ThrowBuildException(string message)
        {
            throw new BuildPlayerWindow.BuildMethodException(message);
        }
    }
}

#endif