using System.Collections.Generic;
using UnityEngine;

namespace Editor.Level_Editor
{
    [System.Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string PortName;
        public string TargetNodeGuid;
    }

    [System.Serializable]
    public class NodeData
    {
        public string Guid;
        public string NodeType;
        public Vector2 Position;
        public List<string> Tags;
        public int NumOutputs;
    }

    public class LevelGraphData : ScriptableObject
    {
        public List<NodeLinkData> LinkData = new List<NodeLinkData>();
        public List<NodeData> NodeData = new List<NodeData>();
    }
}