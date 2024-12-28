using NBC.ActionEditor.Events;
using SimpleTimeArea.Editor;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class MyTimelinePointerView : ViewBase
    {
        public Asset asset => App.AssetData;
        private bool _isDraggingPlayHead;
        private Rect _playHeadRect;
        private Rect _timeRulerRect;
        private float _pointerTime;
        private int _timeHeaderControlId;

        protected override void OnInit()
        {
            base.OnInit();
            _timeHeaderControlId = GUIUtility.GetControlID(FocusType.Passive);
        }

        public override void OnDraw()
        {
            if (asset == null) return;
            _playHeadRect = new Rect(0, 0, Position.width, Position.height);
            DrawTimeStep();
            DrawPointer();
            HandleDragPointer();
        }

        private void DrawTimeStep()
        {
            var originColor = GUI.color;
            GUI.color = Color.white;
            _timeRulerRect = new Rect(Position.x, Position.y, Position.width, Styles.HeaderHeight);
            App.TimeArea.TimeRuler(_timeRulerRect, 60);
            App.AssetData.ViewTimeMin = App.TimeArea.PixelToTime(_timeRulerRect.xMin, _timeRulerRect);
            App.AssetData.ViewTimeMax = App.TimeArea.PixelToTime(_timeRulerRect.xMax, _timeRulerRect);
            GUI.color = originColor;
        }

        private void DrawPointer()
        {
            var x = App.TimeArea.TimeToPixel(_pointerTime, _timeRulerRect);
            var height = Styles.PlayControlHeight - Styles.HeaderHeight;
            TimeArea.DrawPlayhead(x, 0, Position.height, 1, 1f);

            var playPointerHandler = new Rect(x - 5.5f, Position.y, 11, height);

            GUI.DrawTexture(playPointerHandler, Styles.TimelineTimeCursorIcon);
        }

        private void HandleDragPointer()
        {
            var ev = Event.current;

            if (ev.type == EventType.MouseDown && _timeRulerRect.Contains(ev.mousePosition))
            {
                GUIUtility.hotControl = _timeHeaderControlId;
                _isDraggingPlayHead = true;
                _pointerTime = App.TimeArea.PixelToTime(ev.mousePosition.x, _timeRulerRect);
            }

            //这里的MouseUp不知道被哪里Use了(
            if (GUIUtility.hotControl == _timeHeaderControlId && (ev.rawType == EventType.MouseUp || ev.rawType == EventType.Used))
            {
                GUIUtility.hotControl = 0;
                _isDraggingPlayHead = false;
            }

            if (_isDraggingPlayHead)
            {
                var time = asset.PosToTime(ev.mousePosition.x, App.Width);
                _pointerTime = App.TimeArea.PixelToTime(ev.mousePosition.x, _timeRulerRect);
                _pointerTime = _pointerTime < 0 ? 0 : _pointerTime;
                _pointerTime = asset.SnapTime(_pointerTime);
                AssetPlayer.Inst.CurrentTime = Mathf.Clamp(_pointerTime, 0 + float.Epsilon, asset.Length - float.Epsilon);
            }
        }
    }
}