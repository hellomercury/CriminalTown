using System;
using UnityEditor;
using UnityEngine;

namespace CriminalTown.Editors {

    public class QuestNode {
        private Quest m_quest;

        private const float Width = 200;
        private const float Height = 50;

        private Rect m_rect;
        private bool m_isDragged;
        private bool m_isSelected;
        private double m_lastClickTime;
        private const double DoubleClickDelay = 0.5;

        private readonly QuestConnectionPoint m_inPoint;
        private readonly QuestConnectionPoint m_outPoint;

        private readonly GUIStyle m_selectedNodeStyle = new GUIStyle {
            normal = {background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D},
            border = new RectOffset(4, 4, 12, 12)
        };

        private readonly GUIStyle m_inPointStyle = new GUIStyle {
            normal = {background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D},
            active = {background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D},
            border = new RectOffset(4, 4, 12, 12)
        };

        private readonly GUIStyle m_outPointStyle = new GUIStyle {
            normal = {background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D},
            active = {background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D},
            border = new RectOffset(4, 4, 12, 12)
        };

        private readonly GUIStyle m_defaultNodeStyle = new GUIStyle {
            normal = {background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D},
            border = new RectOffset(12, 12, 12, 12)
        };

        public Rect Rect {
            get {
                return m_rect;
            }
        }

        public QuestConnectionPoint InPoint {
            get {
                return m_inPoint;
            }
        }

        public QuestConnectionPoint OutPoint {
            get {
                return m_outPoint;
            }
        }

        public Quest Quest {
            get {
                return m_quest;
            }
        }

        private GUIStyle m_currentStyle;

        private readonly Action<QuestNode> m_onRemoveNode;

        public QuestNode(Vector2 position, Action<QuestConnectionPoint> onClickInPoint, Action<QuestConnectionPoint> onClickOutPoint, Action<QuestNode> onClickRemoveNode) {
            m_currentStyle = m_defaultNodeStyle;
            m_rect = new Rect(position.x, position.y, Width, Height);
            m_inPoint = new QuestConnectionPoint(this, ConnectionPointType.In, m_inPointStyle, onClickInPoint);
            m_outPoint = new QuestConnectionPoint(this, ConnectionPointType.Out, m_outPointStyle, onClickOutPoint);
            m_onRemoveNode = onClickRemoveNode;
        }

        public void Drag(Vector2 delta) {
            m_rect.position += delta;
        }

        public void Draw() {
            m_inPoint.Draw();
            m_outPoint.Draw();
            GUI.Box(m_rect, "", m_currentStyle);
        }

        public void CheckOutOfBordersAndFix() {
            Rect view = QuestsGraphEditor.ScrollViewRect;
            m_rect.x = m_rect.x < view.x ? view.x : m_rect.x;
            m_rect.y = m_rect.y < view.y ? view.y : m_rect.y;
            m_rect.y = m_rect.y > view.y + view.height ? view.y + view.height : m_rect.y;
            m_rect.x = m_rect.x > view.x + view.width ? view.x + view.width : m_rect.x;
        }

        public bool ProcessEvents(Event e) {
            switch (e.type) {
                case EventType.MouseDown:
                    if (e.button == 0) {
                        if (m_rect.Contains(e.mousePosition)) {
                            if (EditorApplication.timeSinceStartup - m_lastClickTime < DoubleClickDelay) {

                            }
                            m_lastClickTime = EditorApplication.timeSinceStartup;
                            m_isDragged = true;
                            GUI.changed = true;
                            m_isSelected = true;
                            m_currentStyle = m_selectedNodeStyle;
                        } else {
                            GUI.changed = true;
                            m_isSelected = false;
                            m_currentStyle = m_defaultNodeStyle;
                        }
                    }

                    if (e.button == 1 && m_isSelected && m_rect.Contains(e.mousePosition)) {
                        ProcessContextMenu();
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    m_isDragged = false;
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && m_isDragged) {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void ProcessContextMenu() {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
            genericMenu.ShowAsContext();
        }

        private void OnClickRemoveNode() {
            if (m_onRemoveNode != null) {
                m_onRemoveNode(this);
            }
        }
    }

}