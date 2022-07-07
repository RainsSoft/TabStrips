using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FarsiLibrary.Win {
    internal class FATabStripMenuGlyph {
        #region Fields

        private Rectangle glyphRect = Rectangle.Empty;
        private bool isMouseOver = false;
        private ToolStripProfessionalRenderer renderer;

        #endregion

        #region Props

        public bool IsMouseOver {
            get {
                return isMouseOver;
            }
            internal set {
                isMouseOver = value;
            }
        }
        public bool IsVisible {
            get;
            internal set;
        }
        public Rectangle Bounds {
            get {
                return glyphRect;
            }
            set {
                glyphRect = value;
            }
        }

        #endregion

        #region Ctor

        internal FATabStripMenuGlyph(ToolStripProfessionalRenderer renderer) {
            this.renderer = renderer;
        }

        #endregion

        #region Methods

        public void DrawGlyph(Graphics g) {
            if(isMouseOver) {
                Color fill = renderer.ColorTable.ButtonSelectedHighlight; //Color.FromArgb(35, SystemColors.Highlight);
                using(SolidBrush sbfill = new SolidBrush(fill)) {
                    g.FillRectangle(sbfill, glyphRect);
                }
                Rectangle borderRect = glyphRect;

                borderRect.Width--;
                borderRect.Height--;

                g.DrawRectangle(SystemPens.Highlight, borderRect);
            }
            else {
                //绘制背景
                Color fill = _CONST.TAB_ACTIVE_TOOLBAR_BACKGROUND;//renderer.ColorTable.OverflowButtonGradientMiddle; //Color.FromArgb(35, SystemColors.Highlight);
                using(SolidBrush sbfill = new SolidBrush(fill)) {
                    g.FillRectangle(sbfill, glyphRect);
                }
                Rectangle borderRect = glyphRect;

                borderRect.Width--;
                borderRect.Height--;

                g.DrawRectangle(SystemPens.Highlight, borderRect);
            }
            SmoothingMode bak = g.SmoothingMode;

            g.SmoothingMode = SmoothingMode.Default;

            using(Pen pen = new Pen(Color.Black)) {
                pen.Width = 2;

                g.DrawLine(pen, new Point(glyphRect.Left + (glyphRect.Width / 3) - 2, glyphRect.Height / 2 - 1),
                    new Point(glyphRect.Right - (glyphRect.Width / 3), glyphRect.Height / 2 - 1));
            }

            g.FillPolygon(Brushes.Black, new Point[]{
                new Point(glyphRect.Left + (glyphRect.Width / 3)-2, glyphRect.Height / 2+2),
                new Point(glyphRect.Right - (glyphRect.Width / 3), glyphRect.Height / 2+2),
                new Point(glyphRect.Left + glyphRect.Width / 2-1,glyphRect.Bottom-4)});

            g.SmoothingMode = bak;
        }

        #endregion
    }
}
