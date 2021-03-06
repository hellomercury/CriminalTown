﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CriminalTown.Editors {

    public class QuestNode {
        private readonly Quest m_quest;

        private const float Width = 200;
        private const float Height = 50;

        private Rect m_rect;
        private bool m_isDragged;
        private bool m_isSelected;
        private double m_lastClickTime;
        private const double DoubleClickDelay = 0.5;

        private readonly QuestConnectionPoint m_inPoint;
        private readonly List<QuestConnectionPoint> m_outPoints;

        private readonly GUIStyle m_selectedNodeStyle = new GUIStyle {
            normal = {background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D},
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

        public List<QuestConnectionPoint> OutPoints {
            get {
                return m_outPoints;
            }
        }

        public Quest Quest {
            get {
                return m_quest;
            }
        }

        private GUIStyle m_currentStyle;

        private readonly Action<QuestNode> m_onRemoveNode;

        public QuestNode(Quest quest, Vector2 position, Action<QuestConnectionPoint> onClickInPoint, Action<QuestConnectionPoint> onClickOutPoint, Action<QuestNode> onClickRemoveNode) {
            m_quest = quest;
            m_currentStyle = m_defaultNodeStyle;
            m_rect = new Rect(position.x, position.y, Width, Height);
            m_inPoint = new QuestConnectionPoint(this, ConnectionPointType.In, onClickInPoint);
            if (quest is LinearQuest) {
                m_outPoints = new List<QuestConnectionPoint> {
                    new QuestConnectionPoint(this, ConnectionPointType.Success, onClickOutPoint),
                    new QuestConnectionPoint(this, ConnectionPointType.Fail, onClickOutPoint),
                };
            } else if (quest is ChoiceQuest) {
                m_outPoints = new List<QuestConnectionPoint> {
                    new QuestConnectionPoint(this, ConnectionPointType.Choice1, onClickOutPoint),
                    new QuestConnectionPoint(this, ConnectionPointType.Choice2, onClickOutPoint),
                };
            }
            m_onRemoveNode = onClickRemoveNode;
        }

        public void Drag(Vector2 delta) {
            m_rect.position += delta;
            m_quest.PositionInEditor = m_rect.position;
        }

        public void Draw() {
            m_inPoint.Draw();
            foreach (QuestConnectionPoint outPoint in m_outPoints) {
                outPoint.Draw();
            }
            GUI.Box(m_rect, "", m_currentStyle);
        }

        public void CheckOutOfBordersAndFix() {
            Rect view = QuestsGraphEditor.ScrollViewRect;
            m_rect.x = m_rect.x < view.x ? view.x : m_rect.x;
            m_rect.y = m_rect.y < view.y ? view.y : m_rect.y;
            m_rect.y = m_rect.y > view.y + view.height - m_rect.height ? view.y + view.height - m_rect.height : m_rect.y;
            m_rect.x = m_rect.x > view.x + view.width - m_rect.width ? view.x + view.width - m_rect.width : m_rect.x;
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