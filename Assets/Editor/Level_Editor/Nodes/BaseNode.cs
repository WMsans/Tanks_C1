using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Editor.Level_Editor.Nodes
{
    public abstract class BaseNode : Node
    {
        public string GUID;
        public List<string> TagsRequired = new List<string>();
        public int NumberOfRooms = 1;

        private TextField _tagsField;
        private IntegerField _intField;

        protected BaseNode()
        {
            _tagsField = new TextField("Tags (comma-separated):");
            _tagsField.RegisterValueChangedCallback(evt =>
            {
                var tags = evt.newValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                TagsRequired = new List<string>(tags);
            });
            mainContainer.Add(_tagsField);

            _intField = new IntegerField("Num Outputs:");
            _intField.RegisterValueChangedCallback(evt =>
            {
                NumberOfRooms = evt.newValue;
                AddOutputPorts();
            });
            mainContainer.Add(_intField);
        }

        public void LoadValuesIntoFields()
        {
            _tagsField.SetValueWithoutNotify(string.Join(",", TagsRequired));
            _intField.SetValueWithoutNotify(NumberOfRooms);
        }

        public abstract void AddOutputPorts();
    }
}