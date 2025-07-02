using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Level_Editor
{
    public class LEditorWindow : EditorWindow
    {
        private LGraphView _graphView;
        private string _fileName = "New Level Graph";

        [MenuItem("Tools/Level Editor/Editor Window")]
        public static void ShowExample()
        {
            var wnd = GetWindow<LEditorWindow>();
            wnd.titleContent = new GUIContent("Level Graph");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Add the graph view
            _graphView = new LGraphView
            {
                name = "Level Graph"
            };
            _graphView.StretchToParentSize();
            root.Add(_graphView);

            // Add a toolbar
            var toolbar = new UnityEditor.UIElements.Toolbar();

            var fileNameTextField = new TextField("File Name:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });
            toolbar.Add(new Button(() => _graphView.GenerateLevel()) { text = "Generate Level" });
            toolbar.Add(new Button(() => GraphPropertiesWindow.ShowWindow(_graphView)) { text = "Properties" });


            root.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "OK");
                return;
            }

            var saveUtility = GraphSaveUtility.GetInstance(_graphView);
            if (save)
                saveUtility.SaveGraph(_fileName);
            else
                saveUtility.LoadGraph(_fileName);
        }
    }
}