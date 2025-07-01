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

        protected BaseNode()
        {
            var tagsField = new TextField("Tags (comma-separated):");
            tagsField.RegisterValueChangedCallback(evt =>
            {
                string[] tags = evt.newValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                TagsRequired = new List<string>(tags);
            });
            mainContainer.Add(tagsField);

            var intField = new IntegerField("Num Outputs:");
            intField.RegisterValueChangedCallback(evt => NumberOfRooms = evt.newValue);
            mainContainer.Add(intField);
        }

        public abstract void AddOutputPorts();
    }
}