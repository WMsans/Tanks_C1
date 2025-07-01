using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Level_Editor.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor.Level_Editor
{
    public class LGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);

        private List<RoomData> _roomPrefabs;

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
            string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { "Assets/Prefabs/Rooms" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject roomPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (roomPrefab != null)
                {
                    Room roomComponent = roomPrefab.GetComponent<Room>();
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
            evt.menu.AppendAction("Add Battle Node", (a) => AddNode(CreateBattleNode()));
        }

        private void AddNode(BaseNode node)
        {
            AddElement(node);
        }

        private BattleNode CreateBattleNode()
        {
            var node = new BattleNode
            {
                title = "Battle Room",
                GUID = Guid.NewGuid().ToString(),
            };
            return node;
        }
        
        public void GenerateLevel()
        {
            var entryNode = nodes.First(n => n is StartNode) as StartNode;
            if (entryNode == null)
            {
                EditorUtility.DisplayDialog("Error", "No Start Node found in the graph.", "OK");
                return;
            }
            
            // Clear previous level
            GameObject[] oldRooms = GameObject.FindGameObjectsWithTag("Room");
            foreach (var room in oldRooms)
            {
                Object.DestroyImmediate(room);
            }

            TraverseAndGenerate(entryNode, Vector3.zero, Quaternion.identity);
        }

        private void TraverseAndGenerate(BaseNode parentNode, Vector3 parentEntryPointPos, Quaternion parentEntryPointRot)
        {
            var outputPorts = parentNode.outputContainer.Children().Cast<Port>();

            foreach (var port in outputPorts)
            {
                if (!port.connected) continue;

                var edge = port.connections.First();
                var childNode = edge.input.node as BaseNode;

                int roomChangeAttempts = 5;
                while (roomChangeAttempts > 0)
                {
                    RoomData selectedRoomData = GetRandomRoom(childNode.TagsRequired);
                    if (selectedRoomData.Prefab == null)
                    {
                        Debug.LogWarning($"No room found with tags: {string.Join(", ", childNode.TagsRequired)} for node {childNode.title}.");
                        break; 
                    }
                    
                    GameObject roomInstance = (GameObject)PrefabUtility.InstantiatePrefab(selectedRoomData.Prefab);
                    roomInstance.tag = "Room"; // Ensure prefabs are tagged for easy cleanup

                    var entryPoints = GetEntryPoints(roomInstance);
                    if (entryPoints.Count == 0)
                    {
                        Debug.LogError($"Prefab {roomInstance.name} has no 'EntryPoint' objects!");
                        Object.DestroyImmediate(roomInstance);
                        break; 
                    }

                    bool placementSuccess = false;
                    int entryPointSwaps = 5;
                    while (entryPointSwaps > 0)
                    {
                        Transform randomEntryPoint = entryPoints[UnityEngine.Random.Range(0, entryPoints.Count)];
                        
                        roomInstance.transform.rotation = parentEntryPointRot * Quaternion.Inverse(randomEntryPoint.localRotation);
                        roomInstance.transform.position = parentEntryPointPos - (roomInstance.transform.rotation * randomEntryPoint.localPosition);

                        if (!CheckOverlap(roomInstance))
                        {
                            placementSuccess = true;
                            
                            var childOutputPorts = GetEntryPoints(roomInstance).Where(e => e != randomEntryPoint).ToList();
                            int childrenToGenerate = Mathf.Min(childNode.NumberOfRooms, childOutputPorts.Count);
                            
                            for (int i = 0; i < childrenToGenerate; i++)
                            {
                                TraverseAndGenerate(childNode, childOutputPorts[i].position, childOutputPorts[i].rotation);
                            }
                            break; 
                        }
                        entryPointSwaps--;
                    }

                    if (placementSuccess)
                    {
                        break; 
                    }
                    else
                    {
                        Object.DestroyImmediate(roomInstance);
                        roomChangeAttempts--;
                    }
                }
            }
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
            Collider roomCollider = room.GetComponent<Collider>();
            if (roomCollider == null)
            {
                 Debug.LogError($"Room prefab {room.name} is missing a Collider component for overlap checks.");
                 return false;
            }

            Collider[] colliders = Physics.OverlapBox(room.transform.position + roomCollider.bounds.center, roomCollider.bounds.extents, room.transform.rotation);
            foreach (var col in colliders)
            {
                if(col.gameObject != room && col.CompareTag("Room"))
                {
                    return true;
                }
            }
            return false;
        }
    }
    
    public struct RoomData
    {
        public GameObject Prefab;
        public List<string> Tags;
    }
}