using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Level_Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.Level_Editor
{
    public class GraphSaveUtility
    {
        private LGraphView _graphView;
        private LevelGraphData _levelGraphData;
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<BaseNode> Nodes => _graphView.nodes.ToList().Cast<BaseNode>().ToList();

        public static GraphSaveUtility GetInstance(LGraphView graphView)
        {
            return new GraphSaveUtility
            {
                _graphView = graphView
            };
        }

        public void SaveGraph(string fileName)
        {
            var graphData = ScriptableObject.CreateInstance<LevelGraphData>();
            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

            foreach (var node in Nodes)
            {
                graphData.NodeData.Add(new NodeData
                {
                    Guid = node.GUID,
                    Position = node.GetPosition().position,
                    Tags = node.TagsRequired,
                    NumOutputs = node.NumberOfRooms,
                    NodeType = node.GetType().Name
                });
            }

            for (var i = 0; i < connectedPorts.Length; i++)
            {
                 var outputNode = connectedPorts[i].output.node as BaseNode;
                 var inputNode = connectedPorts[i].input.node as BaseNode;

                 graphData.LinkData.Add(new NodeLinkData
                 {
                     BaseNodeGuid = outputNode.GUID,
                     PortName = connectedPorts[i].output.portName,
                     TargetNodeGuid = inputNode.GUID
                 });
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            
            AssetDatabase.CreateAsset(graphData, $"Assets/Resources/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string fileName)
        {
            _levelGraphData = Resources.Load<LevelGraphData>(fileName);
            if (_levelGraphData == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "The specified graph file does not exist!", "OK");
                return;
            }

            ClearGraph();
            CreateNodes();
            ConnectNodes();
        }
        
        private void ClearGraph()
        {
            foreach (var node in Nodes)
            {
                _graphView.RemoveElement(node);
            }
            foreach (var edge in Edges)
            {
                _graphView.RemoveElement(edge);
            }
        }

        private void CreateNodes()
        {
            foreach (var nodeData in _levelGraphData.NodeData)
            {
                var tempNode = (BaseNode)Activator.CreateInstance(System.Type.GetType($"Level_Editor.Windows.{nodeData.NodeType}") ?? throw new InvalidOperationException());
                tempNode.GUID = nodeData.Guid;
                tempNode.SetPosition(new Rect(nodeData.Position, _graphView.DefaultNodeSize));
                tempNode.TagsRequired = nodeData.Tags;
                tempNode.NumberOfRooms = nodeData.NumOutputs;
                
                _graphView.AddElement(tempNode);
            }
        }
        
        private void ConnectNodes()
        {
            var nodes = Nodes;
            for (var i = 0; i < nodes.Count; i++)
            {
                var connections = _levelGraphData.LinkData.Where(x => x.BaseNodeGuid == nodes[i].GUID).ToList();
                for (var j = 0; j < connections.Count; j++)
                {
                    var targetNodeGuid = connections[j].TargetNodeGuid;
                    var targetNode = nodes.First(x => x.GUID == targetNodeGuid);
                    var port = _graphView.ports.ToList().First(x => (x.node as BaseNode).GUID == nodes[i].GUID && x.portName == connections[j].PortName);
                    
                    LinkNodes(port, (Port)targetNode.inputContainer[0]);
                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }
    }
}