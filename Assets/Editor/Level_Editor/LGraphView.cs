using System;
using System.Collections.Generic;
using System.Linq;
using Level_Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Level_Editor
{
    public class LGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);
        public GraphProperties GraphProperties = new GraphProperties();

        private List<RoomData> _roomPrefabs;
        private List<Bounds> _generatedRoomBounds;

        public LGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Level_Editor/LevelEditorStyleSheet"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GenerateEntryPointNode());
            LoadRoomPrefabs();
        }

        private void LoadRoomPrefabs()
        {
            _roomPrefabs = new List<RoomData>();
            var guids = AssetDatabase.FindAssets("t:GameObject", new[] { "Assets/Prefabs/Rooms" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var roomPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (roomPrefab != null)
                {
                    var roomComponent = roomPrefab.GetComponent<Room>();
                    if (roomComponent != null)
                    {
                        _roomPrefabs.Add(new RoomData { Prefab = roomPrefab, Tags = roomComponent.Tags });
                    }
                }
            }
        }

        private StartNode GenerateEntryPointNode()
        {
            var node = new StartNode
            {
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                EntryPoint = true
            };
            node.SetPosition(new Rect(100, 200, 100, 150));
            return node;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node).ToList();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var graphMousePosition = contentViewContainer.WorldToLocal(evt.mousePosition);
            evt.menu.AppendAction("Add Battle Node", (a) => AddNode(CreateBattleNode(graphMousePosition)));
        }

        private void AddNode(BaseNode node)
        {
            AddElement(node);
        }

        private BattleNode CreateBattleNode(Vector2 position)
        {
            var node = new BattleNode
            {
                title = "Battle Room",
                GUID = Guid.NewGuid().ToString(),
            };
            node.SetPosition(new Rect(position, DefaultNodeSize));
            return node;
        }

        public void GenerateLevel()
        {
            if (nodes.FirstOrDefault(n => n is StartNode) is not StartNode entryNode)
            {
                EditorUtility.DisplayDialog("Error", "No Start Node found in the graph.", "OK");
                return;
            }

            var oldRooms = GameObject.FindGameObjectsWithTag("Room");
            foreach (var room in oldRooms)
            {
                Object.DestroyImmediate(room);
            }

            _generatedRoomBounds = new List<Bounds>();
            var generatedNodes = new HashSet<BaseNode>();
            TraverseAndGenerate(entryNode, GraphProperties.StartPosition, Quaternion.identity, generatedNodes);
        }

        private bool TraverseAndGenerate(BaseNode nodeToPlace, Vector3 parentEntryPointPos, Quaternion parentEntryPointRot, HashSet<BaseNode> generatedNodes)
        {

            if (!generatedNodes.Add(nodeToPlace))
            {
                return true; 
            }

            var roomChangeAttempts = GraphProperties.RoomChangeAttempts;
            while (roomChangeAttempts > 0)
            {
                var selectedRoomData = GetRandomRoom(nodeToPlace.TagsRequired);
                if (selectedRoomData.Prefab == null)
                {
                    Debug.LogWarning($"No room found with tags: {string.Join(", ", nodeToPlace.TagsRequired)} for node {nodeToPlace.title}.");
                    break; 
                }

                var roomInstance = (GameObject)PrefabUtility.InstantiatePrefab(selectedRoomData.Prefab);
                roomInstance.tag = "Room";
                var allEntryPoints = GetEntryPoints(roomInstance);
                if (allEntryPoints.Count == 0)
                {
                    Debug.LogError($"Prefab {roomInstance.name} has no 'EntryPoint' objects!");
                    Object.DestroyImmediate(roomInstance);
                    break;
                }

                var availableEntryPoints = new List<Transform>(allEntryPoints);
                while (availableEntryPoints.Count > 0)
                {
                    var randomEntryPoint = availableEntryPoints[UnityEngine.Random.Range(0, availableEntryPoints.Count)];
                    availableEntryPoints.Remove(randomEntryPoint);

                    roomInstance.transform.rotation = parentEntryPointRot * Quaternion.Inverse(randomEntryPoint.localRotation);
                    roomInstance.transform.position = parentEntryPointPos - (roomInstance.transform.rotation * randomEntryPoint.localPosition);

                    if (!CheckOverlap(roomInstance))
                    {

                        var roomCollider = roomInstance.GetComponent<Collider>();
                        bool boundsAdded = false;
                        if (roomCollider != null)
                        {
                            var newRoomBounds = roomCollider.bounds;
                            newRoomBounds.center += roomInstance.transform.position;
                            _generatedRoomBounds.Add(newRoomBounds);
                            boundsAdded = true;
                        }
                        if(randomEntryPoint.TryGetComponent<IDoor>(out var door)) door.OpenDoor();

                        var availableChildPorts = allEntryPoints.Where(e => e != randomEntryPoint).ToList();
                        var childGraphNodes = nodeToPlace.outputContainer.Children().Cast<Port>()
                            .Where(p => p.connected)
                            .Select(p => p.connections.First().input.node as BaseNode)
                            .Where(n => n != null)
                            .ToList();

                        bool allChildrenPlaced = true;
                        if (childGraphNodes.Count > 0)
                        {
                            var nodesForChildren = new HashSet<BaseNode>(generatedNodes);
                            foreach (var childNode in childGraphNodes)
                            {
                                if (availableChildPorts.Count == 0)
                                {
                                    allChildrenPlaced = false;
                                    break;
                                }

                                bool childPlacedSuccessfully = false;
                                int portRetryCount = Mathf.Min(GraphProperties.EntryPointSwaps, availableChildPorts.Count);
                                for(int i = 0; i < portRetryCount; i++)
                                {
                                    var selectedPort = availableChildPorts[UnityEngine.Random.Range(0, availableChildPorts.Count)];
                                    if (TraverseAndGenerate(childNode, selectedPort.position, selectedPort.rotation, nodesForChildren))
                                    {
                                        childPlacedSuccessfully = true;
                                        if(selectedPort.TryGetComponent<IDoor>(out door)) door.OpenDoor();
                                        availableChildPorts.Remove(selectedPort);
                                        break;
                                    }
                                }

                                if (!childPlacedSuccessfully)
                                {
                                    allChildrenPlaced = false;
                                    break;
                                }
                            }
                        }

                        if (allChildrenPlaced)
                        {

                            return true;
                        }
                        else
                        {

                            if (boundsAdded)
                            {
                                _generatedRoomBounds.RemoveAt(_generatedRoomBounds.Count - 1);
                            }
                        }

                    }
                }

                Object.DestroyImmediate(roomInstance);
                roomChangeAttempts--;
            }

            generatedNodes.Remove(nodeToPlace);
            return false;
        }

        private bool TryPlaceRoomAndGenerateChildren(BaseNode node, Vector3 parentEntryPointPos, Quaternion parentEntryPointRot, HashSet<BaseNode> generatedNodes)
        {
            var roomChangeAttempts = GraphProperties.RoomChangeAttempts;
            while (roomChangeAttempts > 0)
            {
                var selectedRoomData = GetRandomRoom(node.TagsRequired);
                if (selectedRoomData.Prefab == null)
                {
                    Debug.LogWarning($"No room found with tags: {string.Join(", ", node.TagsRequired)} for node {node.title}.");
                    return false; 
                }

                var roomInstance = (GameObject)PrefabUtility.InstantiatePrefab(selectedRoomData.Prefab);
                roomInstance.tag = "Room";

                var entryPoints = GetEntryPoints(roomInstance);
                if (entryPoints.Count == 0)
                {
                    Debug.LogError($"Prefab {roomInstance.name} has no 'EntryPoint' objects!");
                    Object.DestroyImmediate(roomInstance);
                    return false; 
                }

                var entryPointSwaps = GraphProperties.EntryPointSwaps;
                while (entryPointSwaps > 0)
                {
                    var randomEntryPoint = entryPoints[UnityEngine.Random.Range(0, entryPoints.Count)];

                    roomInstance.transform.rotation = parentEntryPointRot * Quaternion.Inverse(randomEntryPoint.localRotation);
                    roomInstance.transform.position = parentEntryPointPos - (roomInstance.transform.rotation * randomEntryPoint.localPosition);

                    if (!CheckOverlap(roomInstance))
                    {

                        var availableChildPorts = GetEntryPoints(roomInstance).Where(e => e != randomEntryPoint).ToList();

                        bool childrenGeneratedSuccessfully = GenerateChildren(node, availableChildPorts, generatedNodes);

                        if (childrenGeneratedSuccessfully)
                        {

                            var roomCollider = roomInstance.GetComponent<Collider>();
                            if (roomCollider != null)
                            {
                                var newRoomBounds = roomCollider.bounds;
                                newRoomBounds.center += roomCollider.transform.position;
                                _generatedRoomBounds.Add(newRoomBounds);
                            }
                            return true; 
                        }

                    }
                    entryPointSwaps--;
                }

                Object.DestroyImmediate(roomInstance);
                roomChangeAttempts--;
            }

            Debug.LogWarning($"All placement attempts failed for node {node.title}.");
            return false; 
        }

        private bool GenerateChildren(BaseNode parentNode, List<Transform> availablePorts, HashSet<BaseNode> generatedNodes)
        {
            var childrenToGenerate = Mathf.Min(parentNode.NumberOfRooms, availablePorts.Count);
            var successfulChildren = 0;

            for (int i = 0; i < childrenToGenerate; i++)
            {
                bool childGenerated = false;
                int attempts = 0;

                int maxAttempts = GraphProperties.EntryPointSwaps;

                while (!childGenerated && availablePorts.Count > 0 && attempts < maxAttempts)
                {
                    int randomIndex = UnityEngine.Random.Range(0, availablePorts.Count);
                    var selectedPort = availablePorts[randomIndex];
                    availablePorts.RemoveAt(randomIndex); 

                    if (TraverseAndGenerate(parentNode, selectedPort.position, selectedPort.rotation, generatedNodes))
                    {
                        childGenerated = true;
                        successfulChildren++;
                    }
                    attempts++;
                }

                if (!childGenerated)
                {

                    return false;
                }
            }

            return successfulChildren >= childrenToGenerate;
        }

        private RoomData GetRandomRoom(List<string> tags)
        {
            var validRooms = _roomPrefabs.Where(r => tags.All(t => r.Tags.Contains(t))).ToList();
            if (validRooms.Count > 0)
            {
                return validRooms[UnityEngine.Random.Range(0, validRooms.Count)];
            }
            return new RoomData();
        }

        private List<Transform> GetEntryPoints(GameObject roomInstance)
        {
            return roomInstance.GetComponentsInChildren<Transform>().Where(t => t.name.StartsWith("EntryPoint")).ToList();
        }

        private bool CheckOverlap(GameObject room)
        {
            var roomCollider = room.GetComponent<Collider>();
            if (roomCollider == null)
            {
                Debug.LogError($"Room prefab '{room.name}' must have a Collider component for overlap detection.", room);
                return true; 
            }

            var newRoomBounds = roomCollider.bounds;
            newRoomBounds.center += room.transform.position;

            return _generatedRoomBounds.Any(existingBounds => existingBounds.Intersects(newRoomBounds));
        }
    }

    [Serializable]
    public struct RoomData
    {
        public GameObject Prefab;
        public List<string> Tags;
    }
}