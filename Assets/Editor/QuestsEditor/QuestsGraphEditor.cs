using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using CriminalTown.Serialization;

namespace CriminalTown.Editors {

    public class NodeBasedEditor : EditorWindow {
        private static QuestsGraphScriptableObject m_questsGraphData;
        
        private List<QuestNode> m_nodes;
        private List<QuestConnection> m_connections;

        private QuestConnectionPoint m_selectedInPoint;
        private QuestConnectionPoint m_selectedOutPoint;

        private Vector2 m_offset;
        private Vector2 m_drag;
        
        private readonly Rect m_saveButtonRect = new Rect(0, 0, 200, 50);
        private readonly Rect m_loadButtonRect = new Rect(200, 0, 200, 50);

        [MenuItem("CriminalTown/QuestGraphEditor")]
        private static void OpenWindow() {
            m_questsGraphData = QuestsSerialization.GetQuestsGraphData;
            NodeBasedEditor window = GetWindow<NodeBasedEditor>();
            window.titleContent = new GUIContent("Node Based Editor");
        }

        private void OnGUI() {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            DrawNodes();
            DrawConnections();

            DrawConnectionLine(Event.current);

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if (GUI.Button(m_saveButtonRect, "Save")) {
                
            }
            if (GUI.Button(m_loadButtonRect, "Load")) {
                
            }

            if (GUI.changed)
                Repaint();
        }

        private void OnEnable() {
            minSize = new Vector2(640, 480); 
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor) {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            m_offset += m_drag * 0.5f;
            Vector3 newOffset = new Vector3(m_offset.x % gridSpacing, m_offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++) {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++) {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawNodes() {
            if (m_nodes != null) {
                for (int i = 0; i < m_nodes.Count; i++) {
                    m_nodes[i].Draw();
                }
            }
        }

        private void DrawConnections() {
            if (m_connections != null) {
                for (int i = 0; i < m_connections.Count; i++) {
                    m_connections[i].Draw();
                }
            }
        }

        private void ProcessEvents(Event e) {
            m_drag = Vector2.zero;

            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 0) {
                        ClearConnectionSelection();
                    }

                    if (e.button == 1) {
                        ProcessContextMenu(e.mousePosition);
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0) {
                        OnDrag(e.delta);
                    }
                    break;
            }
        }

        private void ProcessNodeEvents(Event e) {
            if (m_nodes != null) {
                for (int i = m_nodes.Count - 1; i >= 0; i--) {
                    bool guiChanged = m_nodes[i].ProcessEvents(e);

                    if (guiChanged) {
                        GUI.changed = true;
                    }
                }
            }
        }

        private void DrawConnectionLine(Event e) {
            if (m_selectedInPoint != null && m_selectedOutPoint == null) {
                Handles.DrawBezier(
                    m_selectedInPoint.Rect.center,
                    e.mousePosition,
                    m_selectedInPoint.Rect.center + Vector2.left * 50f,
                    e.mousePosition - Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }

            if (m_selectedOutPoint != null && m_selectedInPoint == null) {
                Handles.DrawBezier(
                    m_selectedOutPoint.Rect.center,
                    e.mousePosition,
                    m_selectedOutPoint.Rect.center - Vector2.left * 50f,
                    e.mousePosition + Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }
        }

        private void ProcessContextMenu(Vector2 mousePosition) {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
            genericMenu.ShowAsContext();
        }

        private void OnDrag(Vector2 delta) {
            m_drag = delta;

            if (m_nodes != null) {
                for (int i = 0; i < m_nodes.Count; i++) {
                    m_nodes[i].Drag(delta);
                }
            }

            GUI.changed = true;
        }

        private void OnClickAddNode(Vector2 mousePosition) {
            if (m_nodes == null) {
                m_nodes = new List<QuestNode>();
            }

            m_nodes.Add(new QuestNode(mousePosition, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
        }

        private void OnClickInPoint(QuestConnectionPoint inPoint) {
            m_selectedInPoint = inPoint;

            if (m_selectedOutPoint != null) {
                if (m_selectedOutPoint.Node != m_selectedInPoint.Node) {
                    CreateConnection();
                    ClearConnectionSelection();
                } else {
                    ClearConnectionSelection();
                }
            }
        }

        private void OnClickOutPoint(QuestConnectionPoint outPoint) {
            m_selectedOutPoint = outPoint;

            if (m_selectedInPoint != null) {
                if (m_selectedOutPoint.Node != m_selectedInPoint.Node) {
                    CreateConnection();
                    ClearConnectionSelection();
                } else {
                    ClearConnectionSelection();
                }
            }
        }

        private void OnClickRemoveNode(QuestNode node) {
            if (m_connections != null) {
                List<QuestConnection> connectionsToRemove = new List<QuestConnection>();
                for (int i = 0; i < m_connections.Count; i++) {
                    if (m_connections[i].InPoint == node.InPoint || m_connections[i].OutPoint == node.OutPoint) {
                        connectionsToRemove.Add(m_connections[i]);
                    }
                }
                for (int i = 0; i < connectionsToRemove.Count; i++) {
                    m_connections.Remove(connectionsToRemove[i]);
                }
            }

            m_nodes.Remove(node);
        }

        private void OnClickRemoveConnection(QuestConnection connection) {
            m_connections.Remove(connection);
        }

        private void CreateConnection() {
            if (m_connections == null) {
                m_connections = new List<QuestConnection>();
            }
            m_connections.Add(new QuestConnection(m_selectedInPoint, m_selectedOutPoint, OnClickRemoveConnection));
        }

        private void ClearConnectionSelection() {
            m_selectedInPoint = null;
            m_selectedOutPoint = null;
        }

        private void SaveData() {
            
        }

        private void LoadData() {
            
        }
    }

}
