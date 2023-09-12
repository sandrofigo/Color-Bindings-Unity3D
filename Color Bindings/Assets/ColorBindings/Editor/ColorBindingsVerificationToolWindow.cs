using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColorBindings.Editor
{
    public class ColorBindingsVerificationToolWindow : EditorWindow
    {
        [MenuItem("Tools/Color Bindings Verification Tool")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow(typeof(ColorBindingsVerificationToolWindow));
            window.titleContent = new GUIContent("Color Bindings Verification Tool");
            window.minSize = new Vector2(500, 400);
        }

        private readonly List<ColorBindingBase> colorBindings = new();

        private Vector2 scrollPosition;

        private double nextTick;
        private const double RefreshDelay = 3f;

        private bool autoApply;

        private bool colorBindingCheckerFoldedOut = true;

        private void Update()
        {
            double timeUntilTick = nextTick - EditorApplication.timeSinceStartup;

            if (timeUntilTick <= 0)
            {
                OnTick();
                nextTick = EditorApplication.timeSinceStartup + RefreshDelay;
            }
        }

        private void OnTick()
        {
            if (!Application.isPlaying)
                return;

            if (autoApply)
            {
                RefreshColorBindings();
                HideElementsWithColorBinding();
            }
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("This tool should only be used in Play Mode.", MessageType.Warning);

                GUI.enabled = false;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            colorBindingCheckerFoldedOut = EditorGUILayout.BeginFoldoutHeaderGroup(colorBindingCheckerFoldedOut, "Color Binding Checker");
            if (colorBindingCheckerFoldedOut)
            {
                EditorGUI.indentLevel++;

                autoApply = GUILayout.Toggle(autoApply, "Auto-Apply?");

                GUILayout.Space(5);

                if (GUILayout.Button("Test Color Bindings"))
                {
                    RefreshColorBindings();
                    HideElementsWithColorBinding();
                }

                if (GUILayout.Button("Revert Color Bindings"))
                {
                    RefreshColorBindings();
                    RevertColorBindings();
                }

                GUILayout.Space(5);

                EditorGUILayout.HelpBox("All elements using a color binding should be invisible after using the button above. You should only see elements that don't have a binding.", MessageType.Info);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(10);

            GUILayout.EndScrollView();
        }

        private void RevertColorBindings()
        {
            foreach (ColorBindingBase colorBinding in colorBindings)
            {
                if (colorBinding == null)
                    continue;

                colorBinding.UpdateColor();
            }
        }

        private void HideElementsWithColorBinding()
        {
            foreach (ColorBindingBase colorBinding in colorBindings)
            {
                if (colorBinding == null)
                    continue;

                colorBinding.ClearColor();
            }
        }

        private void RefreshColorBindings()
        {
            colorBindings.Clear();
            colorBindings.AddRange(FindAllObjectsOfTypeExpensive<ColorBindingBase>());
        }

        private static IEnumerable<GameObject> GetAllRootGameObjects()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var rootGameObjects = SceneManager.GetSceneAt(i).GetRootGameObjects();

                foreach (GameObject obj in rootGameObjects)
                    yield return obj;
            }
        }

        private static IEnumerable<T> FindAllObjectsOfTypeExpensive<T>() where T : MonoBehaviour
        {
            foreach (GameObject obj in GetAllRootGameObjects())
            {
                foreach (T child in obj.GetComponentsInChildren<T>(true))
                    yield return child;
            }
        }
    }
}