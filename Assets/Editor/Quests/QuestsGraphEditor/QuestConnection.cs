using System;
using UnityEditor;
using UnityEngine;

namespace CriminalTown.Editors {


	public class QuestConnection {
		private readonly QuestConnectionPoint m_inPoint;
		private readonly QuestConnectionPoint m_outPoint;
		private readonly Action<QuestConnection> m_onClickRemoveConnection;

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

		public QuestConnection(QuestConnectionPoint inPoint, QuestConnectionPoint outPoint, Action<QuestConnection> onClickRemoveConnection) {
			m_inPoint = inPoint;
			m_outPoint = outPoint;
			m_onClickRemoveConnection = onClickRemoveConnection;
		}

		public void Draw() {
			Handles.DrawBezier(
				m_inPoint.Rect.center,
				m_outPoint.Rect.center,
				m_inPoint.Rect.center + Vector2.left * 50f,
				m_outPoint.Rect.center - Vector2.left * 50f,
				Color.white,
				null,
				2f
			);

			if (Handles.Button((m_inPoint.Rect.center + m_outPoint.Rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap)) {
				if (m_onClickRemoveConnection != null) {
					m_onClickRemoveConnection(this);
				}
			}
		}
	}

}