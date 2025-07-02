using UnityEditor.Experimental.GraphView;

namespace Level_Editor.Nodes
{
    public class StartNode : BaseNode
    {
        public bool EntryPoint = true;
        
        public StartNode()
        {
            title = "Start";
            AddOutputPorts();
        }

        public sealed override void AddOutputPorts()
        {
            outputContainer.Clear();
            for (var i = 0; i < NumberOfRooms && i < 4; i++)
            {
                var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
                port.portName = $"Out {i}";
                outputContainer.Add(port);
            }
        }
    }
}