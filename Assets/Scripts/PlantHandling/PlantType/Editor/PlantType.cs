using UnityEngine;
using UnityEditor;
using PlantHandling.PlantType;
using Sirenix.OdinInspector;

namespace PlantHandling.PlantType.Editor
{
	[CustomEditor(typeof(APlantType))]
	public class PlantType : UnityEditor.Editor
	{
		private GUIStyle _emptyStyle;
		private GUIStyle _hoverStyle;
		private GUIStyle _filledStyle;
		private Vector2Int _shapeSize;
		private APlantType _plantTarget;

		private void ShapeSizeUI()
		{
			_shapeSize = _plantTarget.ShapeSize;
			GUILayout.BeginHorizontal();
			GUILayout.Label("Plant Size : ");

			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label("X : ", GUILayout.ExpandWidth(false));
			if (GUILayout.Button("-", GUILayout.Width(20))) UpdateShapeSize(new Vector2Int(-1, 0));
			GUILayout.Label(" " + _shapeSize.x + " ", GUILayout.ExpandWidth(false));
			if (GUILayout.Button("+", GUILayout.Width(20))) UpdateShapeSize(new Vector2Int(1, 0));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Y : ", GUILayout.ExpandWidth(false));
			if (GUILayout.Button("-", GUILayout.Width(20))) UpdateShapeSize(new Vector2Int(0, -1));
			GUILayout.Label(" " + _shapeSize.y + " ", GUILayout.ExpandWidth(false));
			if (GUILayout.Button("+", GUILayout.Width(20))) UpdateShapeSize(new Vector2Int(0, 1));
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();
		}
		public override bool RequiresConstantRepaint()
		{
			return true;
		}
		private void CreateInitialShape()
		{
			_plantTarget.ShapeSize = new Vector2Int(1, 1);
			_plantTarget.Shape = new bool[1];
			_plantTarget.Shape[0] = false;
			EditorUtility.SetDirty(_plantTarget);
		}
		private void UpdateShapeSize(Vector2Int offsetSize)
		{
			var updatedSize = _plantTarget.ShapeSize + offsetSize;
			if (updatedSize.x < 1 || updatedSize.y < 1)
			{
				return; //Invalid size
			}
			var newShapeBuffer = new bool[updatedSize.x * updatedSize.y];
			for (var i = 0; i < _plantTarget.ShapeSize.x; i++)
			{
				for (var j = 0; j < _plantTarget.ShapeSize.y; j++)
				{
					if (i >= updatedSize.x || j >= updatedSize.y)
					{
						continue;
					}
					var shapeIndex = i + j * _plantTarget.ShapeSize.x;
					var newShapeIndex = i + j * updatedSize.x;
					newShapeBuffer[newShapeIndex] = _plantTarget.Shape[shapeIndex];
				}
			}
			_plantTarget.ShapeSize = updatedSize;
			_plantTarget.Shape = newShapeBuffer;
			EditorUtility.SetDirty(_plantTarget);
		}

		private void CreateStyles()
		{
			if (_emptyStyle == null)
			{
				var texture = new Texture2D(1, 1);
				texture.SetPixel(0, 0, Color.grey);
				texture.wrapMode = TextureWrapMode.Repeat;
				texture.Apply();
				_emptyStyle = new GUIStyle();
				_emptyStyle.normal.background = texture;
			}
			if (_filledStyle == null)
			{
				var texture = new Texture2D(1, 1);
				texture.SetPixel(0, 0, new Color(0.5f, 0.8f, 0.5f, 1.0f));
				texture.wrapMode = TextureWrapMode.Repeat;
				texture.Apply();
				_filledStyle = new GUIStyle();
				_filledStyle.normal.background = texture;
			}
			if (_hoverStyle == null)
			{
				var texture = new Texture2D(1, 1);
				var color = Color.black;
				color.a = 0.4f;
				texture.SetPixel(0, 0, color);
				texture.wrapMode = TextureWrapMode.Repeat;
				texture.Apply();
				_hoverStyle = new GUIStyle();
				_hoverStyle.normal.background = texture;
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var plant = target as APlantType;
			if (plant == null) return;
			_plantTarget = plant;
			if (_plantTarget.Shape == null || _plantTarget.ShapeSize.x < 1 || _plantTarget.ShapeSize.y < 1 || _plantTarget.Shape.Length != _plantTarget.ShapeSize.x * _plantTarget.ShapeSize.y)
			{
				CreateInitialShape();
			}
			
			CreateStyles();

			//Unity GUI inspector on mouse move handler

			const float buttonSize = 30.0f;
			const float buttonSpacing = 6.0f;
			const float hoverPadding = 4.0f;

			/*if (Event.current.type == EventType.MouseMove)
			{
				Repaint();
			}*/

			GUILayout.Space(10);
			ShapeSizeUI();
			//Get the width of the current window
			var width = EditorGUIUtility.currentViewWidth;
			GUILayout.Space(10);
			var rect = GUILayoutUtility.GetRect(width - 10, 100);
			//get cursor position
			var cursorPos = Event.current.mousePosition;

			//GUI.Box(new Rect(cursorPos.x, cursorPos.y, 10.0f, 10.0f), "", _emptyStyle);

			for (var y = 0; y < _plantTarget.ShapeSize.y; y++)
			{
				for (var x = 0; x < _plantTarget.ShapeSize.x; x++)
				{
					var buttonRect = new Rect(rect.x + x * (buttonSize + buttonSpacing), rect.y + y * (buttonSize + buttonSpacing), buttonSize, buttonSize);
					var index = x + y * _plantTarget.ShapeSize.x;

					if (Event.current.type == EventType.Repaint)
					{
						if (_plantTarget.Shape[index])
						{
							GUI.Box(buttonRect, "", _filledStyle);
						}
						else
						{
							GUI.Box(buttonRect, "", _emptyStyle);
						}
						if (buttonRect.Contains(cursorPos))
						{
							var hoverRect = buttonRect;
							hoverRect.x += hoverPadding;
							hoverRect.y += hoverPadding;
							hoverRect.width -= hoverPadding * 2.0f;
							hoverRect.height -= hoverPadding * 2.0f;
							GUI.Box(hoverRect, "", _hoverStyle);
						}
					}

					if (Event.current.type == EventType.MouseDown && buttonRect.Contains(cursorPos))
					{
						_plantTarget.Shape[index] = !_plantTarget.Shape[index];
						EditorUtility.SetDirty(_plantTarget);
					}
				}
			}
			//GUI.Button(new Rect(0, 0, 100, 100), "Hello");
		}
	}
}