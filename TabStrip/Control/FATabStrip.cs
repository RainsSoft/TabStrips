using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FarsiLibrary.Win.BaseClasses;
using FarsiLibrary.Win.Design;

namespace FarsiLibrary.Win {
    
    [Designer(typeof(FATabStripDesigner))]
    [DefaultEvent("TabStripItemSelectionChanged")]
    [DefaultProperty("Items")]
    [ToolboxItem(true)]
    [ToolboxBitmap("FATabStrip.bmp")]
    public class FATabStrip : BaseStyledPanel, ISupportInitialize, IDisposable {
        #region Static Fields

        internal static int PreferredWidth = 350;
        internal static int PreferredHeight = 200;

        #endregion

        #region Constants

        private const int DEF_HEADER_HEIGHT = 24;//19;
        //private const int DEF_GLYPH_WIDTH = 40;   
        private int DEF_START_POS = 10; 
      
        private const int DEF_GLYPH_WIDTH = DEF_HEADER_HEIGHT*2;        
        private const int DEF_TOOLBAR_BUTTON_HEIGHT =  DEF_HEADER_HEIGHT- 3;
        private const int DEF_TAB_ALLOW_DRAW_BOUNDARY = 20;
        private const int DEF_TAB_TEXT_WIDTH_EXTEND = 30;//20;
        #endregion

        #region Events

        public event TabStripItemClosingHandler TabStripItemClosing;
        public event TabStripItemChangedHandler TabStripItemSelectionChanged;
        public event HandledEventHandler MenuItemsLoading;
        public event EventHandler MenuItemsLoaded;
        public event EventHandler TabStripItemClosed;

        #endregion

        #region Fields

        private Rectangle stripButtonRect = Rectangle.Empty;
        private FATabStripItem selectedItem = null;
        private ContextMenuStrip menu = null;
        private FATabStripMenuGlyph menuGlyph = null;
        private FATabStripCloseButton closeButton = null;
        private FATabStripItemCollection items;
        private FATabStripItemCollection itemsDraw;
        private StringFormat sf = null;
        private static Font defaultFont = new Font("Tahoma", 8.25f, FontStyle.Regular);

        private bool alwaysShowClose = true;
        private bool isIniting = false;
        private bool alwaysShowMenuGlyph = true;
        private bool menuOpen = false;
        private bool alwayDrawBorderLine = true;
        private bool alwayDrawToolBarRegion = false;
        //private bool alwayShowItemCircle = true;
        //private int showItemStartIndex = 0;
        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Returns hit test results
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public HitTestResult HitTest(Point pt) {
            if(closeButton.IsVisible&&closeButton.Bounds.Contains(pt))
                return HitTestResult.CloseButton;

            if(menuGlyph.IsVisible&&menuGlyph.Bounds.Contains(pt))
                return HitTestResult.MenuGlyph;

            if(GetTabItemByPoint(pt) != null)
                return HitTestResult.TabItem;

            //No other result is available.
            return HitTestResult.None;
        }

        /// <summary>
        /// Add a <see cref="FATabStripItem"/> to this control without selecting it.
        /// </summary>
        /// <param name="tabItem"></param>
        public void AddTab(FATabStripItem tabItem) {
            AddTab(tabItem, false);
        }

        /// <summary>
        /// Add a <see cref="FATabStripItem"/> to this control.
        /// User can make the currently selected item or not.
        /// </summary>
        /// <param name="tabItem"></param>
        public void AddTab(FATabStripItem tabItem, bool autoSelect) {
            tabItem.Dock = DockStyle.Fill;
            Items.Add(tabItem);

            if((autoSelect && tabItem.Visible) || (tabItem.Visible && Items.DrawnCount < 1)) {
                SelectedItem = tabItem;
                SelectItem(tabItem);
            }
        }

        /// <summary>
        /// Remove a <see cref="FATabStripItem"/> from this control.
        /// </summary>
        /// <param name="tabItem"></param>
        public void RemoveTab(FATabStripItem tabItem) {
            int tabIndex = Items.IndexOf(tabItem);

            if(tabIndex >= 0) {
                UnSelectItem(tabItem);
                Items.Remove(tabItem);
            }

            if(Items.Count > 0) {
                if(RightToLeft == RightToLeft.No) {
                    if(Items[tabIndex - 1] != null) {
                        SelectedItem = Items[tabIndex - 1];
                    }
                    else {
                        SelectedItem = Items.FirstVisible;
                    }
                }
                else {
                    if(Items[tabIndex + 1] != null) {
                        SelectedItem = Items[tabIndex + 1];
                    }
                    else {
                        SelectedItem = Items.LastVisible;
                    }
                }
            }
        }

