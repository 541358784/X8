using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hospital.Editor
{
    public class WindowMain : EditorWindow
    {
        private const string ScenePath = "Assets/Scenes/";
        private const string MapEditorName = "WorldEditor";
        private const string MainSceneName = "InitScene";
        private const string PathSceneMain = ScenePath+MainSceneName+".unity";
        private const string PathSceneEditor = ScenePath+MapEditorName+".unity";
        
        [MenuItem("Tools/MapEditor", false, 5000)]
        public static void Init(MenuCommand command)
        {
            var window = EditorWindow.GetWindow<WindowMain>(false);
            window.minSize = new Vector2(150f, 200f);
            window.maxSize = new Vector2(6000f, 4000f);
            window.titleContent = new GUIContent($"MapEditor"); //, Icons.spine);
            window.Show();
        }

        private bool isLoaded;
        private WorldMapTool _worldMapTool;

        private void Load(int mapId)
        {
            if (isLoaded) return;
            isLoaded = true;

            _worldMapTool = new WorldMapTool();
            _worldMapTool.LoadWorld(mapId);
        }

        private void Save()
        {
            _worldMapTool.Save();
        }

        private void Unload()
        {
            if (!isLoaded) return;
            isLoaded = false;

            _worldMapTool?.UnloadWorld();
        }

        private void BackToMain()
        {
            Unload();
            EditorSceneManager.OpenScene(PathSceneMain);
        }

        private void OnFocus()
        {
            Selection.selectionChanged -= Repaint;
        }

        private void OnLostFocus()
        {
            Selection.selectionChanged += Repaint;
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlaying)
                return;

            if (SceneManager.GetActiveScene().name != MapEditorName)
            {
                if (GUILayout.Button("Open"))
                {
                    EditorSceneManager.OpenScene(PathSceneEditor);
                    var gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                    foreach (var go in gameObjects)
                        UnityEngine.Object.DestroyImmediate(go);
                }

                if (SceneManager.GetActiveScene().name == "InitScene")
                {
                    if (GUILayout.Button("Play"))
                        EditorApplication.isPlaying = true;
                }

                return;
            }

            if (!DrawBase())
                return;

            DrawDecoWorld();
        }

        private int mapSelectedId;
        private readonly string[] MapIds = { "1", "2", "3", "4" };

        private bool DrawBase()
        {
            using (new HorizontalScope())
            {
                if (GUILayout.Button("Close"))
                {
                    BackToMain();
                    return false;
                }

                using (new DisabledGroupScope(isLoaded))
                {
                    mapSelectedId = EditorGUILayout.Popup(mapSelectedId, MapIds, GUILayout.Width(60));
                }

                if (GUILayout.Button("Load")) 
                    Load(int.Parse(MapIds[mapSelectedId]));

                using (new DisabledGroupScope(!isLoaded))
                {
                    if (GUILayout.Button("Unload")) Unload();
                    if (GUILayout.Button("Save")) Save();
                }
            }

            return true;
        }

        private int deviceSelectedId;

        private void DrawDecoWorld()
        {
            _worldMapTool?.DrawGUI();
        }
    }
    
}