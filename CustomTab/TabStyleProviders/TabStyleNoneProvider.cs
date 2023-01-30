/*
 * This code is provided under the Code Project Open Licence (CPOL)
 * See http://www.codeproject.com/info/cpol10.aspx for details
 */

namespace System.Windows.Forms
{
	using System;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

    [System.ComponentModel.ToolboxItem(false)]
    public class TabStyleNoneProvider : TabStyleProvider {
        public TabStyleNoneProvider(CustomTabControl tabControl) : base(tabControl) {
        }

        public override void AddTabBorder(System.Drawing.Drawing2D.GraphicsPath path, System.Drawing.Rectangle tabBounds) {
        }
    }

    [System.ComponentModel.ToolboxItem(false)]
    public class TabStyleFlatProvider : TabStyleProvider {//, IEventHandler {
        private Color TabBackColor { get; set; } = SystemColors.Control;
        private Color TabBackColorHot { get; set; } = SystemColors.Highlight;
        private Color TabBackColorActive { get; set; } = SystemColors.Highlight;
        private Color TabBackColorDisabled { get; set; } = SystemColors.Control;
        private Color TabActiveSeparator { get; set; } = SystemColors.Window;

        public TabStyleFlatProvider(CustomTabControl tabControl) : base(tabControl) {
            this._Radius = 1;
            this._Overlap = 1;
            this.HotTrack = true;
            this.FocusTrack = true;
            this.Padding = new Point(10, 3); //	Must set after the _Radius
            //EventManager.AddEventHandler(this, EventType.ApplyTheme);
            this.RefreshColors();
        }

        //private Color GetThemeColor(String id) {
        //    return PluginBase.MainForm.GetThemeColor(id);          
        //}

        //public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority) {
        //    if(e.Type == EventType.ApplyTheme)
        //        RefreshColors();
        //}

        public void RefreshColors() {
            //this._BorderColor = GetThemeColor("TabControl.BorderColor");
            //this._BorderColorHot = GetThemeColor("TabControl.BorderColorHot");
            //this._BorderColorSelected = GetThemeColor("TabControl.BorderColorSelected");
            //this._CloserColor = GetThemeColor("TabControl.CloserColor");
            //this._CloserColorActive = GetThemeColor("TabControl.CloserColorActive");
            //this._FocusColor = GetThemeColor("TabControl.FocusColor");
            //this._TextColor = GetThemeColor("TabControl.TextColor");
            //this._TextColorDisabled = GetThemeColor("TabControl.TextColorDisabled");
            //this._TextColorSelected = GetThemeColor("TabControl.TextColorSelected");
            //this.TabActiveSeparator = GetThemeColor("TabControl.TabActiveSeparator");
            //this.TabBackColorActive = GetThemeColor("TabControl.TabBackColorActive");
            //this.TabBackColorDisabled = GetThemeColor("TabControl.TabBackColorDisabled");
            //this.TabBackColorHot = GetThemeColor("TabControl.TabBackColorHot");
            //this.TabBackColor = GetThemeColor("TabControl.TabBackColor");
            this._CloserColorActive = Color.Black;
            this._CloserColor = Color.FromArgb(117, 99, 61);
            this._TextColor = Color.White;
            this._TextColorDisabled = Color.WhiteSmoke;
            this._BorderColor = Color.Transparent;
            this._BorderColorHot = Color.FromArgb(155, 167, 183);
        }

        public override Brush GetPageBackgroundBrush(int index) {
            return new SolidBrush(this.TabActiveSeparator);
        }

        protected override Brush GetTabBackgroundBrush(int index) {
            SolidBrush brush = new SolidBrush(this.TabBackColor);
            if(this._TabControl.SelectedIndex == index) {
                brush = new SolidBrush(this.TabBackColorActive);
            }
            else if(!this._TabControl.TabPages[index].Enabled) {
                brush = new SolidBrush(this.TabBackColorDisabled);
            }
            else if(this._HotTrack && index == this._TabControl.ActiveIndex) {
                brush = new SolidBrush(this.TabBackColorHot);
            }
            return brush;
        }

