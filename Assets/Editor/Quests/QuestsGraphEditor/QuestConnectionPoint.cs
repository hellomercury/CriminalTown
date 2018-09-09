using System;
using UnityEditor;
using UnityEngine;

namespace CriminalTown.Editors {

	public enum ConnectionPointType {
		In,
		Success,
		Fail,
		Choice1,
		Choice2
	}

	public class QuestConnectionPoint {
		private Rect m_rect;

		private readonly ConnectionPointType m_pointType;

		private readonly QuestNode m_node;

		private readonly Action<QuestConnectionPoint> m_onClickConnectionPoint;

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

		public ConnectionPointType PointType {
			get {
				return m_pointType;
			}
		}

		public QuestNode Node {
			get {
				return m_node;
			}
		}

		public Rect Rect {
			get {
				return m_rect;
			}
		}

		public QuestConnectionPoint(QuestNode node, ConnectionPointType type, Action<QuestConnectionPoint> onClickConnectionPoint) {
			m_node = node;
			m_pointType = type;
			m_onClickConnectionPoint = onClickConnectionPoint;
			m_rect = new Rect(0, 0, 10f, 20f);
		}

		public void Draw() {
			m_rect.y = m_node.Rect.y + (m_node.Rect.height * 0.5f) - m_rect.height * 0.5f;
			GUIStyle style = m_outPointStyle;
			switch (m_pointType) {
				case ConnectionPointType.In:
					m_rect.x = m_node.Rect.x - m_rect.width + 8f;
					style = m_inPointStyle;
					break;
				case ConnectionPointType.Success:
					m_rect.x = m_node.Rect.x + m_node.Rect.width - 8f;
					m_rect.y += 8f;
					break;
				case ConnectionPointType.Fail:
					m_rect.x = m_node.Rect.x + m_node.Rect.width - 8f;
					m_rect.y -= 8f;
					break;
				case ConnectionPointType.Choice1:
					m_rect.x = m_node.Rect.x + m_node.Rect.width - 8f;
					m_rect.y += 8f;
					break;
				case ConnectionPointType.Choice2:
					m_rect.x = m_node.Rect.x + m_node.Rect.width - 8f;
					m_rect.y -= 8f;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (GUI.Button(m_rect, "", style)) {
				if (m_onClickConnectionPoint != null) {
					m_onClickConnectionPoint(this);
				}
			}
		}

	}

}