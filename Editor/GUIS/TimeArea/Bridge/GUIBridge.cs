using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SimpleTimeArea.Editor
{
    public static class GUIBridge
    {
        public static Color AlphaMultiplied(this Color c, float multiplier)
        {
            return new Color(c.r, c.g, c.b, c.a * multiplier);
        }


        private static MethodInfo _scrollerMethodCache;

        public static float Scroller(Rect position,
            float value,
            float size,
            float leftValue,
            float rightValue,
            GUIStyle slider,
            GUIStyle thumb,
            GUIStyle leftButton,
            GUIStyle rightButton,
            bool horiz)
        {
            if (_scrollerMethodCache is null)
            {
                var guiType = typeof(UnityEngine.GUI);
                _scrollerMethodCache = guiType.GetMethod("Scroller", BindingFlags.NonPublic | BindingFlags.Static);
            }

            if (_scrollerMethodCache != null)
            {
                // 使用反射调用方法
                return (float)_scrollerMethodCache.Invoke(null, new object[] { value, size, leftValue, rightValue, slider, thumb, leftButton, rightButton, horiz });
            }
            else
            {
                Debug.LogError("Scroller method not found.");
                return -1;
            }
        }

        public static void DrawVerticalLine(float x, float minY, float maxY, Color color)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            Color backupCol = Handles.color;

            HandleUtility.ApplyWireMaterial();
            if (Application.platform == RuntimePlatform.WindowsEditor)
                GL.Begin(GL.QUADS);
            else
                GL.Begin(GL.LINES);
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                GL.Color(color);
                GL.Vertex(new Vector3(x - 0.5f, minY, 0));
                GL.Vertex(new Vector3(x + 0.5f, minY, 0));
                GL.Vertex(new Vector3(x + 0.5f, maxY, 0));
                GL.Vertex(new Vector3(x - 0.5f, maxY, 0));
            }
            else
            {
                GL.Color(color);
                GL.Vertex(new Vector3(x, minY, 0));
                GL.Vertex(new Vector3(x, maxY, 0));
            }

            GL.End();

            Handles.color = backupCol;
        }
        
    }
}