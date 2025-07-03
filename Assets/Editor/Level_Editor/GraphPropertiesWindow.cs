using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Level_Editor
{
    public class GraphPropertiesWindow : EditorWindow
    {
        public LGraphView graphView;

        public void CreateGUI()
        {
            if (graphView == null)
            {
                rootVisualElement.Add(new Label("Waiting for GraphView..."));
                return;
            }

            rootVisualElement.Clear();

            var entryPointSwapsField = new IntegerField("Entry Point Swaps");
            entryPointSwapsField.SetValueWithoutNotify(graphView.GraphProperties.EntryPointSwaps);
            entryPointSwapsField.RegisterValueChangedCallback(evt =>
            {
                graphView.GraphProperties.EntryPointSwaps = evt.newValue;
            });
            rootVisualElement.Add(entryPointSwapsField);

            var roomChangeAttemptsField = new IntegerField("Room Change Attempts");
            roomChangeAttemptsField.SetValueWithoutNotify(graphView.GraphProperties.RoomChangeAttempts);
            roomChangeAttemptsField.RegisterValueChangedCallback(evt =>
            {
                graphView.GraphProperties.RoomChangeAttempts = evt.newValue;
            });
            rootVisualElement.Add(roomChangeAttemptsField);
            
            var startPositionField = new Vector3Field("Start Position");
            startPositionField.SetValueWithoutNotify(graphView.GraphProperties.StartPosition);
            startPositionField.RegisterValueChangedCallback(evt =>
            {
                graphView.GraphProperties.StartPosition = evt.newValue;
            });
            rootVisualElement.Add(startPositionField);
        }

        public static void ShowWindow(LGraphView view)
        {
            var window = GetWindow<GraphPropertiesWindow>("Graph Properties");
            window.graphView = view;
            window.CreateGUI();
        }
    }
}