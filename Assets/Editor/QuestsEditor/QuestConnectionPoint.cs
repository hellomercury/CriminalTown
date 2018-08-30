using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CriminalTown.Editors {

	public enum ConnectionPointType {
		In,
		Out
	}

	public class QuestConnectionPoint {
		private Rect m_rect;

		private readonly ConnectionPointType m_type;

		private readonly QuestNode m_node;

		private readonly GUIStyle m_style;

		private readonly Action<QuestConnectionPoint> m_onClickConnectionPoint;

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

		public QuestConnectionPoint(QuestNode node, ConnectionPointType type, GUIStyle style, Action<QuestConnectionPoint> onClickConnectionPoint) {
			m_node = node;
			m_type = type;
			m_style = style;
			m_onClickConnectionPoint = onClickConnectionPoint;
			m_rect = new Rect(0, 0, 10f, 20f);
		}

		public void Draw() {
			m_rect.y = m_node.Rect.y + (m_node.Rect.height * 0.5f) - m_rect.height * 0.5f;

			switch (m_type) {
				case ConnectionPointType.In:
					m_rect.x = m_node.Rect.x - m_rect.width + 8f;
					break;

				case ConnectionPointType.Out:
					m_rect.x = m_node.Rect.x + m_node.Rect.width - 8f;
					break;
			}

			if (GUI.Button(m_rect, "", m_style)) {
				if (m_onClickConnectionPoint != null) {
					m_onClickConnectionPoint(this);
				}
			}
		}
	}

}