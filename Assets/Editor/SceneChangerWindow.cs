using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TMKOC.EditorTools
{
    /// <summary>
    /// Help -> Scene Changer. A jump list for every scene in the project.
    ///
    /// The list is rebuilt from the AssetDatabase, not from a stored array, so a scene
    /// created, renamed, moved or deleted after this window was opened turns up without
    /// anyone maintaining anything. It refreshes on focus and on EditorApplication.projectChanged.
    ///
    /// This project ships ~120 scenes, but only about a dozen are ours -- the rest are
    /// Feel / Epic Toon FX / AssetKits / Layer Lab demo scenes. A flat list of all of
    /// them is useless, so third-party paths are hidden by default. The ignore list is
    /// editable under Filters and is stored per-user in EditorPrefs.
    /// </summary>
    public class SceneChangerWindow : EditorWindow
    {
        private const string IgnoreKey = "TMKOC.SceneChanger.Ignore";
        private const string HideVendorKey = "TMKOC.SceneChanger.HideVendor";
        private const string BuildOnlyKey = "TMKOC.SceneChanger.BuildOnly";

        /// <summary>Path fragments hidden by default. Case-insensitive substring match on the asset path.</summary>
        private const string DefaultIgnore =
            "Feel, Epic Toon FX, AssetKits, Layer Lab, Plugins, Joystick Pack, StarLink, TutorialInfo, Sirenix, Demigiant";

        private class Entry
        {
            public string path;
            public string name;
            public string folder;
            public int buildIndex = -1;
            public bool buildEnabled;
        }

        private readonly List<Entry> entries = new List<Entry>();
        private readonly Dictionary<string, bool> foldouts = new Dictionary<string, bool>();

        private Vector2 scroll;
        private string search = string.Empty;
        private string ignore;
        private bool hideVendor = true;
        private bool buildOnly;
        private bool showFilters;

        [MenuItem("Help/Scene Changer", false, 200)]
        public static void Open()
        {
            SceneChangerWindow w = GetWindow<SceneChangerWindow>(false, "Scene Changer", true);
            w.minSize = new Vector2(340f, 260f);
            w.Focus();
        }

        private void OnEnable()
        {
            ignore = EditorPrefs.GetString(IgnoreKey, DefaultIgnore);
            hideVendor = EditorPrefs.GetBool(HideVendorKey, true);
            buildOnly = EditorPrefs.GetBool(BuildOnlyKey, false);

            EditorApplication.projectChanged += OnProjectChanged;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            Rescan();
        }

        private void OnDisable()
        {
            EditorApplication.projectChanged -= OnProjectChanged;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
        }

        // a new scene asset, a rename, a delete -- all land here
        private void OnProjectChanged()
        {
            Rescan();
            Repaint();
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            Repaint();
        }

        // catches changes made while this window was in the background
        private void OnFocus()
        {
            Rescan();
            Repaint();
        }

        private void Rescan()
        {
            entries.Clear();

            EditorBuildSettingsScene[] build = EditorBuildSettings.scenes;
            string[] guids = AssetDatabase.FindAssets("t:Scene");

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                // Packages/ scenes are read-only noise; only project assets are listed
                if (string.IsNullOrEmpty(path) || !path.StartsWith("Assets/")) continue;

                Entry e = new Entry();
                e.path = path;
                e.name = Path.GetFileNameWithoutExtension(path);
                e.folder = Path.GetDirectoryName(path).Replace('\\', '/');

                for (int b = 0; b < build.Length; b++)
                {
                    if (build[b].path == path)
                    {
                        e.buildIndex = b;
                        e.buildEnabled = build[b].enabled;
                        break;
                    }
                }

                entries.Add(e);
            }

            entries.Sort(delegate (Entry a, Entry b)
            {
                int c = string.Compare(a.folder, b.folder, StringComparison.OrdinalIgnoreCase);
                return c != 0 ? c : string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase);
            });
        }

        private bool Passes(Entry e)
        {
            if (buildOnly && e.buildIndex < 0) return false;

            if (hideVendor && !string.IsNullOrEmpty(ignore))
            {
                string[] frags = ignore.Split(',');
                for (int i = 0; i < frags.Length; i++)
                {
                    string f = frags[i].Trim();
                    if (f.Length == 0) continue;
                    if (e.path.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0) return false;
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                bool hit = e.name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                        || e.path.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
                if (!hit) return false;
            }

            return true;
        }

        private static bool IsLoaded(string path)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).path == path) return true;
            }
            return false;
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Unity is in Play mode. Stop it before switching scenes.", MessageType.Warning);
            }

            int shown = 0;
            scroll = EditorGUILayout.BeginScrollView(scroll);

            string currentFolder = null;
            bool folderVisible = true;

            for (int i = 0; i < entries.Count; i++)
            {
                Entry e = entries[i];
                if (!Passes(e)) continue;
                shown++;

                if (e.folder != currentFolder)
                {
                    currentFolder = e.folder;
                    if (!foldouts.ContainsKey(currentFolder)) foldouts[currentFolder] = true;

                    EditorGUILayout.Space(2f);
                    foldouts[currentFolder] = EditorGUILayout.Foldout(
                        foldouts[currentFolder], currentFolder, true, EditorStyles.foldoutHeader);
                    folderVisible = foldouts[currentFolder];
                }

                if (folderVisible) DrawRow(e);
            }

            EditorGUILayout.EndScrollView();

            if (shown == 0)
            {
                EditorGUILayout.HelpBox(
                    entries.Count == 0
                        ? "No scenes found under Assets/."
                        : "No scenes match the current search and filters.",
                    MessageType.Info);
            }

            EditorGUILayout.LabelField(shown + " of " + entries.Count + " scenes", EditorStyles.miniLabel);
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Label("Search", EditorStyles.miniLabel, GUILayout.Width(44f));
            search = GUILayout.TextField(search, EditorStyles.toolbarTextField, GUILayout.MinWidth(70f));
            if (GUILayout.Button("x", EditorStyles.toolbarButton, GUILayout.Width(20f)))
            {
                search = string.Empty;
                GUI.FocusControl(null);
            }

            EditorGUI.BeginChangeCheck();
            hideVendor = GUILayout.Toggle(hideVendor, new GUIContent("Project only",
                "Hide scenes whose path matches the ignore list under Filters."),
                EditorStyles.toolbarButton, GUILayout.Width(84f));
            buildOnly = GUILayout.Toggle(buildOnly, new GUIContent("In build",
                "Only scenes listed in Build Settings."),
                EditorStyles.toolbarButton, GUILayout.Width(62f));
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(HideVendorKey, hideVendor);
                EditorPrefs.SetBool(BuildOnlyKey, buildOnly);
            }

            GUILayout.FlexibleSpace();

            showFilters = GUILayout.Toggle(showFilters, "Filters", EditorStyles.toolbarButton, GUILayout.Width(50f));
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(56f))) Rescan();

            EditorGUILayout.EndHorizontal();

            if (!showFilters) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Hide any scene whose path contains (comma separated):", EditorStyles.miniLabel);

            EditorGUI.BeginChangeCheck();
            ignore = EditorGUILayout.TextArea(ignore, GUILayout.MinHeight(38f));
            if (EditorGUI.EndChangeCheck()) EditorPrefs.SetString(IgnoreKey, ignore);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset to default", EditorStyles.miniButton))
            {
                ignore = DefaultIgnore;
                EditorPrefs.SetString(IgnoreKey, ignore);
                GUI.FocusControl(null);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawRow(Entry e)
        {
            bool loaded = IsLoaded(e.path);
            bool playing = EditorApplication.isPlaying;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(14f);

            // build-settings badge, so it is obvious which scenes actually ship
            if (e.buildIndex >= 0)
            {
                GUIContent badge = new GUIContent(
                    e.buildIndex.ToString(),
                    e.buildEnabled ? "Build Settings index " + e.buildIndex : "In Build Settings but disabled");
                Color prev = GUI.color;
                if (!e.buildEnabled) GUI.color = new Color(1f, 1f, 1f, 0.45f);
                GUILayout.Label(badge, EditorStyles.miniButton, GUILayout.Width(24f));
                GUI.color = prev;
            }
            else
            {
                GUILayout.Space(28f);
            }

            GUIStyle nameStyle = loaded ? EditorStyles.boldLabel : EditorStyles.label;
            string label = loaded ? e.name + "   (open)" : e.name;
            if (GUILayout.Button(new GUIContent(label, e.path), nameStyle))
            {
                // clicking the name reveals it, so this window doubles as a locator
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(e.path));
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(playing))
            {
                if (GUILayout.Button(new GUIContent("+", "Open additively, on top of what is already loaded"),
                        EditorStyles.miniButtonLeft, GUILayout.Width(24f)))
                {
                    OpenScene(e.path, OpenSceneMode.Additive);
                }

                using (new EditorGUI.DisabledScope(loaded && SceneManager.sceneCount == 1))
                {
                    if (GUILayout.Button(new GUIContent("Open", "Close everything else and open this scene"),
                            EditorStyles.miniButtonRight, GUILayout.Width(46f)))
                    {
                        OpenScene(e.path, OpenSceneMode.Single);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            Rect row = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.ContextClick && row.Contains(Event.current.mousePosition))
            {
                ShowContextMenu(e);
                Event.current.Use();
            }
        }

        private void ShowContextMenu(Entry e)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Show in Project"), false, delegate
            {
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(e.path));
            });
            menu.AddItem(new GUIContent("Copy Path"), false, delegate
            {
                EditorGUIUtility.systemCopyBuffer = e.path;
            });

            menu.AddSeparator(string.Empty);

            if (e.buildIndex >= 0)
            {
                menu.AddItem(new GUIContent("Remove from Build Settings"), false, delegate { SetInBuild(e.path, false); });
                menu.AddItem(new GUIContent(e.buildEnabled ? "Disable in Build Settings" : "Enable in Build Settings"),
                    false, delegate { ToggleBuildEnabled(e.path); });
            }
            else
            {
                menu.AddItem(new GUIContent("Add to Build Settings"), false, delegate { SetInBuild(e.path, true); });
            }

            menu.ShowAsContext();
        }

        private void OpenScene(string path, OpenSceneMode mode)
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("[Scene Changer] Stop Play mode before switching scenes.");
                return;
            }

            // the asset may have been deleted since the last scan
            if (!File.Exists(path))
            {
                Debug.LogWarning("[Scene Changer] Scene no longer exists: " + path);
                Rescan();
                Repaint();
                return;
            }

            // never lose work silently -- this is the whole reason not to call OpenScene directly
            if (mode == OpenSceneMode.Single && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            EditorSceneManager.OpenScene(path, mode);
            Repaint();
        }

        private void SetInBuild(string path, bool include)
        {
            List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

            if (include)
            {
                for (int i = 0; i < list.Count; i++) if (list[i].path == path) return;
                list.Add(new EditorBuildSettingsScene(path, true));
            }
            else
            {
                list.RemoveAll(delegate (EditorBuildSettingsScene s) { return s.path == path; });
            }

            EditorBuildSettings.scenes = list.ToArray();
            Rescan();
            Repaint();
        }

        private void ToggleBuildEnabled(string path)
        {
            EditorBuildSettingsScene[] list = EditorBuildSettings.scenes;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].path == path) list[i].enabled = !list[i].enabled;
            }
            EditorBuildSettings.scenes = list;
            Rescan();
            Repaint();
        }
    }
}
