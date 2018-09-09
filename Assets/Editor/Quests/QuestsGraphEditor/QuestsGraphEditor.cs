using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CriminalTown.Serialization;

namespace CriminalTown.Editors {

    public class QuestsGraphEditor : EditorWindow {

        private static QuestsGraphScriptableObject m_questsGraphData;

        private List<QuestNode> m_nodes;
        private List<QuestConnection> m_connections;

        private QuestConnectionPoint m_selectedInPoint;
        private QuestConnectionPoint m_selectedOutPoint;

        private const int ButtonsPanelHeight = 50;
        private readonly Rect m_saveButtonRect = new Rect(0, 0, 200, ButtonsPanelHeight);
        private readonly Rect m_loadButtonRect = new Rect(200, 0, 200, ButtonsPanelHeight);
        public static readonly Rect ScrollViewRect = new Rect(0, 50, 1280, 1024);
        private Vector2 m_scrollPosition;
        private Rect m_scrollRect;

        [MenuItem("CriminalTown/QuestsGraphEditor")]
        private static void OpenWindow() {
            m_questsGraphData = QuestsGraphScriptableObject.Instance;
            QuestsGraphEditor window = GetWindow<QuestsGraphEditor>();
            window.titleContent = new GUIContent("Quests Graph Editor");
        }

        private void OnGUI() {
            if (GUI.Button(m_saveButtonRect, "Save")) {
                SaveData();
            }
            if (GUI.Button(m_loadButtonRect, "Load")) {
                LoadData();
            }

            m_scrollRect.x = 0;
            m_scrollRect.width = position.width;
            m_scrollRect.y = ButtonsPanelHeight;
            m_scrollRect.height = position.height - ButtonsPanelHeight;
            m_scrollPosition = GUI.BeginScrollView(m_scrollRect, m_scrollPosition, ScrollViewRect);
            {
                DrawGrid(ScrollViewRect, 20, 0.2f, Color.gray);
                DrawGrid(ScrollViewRect, 100, 0.4f, Color.gray);

                DrawNodes();
                DrawConnections();
                DrawConnectionLine(Event.current);

                ProcessNodeEvents(Event.current);
                ProcessEvents(Event.current);
            }
            GUI.EndScrollView();

            if (GUI.changed) {
                Repaint();
            }
        }

        private void OnEnable() {
            minSize = new Vector2(640, 480);
            maxSize = new Vector2(1024, 768);
            //m_scrollRect = new Rect(position.x, position.y + ButtonsPanelHeight, position.width, position.height - ButtonsPanelHeight);
        }

