// ═══════════════════════════════════════════════════════════════
// LoadingPipelineEditor.cs
// ═══════════════════════════════════════════════════════════════
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LoadingSystem.Editor
{
    /// <summary>
    /// Custom editor for LoadingPipeline components (if they were MonoBehaviours).
    /// This is a standalone editor tool for visualizing pipeline structures.
    /// </summary>
    public class LoadingPipelineEditor : EditorWindow
    {
        private Vector2 _scrollPosition;
        private string _pipelineJson = "";
        private LoadingPipeline _currentPipeline;

        [MenuItem("Tools/Loading System/Pipeline Visualizer")]
        public static void ShowWindow()
        {
            GetWindow<LoadingPipelineEditor>("Pipeline Visualizer");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Loading Pipeline Visualizer", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Create Demo Pipeline", EditorStyles.boldLabel);
            if (GUILayout.Button("Generate Demo Pipeline"))
            {
                _currentPipeline = LoadingProgressTracker.CreateDemoPipeline();
                _pipelineJson = JsonUtility.ToJson(_currentPipeline, true);
            }

            EditorGUILayout.Space();

            if (_currentPipeline != null)
            {
                EditorGUILayout.LabelField("Pipeline Structure", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "This is a visual representation of the pipeline structure.\n" +
                    "In a real implementation, you would visualize the actual steps and weights.",
                    MessageType.Info
                );

                EditorGUILayout.LabelField("Progress", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Current Progress: {_currentPipeline.Progress:P0}");
                EditorGUILayout.LabelField($"Is Completed: {_currentPipeline.IsCompleted}");

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Pipeline JSON (Debug)", EditorStyles.boldLabel);
                _pipelineJson = EditorGUILayout.TextArea(_pipelineJson, GUILayout.Height(200));
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Generate a demo pipeline to visualize the structure.\n" +
                    "This tool helps understand how pipelines are constructed and how progress is calculated.",
                    MessageType.Info
                );
            }

            EditorGUILayout.EndScrollView();
        }

        private void Update()
        {
            if (_currentPipeline != null && !_currentPipeline.IsCompleted)
            {
                // Force repaint to update progress
                Repaint();
            }
        }
    }
}
#endif