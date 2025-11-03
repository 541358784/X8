using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
public class MyEditorTools {
	[MenuItem ("Tools/UI/预制体中重复资源检查", priority = 203)]
	public static void FindSameTexture () {
		Dictionary<string, string> md5dic = new Dictionary<string, string> ();
		string[] paths = AssetDatabase.FindAssets ("t:prefab", new string[] { "Assets" });
		foreach (var prefabguid in paths) {
			string prefabAssetPath = AssetDatabase.GUIDToAssetPath (prefabguid);
			string[] depends = AssetDatabase.GetDependencies (prefabAssetPath, true);
			for (int i = 0; i < depends.Length; i++) {
				string assetPath = depends[i];
				AssetImporter importer = AssetImporter.GetAtPath (assetPath);
				if (importer is TextureImporter || importer is ModelImporter) {
					string md5 = getMd5Hash (Path.Combine (Directory.GetCurrentDirectory (), assetPath));
					string path;
					if (!md5dic.TryGetValue (md5, out path)) {
						md5dic[md5] = assetPath;
					} else {
						if (!string.IsNullOrEmpty (path) && path != assetPath) {
							Debug.Log($"<color=blue> {path}</color>" ,AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
							Debug.Log($"<color=red> {assetPath}</color>" ,AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath));
						}
					}
				}
			}
		}
	}
	[MenuItem ("Tools/UI/重复Texture检查", priority = 202)]
	public static void FindSameRes(){
		Dictionary<string, string> md5dic = new Dictionary<string, string> ();
		string[] paths = AssetDatabase.FindAssets ("t:Texture", new string[] { "Assets" });
		string currentpath = Directory.GetCurrentDirectory ();
		Debug.LogFormat ("currentDirectory:{0}",currentpath);
		foreach (var prefabguid in paths) {
			string assetPath = AssetDatabase.GUIDToAssetPath (prefabguid);
			// AssetImporter importer = AssetImporter.GetAtPath (assetPath);
			// if (importer is TextureImporter || importer is ModelImporter) {
				string md5 = getMd5Hash (Path.Combine (currentpath, assetPath));
				string path;
				if (!md5dic.TryGetValue (md5, out path)) {
					md5dic[md5] = assetPath;
				} else {
					if (!string.IsNullOrEmpty (path) && path != assetPath) {
						Debug.Log($"<color=green>{path}</color>" ,AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
						Debug.Log($"<color=red> {assetPath}</color>" ,AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath));
					}
				}
			// }
		}
	}

	public static string getMd5Hash (string path) {
		MD5 md5 = new MD5CryptoServiceProvider ();
		string rst = BitConverter.ToString (md5.ComputeHash (File.ReadAllBytes (path)));
		rst = rst.Replace ("-", "").ToLower ();
		return rst;
	}

	[MenuItem ("Assets/FindwhoUsedMe", false, 10)]
	public static void findRef () {
		Dictionary<string, string> guidDics = new Dictionary<string, string> ();
		foreach (UnityEngine.Object o in Selection.objects) {
			string path = AssetDatabase.GetAssetPath (o);
			if (!string.IsNullOrEmpty (path)) {
//				string guid = AssetDatabase.AssetPathToGUID (path);
				AssetDatabase.TryGetGUIDAndLocalFileIdentifier(o, out var guid, out long localId);
				if (!guidDics.ContainsKey (guid)) {
					guidDics[guid] = path;
				}
				Debug.Log($"{o.name }'s localid ={localId}");
			}
		}
		if (guidDics.Count > 0) {
			List<string> extensions = new List<string> () { ".prefab", ".unity", ".mat", ".asset" };
			string[] files = Directory.GetFiles (Application.dataPath, "*.*", SearchOption.AllDirectories).Where (
				s => extensions.Contains (Path.GetExtension (s).ToLower ())).ToArray ();

			for (int i = 0; i < files.Length; i++) {
				string file = files[i];
				if(i %20 == 0){
					bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中",file,(float)i/(float)files.Length);
					if(isCancel){
						break;
					}
				}
				foreach(KeyValuePair<string,string> guidItem in guidDics){
					if(Regex.IsMatch(File.ReadAllText(file),guidItem.Key)){
						Debug.Log($"<color=green>{guidItem.Value}</color>" ,AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(guidItem.Value));
						Debug.Log($"<color=red> {file}</color>" ,AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(getRelativeAssetPath(file)));
					}
				}
			}
			EditorUtility.ClearProgressBar();
		}
	}

	[MenuItem ("Assets/FindwhoUsedMe", true)]
	public static bool vfindRef () {
		if(!Selection.activeObject){
			return false;
		}
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		return (!string.IsNullOrEmpty (path));
	}
	/// <summary>
	/// 相对Assets 相对目录
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	static string getRelativeAssetPath (string path) {
		return Path.GetFullPath (path).Replace (Application.dataPath, "Assets").Replace ("\\", "/");
	}
}