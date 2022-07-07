using System.Drawing;
using System.Windows.Forms;

namespace FarsiLibrary.Win {
    internal class FATabStripCloseButton {
        #region Fields

        private Rectangle crossRect = Rectangle.Empty;
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
                return crossRect;
            }
            set {
                crossRect = value;
            }
        }

        #endregion

        #region Ctor

        internal FATabStripCloseButton(ToolStripProfessionalRenderer renderer) {
            this.renderer = renderer;
        }

        #endregion

        #region Methods

        public void DrawCross(Graphics g) {
            if(isMouseOver) {
                Color fill = renderer.ColorTable.ButtonSelectedHighlight;
                using(SolidBrush sbfill = new SolidBrush(fill)) {
                    g.FillRectangle(sbfill, crossRect);
                }
                Rectangle borderRect = crossRect;

                borderRect.Width--;
                borderRect.Height--;

                g.DrawRectangle(SystemPens.Highlight, borderRect);
            }
            else {
                //绘制背景
                Color fill = _CONST.TAB_ACTIVE_TOOLBAR_BACKGROUND; //renderer.ColorTable.OverflowButtonGradientMiddle;
                using(SolidBrush sbfill = new SolidBrush(fill)) {
                    g.FillRectangle(sbfill, crossRect);
                }

                Rectangle borderRect = crossRect;

                borderRect.Width--;
                borderRect.Height--;

                g.DrawRectangle(SystemPens.Highlight, borderRect);
            }

            using(Pen pen = new Pen(Color.Black, 1.6f)) {
                g.DrawLine(pen, crossRect.Left + 3, crossRect.Top + 3,
                    crossRect.Right - 5, crossRect.Bottom - 4);

                g.DrawLine(pen, crossRect.Right - 5, crossRect.Top + 3,
                    crossRect.Left + 3, crossRect.Bottom - 4);
            }
        }

        #endregion
    }
}