        /// <summary>
        /// Get a <see cref="FATabStripItem"/> at provided point.
        /// If no item was found, returns null value.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public FATabStripItem GetTabItemByPoint(Point pt) {
            FATabStripItem item = null;
            bool found = false;

            for(int i = 0; i < Items.Count; i++) {
                FATabStripItem current = Items[i];

                if(current.StripRect.Contains(pt) && current.Visible && current.IsDrawn) {
                    item = current;
                    found = true;
                }

                if(found)
                    break;
            }

            return item;
        }

        /// <summary>
        /// Display items menu
        /// </summary>
        public virtual void ShowMenu() {
            if(menu.Visible == false && menu.Items.Count > 0) {
                if(RightToLeft == RightToLeft.No) {
                    menu.Show(this, new Point(menuGlyph.Bounds.Left, menuGlyph.Bounds.Bottom));
                }
                else {
                    menu.Show(this, new Point(menuGlyph.Bounds.Right, menuGlyph.Bounds.Bottom));
                }

                menuOpen = true;
            }
        }

        #endregion

        #region Internal

        internal void UnDrawAll() {
            for(int i = 0; i < Items.Count; i++) {
                Items[i].IsDrawn = false;
            }
        }

        internal void SelectItem(FATabStripItem tabItem) {
            tabItem.Dock = DockStyle.Fill;
            tabItem.Visible = true;
            tabItem.Selected = true;
        }

        internal void UnSelectItem(FATabStripItem tabItem) {
            //tabItem.Visible = false;
            tabItem.Selected = false;
        }

        #endregion

        #region Protected

