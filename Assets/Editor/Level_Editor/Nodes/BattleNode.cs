using UnityEditor.Experimental.GraphView;

namespace Editor.Level_Editor.Nodes
{
    public class BattleNode : BaseNode
    {
        public BattleNode()
        {
            title = "Battle Room";
            var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "In";
            inputContainer.Add(inputPort);
            
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