        protected override void DrawTabFocusIndicator(GraphicsPath tabpath, int index, Graphics graphics) {
            if(this._FocusTrack && this._TabControl.Focused && index == this._TabControl.SelectedIndex) {
                Int32 width = 3;
                Rectangle focusRect = Rectangle.Empty;
                RectangleF pathRect = tabpath.GetBounds();
                Brush focusBrush = new SolidBrush(this._FocusColor);
                switch(this._TabControl.Alignment) {
                    case TabAlignment.Top:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, (int)pathRect.Width, width);
                        break;
                    case TabAlignment.Bottom:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Bottom - width, (int)pathRect.Width, width);
                        break;
                    case TabAlignment.Left:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, width, (int)pathRect.Height);
                        break;
                    case TabAlignment.Right:
                        focusRect = new Rectangle((int)pathRect.Right - width, (int)pathRect.Y, width, (int)pathRect.Height);
                        break;
                }
                //	Ensure the focus stip does not go outside the tab
                Region focusRegion = new Region(focusRect);
                focusRegion.Intersect(tabpath);
                graphics.FillRegion(focusBrush, focusRegion);
                focusRegion.Dispose();
                focusBrush.Dispose();
            }
        }

        public override void AddTabBorder(System.Drawing.Drawing2D.GraphicsPath path, System.Drawing.Rectangle tabBounds) {
            switch(this._TabControl.Alignment) {
                case TabAlignment.Top:
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    break;
                case TabAlignment.Bottom:
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    break;
                case TabAlignment.Left:
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    break;
                case TabAlignment.Right:
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    break;
            }
        }
    }

    public class TabStyleDarkProvider : TabStyleProvider {
        public TabStyleDarkProvider(CustomTabControl tabControl) : base(tabControl) {
            _BorderColor = Color.FromArgb(96, 96, 96);
            _BorderColorHot = Color.FromArgb(96, 96, 96);
            _BorderColorSelected = Color.FromArgb(96, 96, 96);
            _TextColor = Color.FromArgb(153, 153, 153);
            _TextColorSelected = Color.FromArgb(152, 196, 232);
            _TextColorDisabled = Color.FromArgb(96, 96, 96);
            _CloserColor = Color.White;
            _CloserColorActive = Color.FromArgb(152, 196, 232);

            _HotTrack = false;
            _FocusTrack = false;

            _Radius = 10;
            _Overlap = 0;
            _Opacity = 1F;

            _Padding = new Point(6, 3);
        }

        protected override Brush GetTabBackgroundBrush(int index) {
            if(_TabControl.SelectedIndex == index)
                return new LinearGradientBrush(GetTabRect(index), Color.FromArgb(60, 63, 65), Color.FromArgb(60, 63, 65), LinearGradientMode.Vertical);
            else
                return new LinearGradientBrush(GetTabRect(index), Color.FromArgb(48, 48, 48), Color.FromArgb(48, 48, 48), LinearGradientMode.Vertical);
        }

        public override void AddTabBorder(GraphicsPath path, Rectangle tabBounds) {
            switch(_TabControl.Alignment) {
                case TabAlignment.Top:
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    break;

                case TabAlignment.Bottom:
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    break;

                case TabAlignment.Left:
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    break;

                case TabAlignment.Right:
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    break;
            }
        }

        public override Rectangle GetTabRect(int index) {
            if(index < 0)
                return new Rectangle();

            Rectangle tabBounds = base.GetTabRect(index);

            switch(_TabControl.Alignment) {
                case TabAlignment.Top:
                    tabBounds.Height += 1;
                    break;

                case TabAlignment.Bottom:
                    tabBounds = new Rectangle(tabBounds.Location.X, tabBounds.Location.Y - 1, tabBounds.Width, tabBounds.Height + 1);
                    break;
            }

            EnsureFirstTabIsInView(ref tabBounds, index);

            return tabBounds;
        }
    }

}