        /// <summary>
        /// Fires <see cref="TabStripItemClosing"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnTabStripItemClosing(TabStripItemClosingEventArgs e) {
            if(TabStripItemClosing != null)
                TabStripItemClosing(e);
        }

        /// <summary>
        /// Fires <see cref="TabStripItemClosed"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnTabStripItemClosed(EventArgs e) {
            if(TabStripItemClosed != null)
                TabStripItemClosed(this, e);
        }

        /// <summary>
        /// Fires <see cref="MenuItemsLoading"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMenuItemsLoading(HandledEventArgs e) {
            if(MenuItemsLoading != null)
                MenuItemsLoading(this, e);
        }
        /// <summary>
        /// Fires <see cref="MenuItemsLoaded"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMenuItemsLoaded(EventArgs e) {
            if(MenuItemsLoaded != null)
                MenuItemsLoaded(this, e);
        }

        /// <summary>
        /// Fires <see cref="TabStripItemSelectionChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTabStripItemChanged(TabStripItemChangedEventArgs e) {
            if(TabStripItemSelectionChanged != null)
                TabStripItemSelectionChanged(e);
        }

        /// <summary>
        /// Loads menu items based on <see cref="FATabStripItem"/>s currently added
        /// to this control.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMenuItemsLoad(EventArgs e) {
            menu.RightToLeft = RightToLeft;
            menu.Items.Clear();

            for(int i = 0; i < Items.Count; i++) {
                FATabStripItem item = Items[i];
                if(!item.Visible)
                    continue;

                ToolStripMenuItem tItem = new ToolStripMenuItem(item.Title);
                tItem.Tag = item;
                tItem.Image = item.Image;
                menu.Items.Add(tItem);
            }

            OnMenuItemsLoaded(EventArgs.Empty);
        }

        #endregion

        #region Overrides

        protected override void OnRightToLeftChanged(EventArgs e) {
            base.OnRightToLeftChanged(e);
            UpdateLayout();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e) {
            SetDefaultSelected();
            Rectangle borderRc = ClientRectangle;
            borderRc.Width--;
            borderRc.Height--;

            if(RightToLeft == RightToLeft.No) {
                DEF_START_POS = 10;
            }
            else {
                DEF_START_POS = stripButtonRect.Right;
            }
            //绘制toolBar背景
            if(alwayDrawToolBarRegion) {
                Rectangle borderToolBarRc = new Rectangle(1,1,borderRc.Width-2, borderRc.Height - 2);
                //using(Pen penBorderRc = new Pen(_CONST.TAB_ACTIVE_TOOLBAR_BACKGROUND)) {
                //绘制顶部背景色
                using(Brush bh = new SolidBrush(_CONST.TAB_ACTIVE_TOOLBAR_BACKGROUND)) {
                   e.Graphics.FillRectangle(bh, borderToolBarRc);
                }
                //}
            }
            // draw border: left, top, right, bottom of tab control
            //e.Graphics.DrawRectangle(SystemPens.ControlDark, borderRc);     
            if(alwayDrawBorderLine) {
                using(Pen penBorderRc = new Pen(_CONST.TAB_ACTIVE_BORDER_COLOR)) {
                    //绘制tab边缘线
                    e.Graphics.DrawRectangle(penBorderRc, borderRc);
                }
            }
            
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            #region Draw Pages

            for(int i = 0; i < Items.Count; i++) {
                FATabStripItem currentItem = Items[i];
                if(!currentItem.Visible && !DesignMode)
                    continue;

                OnCalcTabPage(e.Graphics, currentItem);
                currentItem.IsDrawn = false;

                if(!AllowDraw(currentItem))
                    continue;

                OnDrawTabPage(e.Graphics, currentItem);
            }

            #endregion

            #region Draw UnderPage Line

            if(RightToLeft == RightToLeft.No) {
                if(Items.DrawnCount == 0 || Items.VisibleCount == 0) {
                    e.Graphics.DrawLine(SystemPens.ControlDark, new Point(0, DEF_HEADER_HEIGHT), new Point(ClientRectangle.Width, DEF_HEADER_HEIGHT));
                }
                else if(SelectedItem != null && SelectedItem.IsDrawn) {
                    Point end = new Point((int)SelectedItem.StripRect.Left - 9, DEF_HEADER_HEIGHT);
                    e.Graphics.DrawLine(SystemPens.ControlDark, new Point(0, DEF_HEADER_HEIGHT), end);
                    end.X += (int)SelectedItem.StripRect.Width + 10;
                    //??????????????????????????????????????????????
                    ///e.Graphics.DrawLine(SystemPens.ControlDark, end, new Point(ClientRectangle.Width, DEF_HEADER_HEIGHT));
                    e.Graphics.DrawLine(SystemPens.ControlDark, new Point(end.X + 3, end.Y), new Point(ClientRectangle.Width - 3, DEF_HEADER_HEIGHT));
                }
            }
            else {
                if(Items.DrawnCount == 0 || Items.VisibleCount == 0) {
                    e.Graphics.DrawLine(SystemPens.ControlDark, new Point(0, DEF_HEADER_HEIGHT),
                                        new Point(ClientRectangle.Width, DEF_HEADER_HEIGHT));
                }
                else if(SelectedItem != null && SelectedItem.IsDrawn) {
                    Point end = new Point((int)SelectedItem.StripRect.Left, DEF_HEADER_HEIGHT);
                    e.Graphics.DrawLine(SystemPens.ControlDark, new Point(0, DEF_HEADER_HEIGHT), end);
                    end.X += (int)SelectedItem.StripRect.Width + DEF_TAB_TEXT_WIDTH_EXTEND;// 20;
                    e.Graphics.DrawLine(SystemPens.ControlDark, end, new Point(ClientRectangle.Width, DEF_HEADER_HEIGHT));
                }
            }

            #endregion

            #region Draw Menu and Close Glyphs

            if(AlwaysShowMenuGlyph || Items.DrawnCount < Items.VisibleCount) {
                menuGlyph.IsVisible = true;
                //menuGlyph.DrawGlyph(e.Graphics);

            }
            else{
                menuGlyph.IsVisible = false;
            }

            if(AlwaysShowClose || (SelectedItem != null && SelectedItem.CanClose)) {
                closeButton.IsVisible = true;
                //closeButton.DrawCross(e.Graphics);
            }
            else {
                closeButton.IsVisible = false;
            }
            caculCloseAndMenuRect();
            if(menuGlyph.IsVisible) {
                menuGlyph.DrawGlyph(e.Graphics);
            }
            if(closeButton.IsVisible) {
                closeButton.DrawCross(e.Graphics);
            }
            #endregion
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if(e.Button != MouseButtons.Left)
                return;

            HitTestResult result = HitTest(e.Location);
            if(result == HitTestResult.MenuGlyph) {
                HandledEventArgs args = new HandledEventArgs(false);
                OnMenuItemsLoading(args);

                if(!args.Handled)
                    OnMenuItemsLoad(EventArgs.Empty);

                ShowMenu();
            }
            else if(result == HitTestResult.CloseButton) {
                if(SelectedItem != null) {
                    TabStripItemClosingEventArgs args = new TabStripItemClosingEventArgs(SelectedItem);
                    OnTabStripItemClosing(args);
                    if(!args.Cancel && SelectedItem.CanClose) {
                        RemoveTab(SelectedItem);
                        OnTabStripItemClosed(EventArgs.Empty);
                    }
                }
            }
            else if(result == HitTestResult.TabItem) {
                FATabStripItem item = GetTabItemByPoint(e.Location);
                if(item != null)
                    SelectedItem = item;
            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if(menuGlyph.Bounds.Contains(e.Location)) {
                menuGlyph.IsMouseOver = true;
                Invalidate(menuGlyph.Bounds);
            }
            else {
                if(menuGlyph.IsMouseOver && !menuOpen) {
                    menuGlyph.IsMouseOver = false;
                    Invalidate(menuGlyph.Bounds);
                }
            }

            if(closeButton.Bounds.Contains(e.Location)) {
                closeButton.IsMouseOver = true;
                Invalidate(closeButton.Bounds);
            }
            else {
                if(closeButton.IsMouseOver) {
                    closeButton.IsMouseOver = false;
                    Invalidate(closeButton.Bounds);
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            menuGlyph.IsMouseOver = false;
            Invalidate(menuGlyph.Bounds);

            closeButton.IsMouseOver = false;
            Invalidate(closeButton.Bounds);
        }

        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            if(isIniting)
                return;

            UpdateLayout();
        }

        #endregion

        #region Private

        private bool AllowDraw(FATabStripItem item) {
            bool result = true;
            int boundary = DEF_TAB_ALLOW_DRAW_BOUNDARY;
            if(RightToLeft == RightToLeft.No) {
                //if(item.StripRect.Right >= stripButtonRect.Width)
                //    result = false;
                if(item.StripRect.Left+boundary >= stripButtonRect.Width)
                    result = false;
            }
            else {
                //if(item.StripRect.Left <= stripButtonRect.Left)
                //    return false;
                if(item.StripRect.Right-boundary <= stripButtonRect.Left)
                    return false;
            }

            return result;
        }

        private void SetDefaultSelected() {
            if(selectedItem == null && Items.Count > 0)
                SelectedItem = Items[0];

            for(int i = 0; i < Items.Count; i++) {
                FATabStripItem itm = Items[i];
                itm.Dock = DockStyle.Fill;
            }
        }

        private void OnMenuItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            FATabStripItem clickedItem = (FATabStripItem)e.ClickedItem.Tag;
            SelectedItem = clickedItem;
        }

        private void OnMenuVisibleChanged(object sender, EventArgs e) {
            if(menu.Visible == false) {
                menuOpen = false;
            }
        }

        private void OnCalcTabPage(Graphics g, FATabStripItem currentItem) {
            Font currentFont = Font;
            if(currentItem == SelectedItem)
                currentFont = new Font(Font, FontStyle.Bold);

            SizeF textSize = g.MeasureString(currentItem.Title, currentFont, new SizeF(200, 10), sf);
            textSize.Width += DEF_TAB_TEXT_WIDTH_EXTEND;//20;

            if(RightToLeft == RightToLeft.No) {
                int h = DEF_HEADER_HEIGHT - 2;
                RectangleF buttonRect = new RectangleF(DEF_START_POS, 3, textSize.Width, h);
                currentItem.StripRect = buttonRect;
                DEF_START_POS += (int)textSize.Width;
            }
            else {
                int h =  DEF_HEADER_HEIGHT - 2;
                RectangleF buttonRect = new RectangleF(DEF_START_POS - textSize.Width + 1, 3, textSize.Width - 1, h);
                currentItem.StripRect = buttonRect;
                DEF_START_POS -= (int)textSize.Width;
            }
        }

        private void OnDrawTabPage(Graphics g, FATabStripItem currentItem) {
            bool isFirstTab = Items.IndexOf(currentItem) == 0;
            Font currentFont = Font;

            if(currentItem == SelectedItem)
                currentFont = new Font(Font, FontStyle.Bold);

            SizeF textSize = g.MeasureString(currentItem.Title, currentFont, new SizeF(200, 10), sf);
            textSize.Width += DEF_TAB_TEXT_WIDTH_EXTEND;//20;
            RectangleF buttonRect = currentItem.StripRect;

            GraphicsPath path = new GraphicsPath();
            LinearGradientBrush brush;
            int mtop = 3;

            #region Draw Not Right-To-Left Tab

            if(RightToLeft == RightToLeft.No) {
                if(currentItem == SelectedItem || isFirstTab) {
                    //??????????????????????????????????????????????
                    //path.AddLine(buttonRect.Left - 10, buttonRect.Bottom - 1, buttonRect.Left + (buttonRect.Height / 2) - 4, mtop + 4);
                    path.AddLine(buttonRect.Left - 9, buttonRect.Bottom - 1, buttonRect.Left + (buttonRect.Height / 2) - 3, mtop + 4);
                }
                else {
                    path.AddLine(buttonRect.Left, buttonRect.Bottom - 1, buttonRect.Left, buttonRect.Bottom - (buttonRect.Height / 2) - 2);
                    path.AddLine(buttonRect.Left, buttonRect.Bottom - (buttonRect.Height / 2) - 3, buttonRect.Left + (buttonRect.Height / 2) - 4, mtop + 3);
                }

                path.AddLine(buttonRect.Left + (buttonRect.Height / 2) + 2, mtop, buttonRect.Right - 3, mtop);
                path.AddLine(buttonRect.Right, mtop + 2, buttonRect.Right, buttonRect.Bottom - 1);
                path.AddLine(buttonRect.Right - 4, buttonRect.Bottom - 1, buttonRect.Left, buttonRect.Bottom - 1);
                path.CloseFigure();

                if(currentItem == SelectedItem) {
                    //??????????????????????????????????????????????
                    //brush = new LinearGradientBrush(buttonRect, SystemColors.ControlLightLight, SystemColors.Window, LinearGradientMode.Vertical);
                    brush = new LinearGradientBrush(buttonRect, _CONST.TAB_ACTIVE_TITLE_BACKGROUND_1, _CONST.TAB_ACTIVE_TITLE_BACKGROUND_2, LinearGradientMode.Vertical);
                }
                else {
                    //??????????????????????????????????????????????
                    brush = new LinearGradientBrush(buttonRect, SystemColors.ControlLightLight, SystemColors.Control, LinearGradientMode.Vertical);
                    //brush = new LinearGradientBrush(buttonRect, Color.WhiteSmoke, Color.WhiteSmoke, LinearGradientMode.Horizontal);
                }

                g.FillPath(brush, path);
                g.DrawPath(SystemPens.ControlDark, path);

                if(currentItem == SelectedItem) {
                    g.DrawLine(new Pen(brush), buttonRect.Left - 9, buttonRect.Height + 2, buttonRect.Left + buttonRect.Width - 1, buttonRect.Height + 2);
                }

                PointF textLoc = new PointF(buttonRect.Left + buttonRect.Height - 4, buttonRect.Top + (buttonRect.Height / 2) - (textSize.Height / 2) - 3);
                RectangleF textRect = new RectangleF(buttonRect.X,buttonRect.Y,buttonRect.Width,17);//buttonRect;
                textRect.Location = textLoc;
                textRect.Width = buttonRect.Width - (textRect.Left - buttonRect.Left) - 4;
                textRect.Height = textSize.Height + currentFont.Size / 2;

                if(currentItem == SelectedItem) {
                    //textRect.Y -= 2;
                    g.DrawString(currentItem.Title, currentFont, new SolidBrush(ForeColor), textRect, sf);
                }
                else {
                    g.DrawString(currentItem.Title, currentFont, new SolidBrush(ForeColor), textRect, sf);
                }
            }

            #endregion

            #region Draw Right-To-Left Tab

            if(RightToLeft == RightToLeft.Yes) {
                if(currentItem == SelectedItem || isFirstTab) {
                    path.AddLine(buttonRect.Right + 10, buttonRect.Bottom - 1,
                                 buttonRect.Right - (buttonRect.Height / 2) + 4, mtop + 4);
                }
                else {
                    path.AddLine(buttonRect.Right, buttonRect.Bottom - 1, buttonRect.Right,
                                 buttonRect.Bottom - (buttonRect.Height / 2) - 2);
                    path.AddLine(buttonRect.Right, buttonRect.Bottom - (buttonRect.Height / 2) - 3,
                                 buttonRect.Right - (buttonRect.Height / 2) + 4, mtop + 3);
                }

                path.AddLine(buttonRect.Right - (buttonRect.Height / 2) - 2, mtop, buttonRect.Left + 3, mtop);
                path.AddLine(buttonRect.Left, mtop + 2, buttonRect.Left, buttonRect.Bottom - 1);
                path.AddLine(buttonRect.Left + 4, buttonRect.Bottom - 1, buttonRect.Right, buttonRect.Bottom - 1);
                path.CloseFigure();

                if(currentItem == SelectedItem) {
                    brush =
                        new LinearGradientBrush(buttonRect, SystemColors.ControlLightLight, SystemColors.Window,
                                                LinearGradientMode.Vertical);
                }
                else {
                    brush =
                        new LinearGradientBrush(buttonRect, SystemColors.ControlLightLight, SystemColors.Control,
                                                LinearGradientMode.Vertical);
                }

                g.FillPath(brush, path);
                g.DrawPath(SystemPens.ControlDark, path);

                if(currentItem == SelectedItem) {
                    g.DrawLine(new Pen(brush), buttonRect.Right + 9, buttonRect.Height + 2,
                               buttonRect.Right - buttonRect.Width + 1, buttonRect.Height + 2);
                }

                PointF textLoc = new PointF(buttonRect.Left + 2, buttonRect.Top + (buttonRect.Height / 2) - (textSize.Height / 2) - 2);
                RectangleF textRect = new RectangleF(buttonRect.X, buttonRect.Y, buttonRect.Width, 17);//buttonRect
                textRect.Location = textLoc;
                textRect.Width = buttonRect.Width - (textRect.Left - buttonRect.Left) - 10;
                textRect.Height = textSize.Height + currentFont.Size / 2;

                if(currentItem == SelectedItem) {
                    textRect.Y -= 1;
                    g.DrawString(currentItem.Title, currentFont, new SolidBrush(ForeColor), textRect, sf);
                }
                else {
                    g.DrawString(currentItem.Title, currentFont, new SolidBrush(ForeColor), textRect, sf);
                }

                //g.FillRectangle(Brushes.Red, textRect);
            }

            #endregion

            currentItem.IsDrawn = true;
        }
        private void caculCloseAndMenuRect() {
            if(RightToLeft == RightToLeft.No) {               

                int buttonSize = DEF_TOOLBAR_BUTTON_HEIGHT;               
               
                menuGlyph.Bounds = new Rectangle(ClientSize.Width - (buttonSize * 2 + 4 + 1), 2, buttonSize, buttonSize);
                closeButton.Bounds = new Rectangle(ClientSize.Width - (buttonSize + 4), 2, buttonSize, buttonSize);
                if(closeButton.IsVisible == false) {
                    menuGlyph.Bounds = new Rectangle(ClientSize.Width - (buttonSize + 4), 2, buttonSize, buttonSize);
                }
            }
            else {
               
                int buttonSize = DEF_TOOLBAR_BUTTON_HEIGHT;
              
                closeButton.Bounds = new Rectangle(4, 2, buttonSize, buttonSize); //ClientSize.Width - DEF_GLYPH_WIDTH, 2, 16, 16);
                menuGlyph.Bounds = new Rectangle(4 + buttonSize + 1, 2, buttonSize, buttonSize); //this.ClientSize.Width - 20, 2, 16, 16);
                if(closeButton.IsVisible == false) {
                    menuGlyph.Bounds = new Rectangle(4, 2, buttonSize, buttonSize);
                }
            }
        }
        private void UpdateLayout() {
            if(RightToLeft == RightToLeft.No) {
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                sf.FormatFlags &= StringFormatFlags.DirectionRightToLeft;

                //stripButtonRect = new Rectangle(0, 0, ClientSize.Width - DEF_GLYPH_WIDTH - 2, 10);
                //menuGlyph.Bounds = new Rectangle(ClientSize.Width - DEF_GLYPH_WIDTH, 2, 16, 16);
                //closeButton.Bounds = new Rectangle(ClientSize.Width - 20, 2, 16, 16);

                int buttonSize = DEF_TOOLBAR_BUTTON_HEIGHT;
                //int max = Math.Max(buttonSize * 2, DEF_GLYPH_WIDTH);
                stripButtonRect = new Rectangle(0, 0, ClientSize.Width - DEF_GLYPH_WIDTH - 2, 10);
                menuGlyph.Bounds = new Rectangle(ClientSize.Width - (buttonSize*2 + 4+1), 2, buttonSize, buttonSize);
                closeButton.Bounds = new Rectangle(ClientSize.Width - (buttonSize + 4), 2, buttonSize, buttonSize);
                
            }
            else {
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;              
                //stripButtonRect = new Rectangle(DEF_GLYPH_WIDTH + 2, 0, ClientSize.Width - DEF_GLYPH_WIDTH - 15, 10);
                //closeButton.Bounds = new Rectangle(4, 2, 16, 16); //ClientSize.Width - DEF_GLYPH_WIDTH, 2, 16, 16);
                //menuGlyph.Bounds = new Rectangle(20 + 4, 2, 16, 16); //this.ClientSize.Width - 20, 2, 16, 16);

                int buttonSize = DEF_TOOLBAR_BUTTON_HEIGHT;
                //int max = Math.Max(buttonSize * 2, DEF_GLYPH_WIDTH);
                stripButtonRect = new Rectangle(DEF_GLYPH_WIDTH + 2, 0, ClientSize.Width - DEF_GLYPH_WIDTH - 15, 10);
                closeButton.Bounds = new Rectangle(4, 2, buttonSize, buttonSize); //ClientSize.Width - DEF_GLYPH_WIDTH, 2, 16, 16);
                menuGlyph.Bounds = new Rectangle(4+buttonSize + 1, 2, buttonSize, buttonSize); //this.ClientSize.Width - 20, 2, 16, 16);
                
            }

            DockPadding.Top = DEF_HEADER_HEIGHT + 1;
            DockPadding.Bottom = 1;
            DockPadding.Right = 1;
            DockPadding.Left = 1;
        }

        private void OnCollectionChanged(object sender, CollectionChangeEventArgs e) {
            FATabStripItem itm = (FATabStripItem)e.Element;

            if(e.Action == CollectionChangeAction.Add) {
                Controls.Add(itm);
                OnTabStripItemChanged(new TabStripItemChangedEventArgs(itm, FATabStripItemChangeTypes.Added));
            }
            else if(e.Action == CollectionChangeAction.Remove) {
                Controls.Remove(itm);
                OnTabStripItemChanged(new TabStripItemChangedEventArgs(itm, FATabStripItemChangeTypes.Removed));
            }
            else {
                OnTabStripItemChanged(new TabStripItemChangedEventArgs(itm, FATabStripItemChangeTypes.Changed));
            }

            UpdateLayout();
            Invalidate();
        }

        #endregion

        #endregion

        #region Ctor

        public FATabStrip() {
            BeginInit();

            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, true);

            items = new FATabStripItemCollection();
            items.CollectionChanged += new CollectionChangeEventHandler(OnCollectionChanged);
            base.Size = new Size(350, 200);

            menu = new ContextMenuStrip();
            menu.Renderer = ToolStripRenderer;
            menu.ItemClicked += new ToolStripItemClickedEventHandler(OnMenuItemClicked);
            menu.VisibleChanged += new EventHandler(OnMenuVisibleChanged);

            menuGlyph = new FATabStripMenuGlyph(ToolStripRenderer);
            closeButton = new FATabStripCloseButton(ToolStripRenderer);
            Font = defaultFont;
            sf = new StringFormat();

            EndInit();

            UpdateLayout();
        }

        #endregion

        #region Props

        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.All)]
        public FATabStripItem SelectedItem {
            get {
                return selectedItem;
            }
            set {
                if(selectedItem == value)
                    return;

                if(value == null && Items.Count > 0) {
                    FATabStripItem itm = Items[0];
                    if(itm.Visible) {
                        selectedItem = itm;
                        selectedItem.Selected = true;
                        selectedItem.Dock = DockStyle.Fill;
                    }
                }
                else {
                    selectedItem = value;
                }

                foreach(FATabStripItem itm in Items) {
                    if(itm == selectedItem) {
                        SelectItem(itm);
                        itm.Dock = DockStyle.Fill;
                        itm.Show();
                    }
                    else {
                        UnSelectItem(itm);
                        itm.Hide();
                    }
                }

                SelectItem(selectedItem);
                Invalidate();

                if(!selectedItem.IsDrawn) {
                    //对Items重新排序
                    //FATabStripItemCollection cache = new FATabStripItemCollection();
                    int index = 0;
                    int len = Items.Count;
                    for(int i = 0; i < len; i++) {
                        //cache.Add(Items[i]);
                        if(Items[i] == selectedItem) {
                            index = i;
                            break;
                        }
                    }
                    for(int i = len-1; i>=index; i--) {
                        var item = Items[len-1];
                        Items.MoveTo(0, item);
                    }
                    //Items.MoveTo(0, selectedItem);
                    Invalidate();
                }

                OnTabStripItemChanged(
                    new TabStripItemChangedEventArgs(selectedItem, FATabStripItemChangeTypes.SelectionChanged));
            }
        }

        [DefaultValue(true)]
        public bool AlwaysShowMenuGlyph {
            get {
                return alwaysShowMenuGlyph;
            }
            set {
                if(alwaysShowMenuGlyph == value)
                    return;

                alwaysShowMenuGlyph = value;
                Invalidate();
            }
        }

        [DefaultValue(true)]
        public bool AlwaysShowClose {
            get {
                return alwaysShowClose;
            }
            set {
                if(alwaysShowClose == value)
                    return;

                alwaysShowClose = value;
                Invalidate();
            }
        }
        [DefaultValue(true)]
        public bool AlwayDrawBorderLine {
            get {
                return alwayDrawBorderLine;
            }
            set {
                if(alwayDrawBorderLine == value)
                    return;

                alwayDrawBorderLine = value;
                Invalidate();
            }
        }
        [DefaultValue(true)]
        public bool AlwayDrawToolBarRegion {
            get {
                return alwayDrawToolBarRegion;
            }
            set {
                if(alwayDrawToolBarRegion == value)
                    return;

                alwayDrawToolBarRegion = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public FATabStripItemCollection Items {
            get {
                return items;
            }
        }

        [DefaultValue(typeof(Size), "350,200")]
        public new Size Size {
            get {
                return base.Size;
            }
            set {
                if(base.Size == value)
                    return;

                base.Size = value;
                UpdateLayout();
            }
        }

        /// <summary>
        /// DesignerSerializationVisibility
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ControlCollection Controls {
            get {
                return base.Controls;
            }
        }

        #endregion

        #region ShouldSerialize

        public bool ShouldSerializeFont() {
            return Font != null && !Font.Equals(defaultFont);
        }

        public bool ShouldSerializeSelectedItem() {
            return true;
        }

        public bool ShouldSerializeItems() {
            return items.Count > 0;
        }

        public new void ResetFont() {
            Font = defaultFont;
        }

        #endregion

        #region ISupportInitialize Members

        public void BeginInit() {
            isIniting = true;
        }

        public void EndInit() {
            isIniting = false;
        }

        #endregion

        #region IDisposable

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        protected override void Dispose(bool disposing) {
            if(disposing) {
                items.CollectionChanged -= new CollectionChangeEventHandler(OnCollectionChanged);
                menu.ItemClicked -= new ToolStripItemClickedEventHandler(OnMenuItemClicked);
                menu.VisibleChanged -= new EventHandler(OnMenuVisibleChanged);

                foreach(FATabStripItem item in items) {
                    if(item != null && !item.IsDisposed)
                        item.Dispose();
                }

                if(menu != null && !menu.IsDisposed)
                    menu.Dispose();

                if(sf != null)
                    sf.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}