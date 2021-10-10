using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MessengerBotManager
{
    public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {
        private TextEditor _editor;
        private Color _highlightColor;

        public HighlightCurrentLineBackgroundRenderer(TextEditor editor, Color highlightColor)
        {
            _editor = editor;
            _highlightColor = highlightColor;
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Caret; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null)
                return;

            textView.EnsureVisualLines();
            var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);
            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {
                drawingContext.DrawRectangle(
                    new SolidColorBrush(_highlightColor), null,
                    new Rect(new Point(rect.Location.X + textView.ScrollOffset.X, rect.Location.Y), new Size(textView.ActualWidth, rect.Height)));
            }
        }
    }
}
