using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DS.Windows
{
    using Data.Error;
    using Elements;  
    using Enumerations;
    using Utilities;  

    public class DSGraphView : GraphView
    {
        private DSEditorWindow editorWindow;
        private DSSearchWindow searchWindow;

        private SerializableDictionary<string, DSNodeErrorData> ungroupedNodes;
        private SerializableDictionary<string, DSGroupErrorData> groups;
        private SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>> groupedNodes;

        private int repeatedNamesAmount;
        
        public int RepeatedNamesAmount
        {
            get
            {
                return repeatedNamesAmount;
            }
            set
            {
                repeatedNamesAmount = value;

                if (repeatedNamesAmount == 0)
                {
                    editorWindow.EnableSaving();
                }

                if (repeatedNamesAmount == 1)
                {
                    editorWindow.DisableSaving();
                }
            }
        }

        public DSGraphView(DSEditorWindow dsEditorWindow)
        {
            editorWindow = dsEditorWindow;

            ungroupedNodes = new SerializableDictionary<string, DSNodeErrorData>();
            groups = new SerializableDictionary<string, DSGroupErrorData>();
            groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>>();

            AddManipulators();
            AddGridBackground();
            AddSearchWindow();

            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();

            AddStyles();
        }

        #region Métodos Override
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
        #endregion

        #region Manipuladores
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            

            this.AddManipulator(CreateNodeContextualMenu("Añadir Nodo (Opción Única)", DSDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Añadir Nodo (Opción Múltiple)", DSDialogueType.MultipleChoice));

            this.AddManipulator(CreateGroupContextualMenu());
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Añadir Grupo", actionEvent => CreateGroup("GrupoDeDiálogos", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }

        
        private IManipulator CreateNodeContextualMenu(string actionTitle, DSDialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );

            return contextualMenuManipulator;
        }
        #endregion

        #region Creación de Elementos
        public DSGroup CreateGroup(string title, Vector2 localMousePosition)
        {
            DSGroup group = new DSGroup(title, localMousePosition);

            AddGroup(group);
            
            AddElement(group);

            foreach (GraphElement selectedElement in selection)
            {
                if (!(selectedElement is DSNode))
                {
                    continue;
                }
                
                DSNode node = (DSNode) selectedElement;
                
                group.AddElement(node);
            }

            return group;
        }

        public DSNode CreateNode(DSDialogueType dialogueType, Vector2 position)
        {
            Type nodeType = Type.GetType($"DS.Elements.DS{dialogueType}Node");
            DSNode node = (DSNode) Activator.CreateInstance(nodeType);

            node.Initialize(this, position);
            node.Draw();

            AddUngroupedNode(node);

            return node;
        }
        #endregion

        #region Elementos Repetidos
        public void AddUngroupedNode(DSNode node)
        {
            string nodeName = node.DialogueName.ToLower();

            if (!ungroupedNodes.ContainsKey(nodeName))
            {
                DSNodeErrorData nodeErrorData = new DSNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                ungroupedNodes.Add(nodeName, nodeErrorData);

                return;
            }

            List<DSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Add(node);

            Color errorColor = ungroupedNodes[nodeName].ErrorData.Color;

            node.SetErrorStyle(errorColor);

            if (ungroupedNodesList.Count == 2)
            {
                ++RepeatedNamesAmount;
                
                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveUngroupedNode(DSNode node)
        {
            string nodeName = node.DialogueName.ToLower();

            List<DSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Remove(node);

            node.ResetStyle();

            if (ungroupedNodesList.Count == 1)
            {
                --RepeatedNamesAmount;
                ungroupedNodesList[0].ResetStyle();

                return;
            }

            if (ungroupedNodesList.Count == 0)
            {
                ungroupedNodes.Remove(nodeName);
            }
        }
        
        private void AddGroup(DSGroup group)
        {
            string groupName = group.title.ToLower();

            if (!groups.ContainsKey(groupName))
            {
                DSGroupErrorData groupErrorData = new DSGroupErrorData();
                
                groupErrorData.Groups.Add(group);
                
                groups.Add(groupName, groupErrorData);

                return;
            }
            
            List<DSGroup> groupsList = groups[groupName].Groups;
            
            groupsList.Add(group);
            
            Color errorColor = groups[groupName].ErrorData.Color;
            
            group.SetErrorStyle(errorColor);

            if (groupsList.Count == 2)
            {
                ++RepeatedNamesAmount;
                groupsList[0].SetErrorStyle(errorColor);
            }
        }
        public void AddGroupedNode(DSNode node, DSGroup group)
        {
            string nodeName = node.DialogueName.ToLower();

            node.Group = group;

            if (!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new SerializableDictionary<string, DSNodeErrorData>());
            }

            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                DSNodeErrorData nodeErrorData = new DSNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                groupedNodes[group].Add(nodeName, nodeErrorData);

                return;
            }

            List<DSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Add(node);

            Color errorColor = groupedNodes[group][nodeName].ErrorData.Color;

            node.SetErrorStyle(errorColor);

            if (groupedNodesList.Count ==2)
            {
                ++RepeatedNamesAmount;
                groupedNodesList[0].SetErrorStyle(errorColor);
            }
        }
        
        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode))
                    {
                        continue;
                    }
                    
                    DSGroup nodeGroup = (DSGroup) group;
                    DSNode node = (DSNode) element;

                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, nodeGroup);
                }
            };
        }


        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode))
                    {
                        continue;
                    }

                    DSNode node = (DSNode) element;

                    RemoveGroupedNode(node, group);
                    AddUngroupedNode(node);
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                DSGroup dsGroup = (DSGroup)group;
                
                dsGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

                RemoveGroup(dsGroup);

                dsGroup.oldTitle = dsGroup.title;

                AddGroup(dsGroup);
            };
        }

        public void RemoveGroupedNode(DSNode node, Group group)
        {
            string nodeName = node.DialogueName.ToLower();

            node.Group = null;

            List<DSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Remove(node);

            node.ResetStyle();

            if (groupedNodesList.Count == 1)
            {
                --RepeatedNamesAmount;
                
                groupedNodesList[0].ResetStyle();

                return;
            }

            if (groupedNodesList.Count == 0)
            {
                groupedNodes[group].Remove(nodeName);

                if (groupedNodes[group].Count == 0)
                {
                    groupedNodes.Remove(group);
                }
            }
        }
        #endregion

        #region Callbacks
        private void OnElementsDeleted()
        {
            deleteSelection = (OperationName, AskUser) =>
            {
                Type groupType = typeof(DSGroup);
                Type edgeType = typeof(Edge);
            
                List<DSGroup> groupsToDelete = new List<DSGroup>();
                List<Edge> edgesToDelete = new List<Edge>();
                List<DSNode> nodesToDelete = new List<DSNode>();
                
                foreach (GraphElement element in selection)
                {
                    if (element is DSNode node)
                    {
                        nodesToDelete.Add(node);

                        continue;
                    }

                    if (element.GetType() == edgeType)
                    {
                        Edge edge = (Edge) element;
                        
                        edgesToDelete.Add(edge);
                        
                        continue;
                    }

                    if (element.GetType() != groupType)
                    {
                        continue;
                    }
                    
                    DSGroup group = (DSGroup) element;
                    
                    groupsToDelete.Add(group);
                }

                foreach (DSGroup group in groupsToDelete)
                {
                    List<DSNode> groupNodes = new List<DSNode>();

                    foreach (GraphElement groupElement in group.containedElements)
                    {
                        if (!(groupElement is DSNode))
                        {
                            continue;
                        }
                        
                        DSNode groupNode = (DSNode) groupElement;
                        
                        groupNodes.Add(groupNode);
                    }
                    
                    group.RemoveElements(groupNodes);
                    
                    RemoveGroup(group);
                    
                    RemoveElement(group);
                }
                
                DeleteElements(edgesToDelete);

                foreach (DSNode node in nodesToDelete)
                {
                    if (node.Group != null)
                    {
                        node.Group.RemoveElement(node);
                    }
                    
                    RemoveUngroupedNode(node);
                    
                    node.DisconnectAllPorts();

                    RemoveElement(node);
                }
            };
        }

        private void RemoveGroup(DSGroup group)
        {
            string oldGroupName = group.oldTitle.ToLower();
            
            List<DSGroup> groupsList = groups[oldGroupName].Groups;
            
            groupsList.Remove(group);
            
            group.ResetStyle();

            if (groupsList.Count == 1)
            {
                --RepeatedNamesAmount;
                
                groupsList[0].ResetStyle();
            }

            if (groupsList.Count == 0)
            {
                groups.Remove(oldGroupName);
            }
        }

        #endregion

        #region Adición de Elementos

        private void AddSearchWindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();
            }

            searchWindow.Initialize(this);

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        private void AddStyles()
        {
            this.AddStyleSheets(
                "DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss"
            );
        }
        #endregion

        #region Utilidades
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, mousePosition - editorWindow.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }
        #endregion
    }
}