        private static void DrawGrid(Rect rect, float gridSpacing, float gridOpacity, Color gridColor) {
            float width = rect.width;
            float height = rect.height;
            float x = rect.x;
            float y = rect.y;

            int widthDivs = Mathf.CeilToInt(width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);


            for (int i = 0; i < widthDivs; i++) {
                Handles.DrawLine(new Vector3(gridSpacing * i + x, -gridSpacing + y, 0), new Vector3(gridSpacing * i + x, height + y, 0f));
            }

            for (int j = 0; j < heightDivs; j++) {
                Handles.DrawLine(new Vector3(-gridSpacing + x, gridSpacing * j + y, 0), new Vector3(width + x, gridSpacing * j + y, 0f));
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawNodes() {
            if (m_nodes != null) {
                for (int i = 0; i < m_nodes.Count; i++) {
                    m_nodes[i].CheckOutOfBordersAndFix();
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
            genericMenu.AddItem(new GUIContent("Add HireCharacterQuest"), false, () => OnClickAddNode<HireCharacterQuest>(mousePosition));
            genericMenu.AddItem(new GUIContent("Add KickCharacterQuest"), false, () => OnClickAddNode<KickCharacterQuest>(mousePosition));
            genericMenu.AddItem(new GUIContent("Add StatusCharacterQuest"), false, () => OnClickAddNode<StatusCharacterQuest>(mousePosition));
            genericMenu.AddItem(new GUIContent("Add LevelUpCharacterQuest"), false, () => OnClickAddNode<LevelUpCharacterQuest>(mousePosition));
            genericMenu.ShowAsContext();
        }

        private void OnDrag(Vector2 delta) {
            m_scrollPosition -= delta;

            GUI.changed = true;
        }

        private void OnClickAddNode<TQuestType>(Vector2 mousePosition) where TQuestType : Quest, new() {
            if (m_nodes == null) {
                m_nodes = new List<QuestNode>();
            }
            Quest newQuest = ScriptableObject.CreateInstance<TQuestType>();
            newQuest.Id = QuestsGraphScriptableObject.GenerateId();
            newQuest.PositionInEditor = mousePosition;
            QuestNode newNode = new QuestNode(newQuest, mousePosition, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
            m_nodes.Add(newNode);
            m_questsGraphData.AddQuest(newQuest);
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
                    if (m_connections[i].InPoint == node.InPoint ||
                        m_connections[i].OutPoint == node.OutPoints[0] ||
                        m_connections[i].OutPoint == node.OutPoints[1]) {
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
//          int toId = m_selectedInPoint.Node.Quest.Id;
//          int fromId = m_selectedOutPoint.Node.Quest.Id;
//          m_questsGraphData.GetQuestById(fromId);
        }

        private void ClearConnectionSelection() {
            m_selectedInPoint = null;
            m_selectedOutPoint = null;
        }

        private void SaveData() {
            EditorUtility.SetDirty(m_questsGraphData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void LoadData() {
            ReadOnlyCollection<Quest> questsData = m_questsGraphData.Quests;
            if (questsData == null || questsData.Count == 0) {
                return;
            }
            Dictionary<int, KeyValuePair<Quest, QuestNode>> questsDictionary = new Dictionary<int, KeyValuePair<Quest, QuestNode>>();
            m_nodes = new List<QuestNode>();
            m_connections = new List<QuestConnection>();

            foreach (Quest quest in questsData) {
                QuestNode newNode = new QuestNode(quest, quest.PositionInEditor, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
                if (!questsDictionary.ContainsKey(quest.Id)) {
                    questsDictionary.Add(quest.Id, new KeyValuePair<Quest, QuestNode>(quest, newNode));
                } else {
                    Debug.LogError("ID is already exists! " + quest.Id + " " + quest.GetType());
                }
                m_nodes.Add(newNode);
            }

            foreach (int key in questsDictionary.Keys) {
                KeyValuePair<Quest, QuestNode> questInfo = questsDictionary[key];
                Quest quest = questInfo.Key;
                int[] ids = {-1, -1};

                if (quest is LinearQuest) {
                    LinearQuest linearQuest = (LinearQuest) quest;
                    ids[0] = linearQuest.SuccessTransition.NextId;
                    ids[1] = linearQuest.FailTransition.NextId;
                } else if (quest is ChoiceQuest) {
                    ChoiceQuest choiceQuest = (ChoiceQuest) quest;
                    ids[0] = choiceQuest.Choices.Length > 0 ? choiceQuest.Choices[0].NextId : -1;
                    ids[1] = choiceQuest.Choices.Length > 1 ? choiceQuest.Choices[1].NextId : -1;
                }
                for (int i = 0; i < ids.Length; i++) {
                    Debug.Log(ids[i]);
                    if (ids[i] != -1) {
                        if (questsDictionary.ContainsKey(ids[i])) {
                            m_connections.Add(new QuestConnection(
                                questsDictionary[ids[i]].Value.InPoint,
                                questInfo.Value.OutPoints[i],
                                OnClickRemoveConnection
                            ));
                        } else {
                            Debug.LogError("Unknown next quest ID : " + ids[i] + " in quest " + quest.GetType() + " " + quest.Id);
                        }
                    }
                }
            }
        }

    }

}
