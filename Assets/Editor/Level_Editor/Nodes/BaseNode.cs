using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Level_Editor.Nodes
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
                var newNumberOfRooms = Math.Max(0, Math.Min(evt.newValue, 4)); // Clamping value between 0 and 4

                if (NumberOfRooms == newNumberOfRooms) return;

                NumberOfRooms = newNumberOfRooms;
                var graphView = GetFirstAncestorOfType<LGraphView>();

                // Add new ports if NumberOfRooms has increased
                while (outputContainer.childCount < NumberOfRooms)
                {
                    var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
                    port.portName = $"Out {outputContainer.childCount}";
                    outputContainer.Add(port);
                }

                // Remove ports if NumberOfRooms has decreased
                while (outputContainer.childCount > NumberOfRooms)
                {
                    var portToRemove = outputContainer.Children().Last() as Port;
                    if (portToRemove != null)
                    {
                        if (graphView != null)
                        {
                            // Disconnect and remove all edges connected to this port
                            var edgesToClear = portToRemove.connections.ToList();
                            foreach (var edge in edgesToClear)
                            {
                                edge.input.Disconnect(edge);
                                graphView.RemoveElement(edge);
                            }
                        }
                        outputContainer.Remove(portToRemove);
                    }
                }
                
                RefreshExpandedState();
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