using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LunaForms;
using System.Runtime.InteropServices;

namespace pmdbs
{
    public partial class LunaItemList : UserControl
    {
        #region LunaItemList
        private int _scrollBarMargin = 30;
        private bool _isScrollable = true;
        private bool showScrollBar = false;
        private List<LunaItem> _items = new List<LunaItem>();
        private List<Control> controls = new List<Control>();
        private int _seperatorVerticalPadding = 6;
        private bool _showBorderOnScrollBarShown = false;
        private Color _backColor = Color.White;
        #endregion
        #region LunaItem
        private bool _itemShowInfo = false;
        private string _itemHeader = "Item";
        private string _itemInfo = "Info";
        private Font _itemFont = new Font("Segoe UI", 25, GraphicsUnit.Pixel);
        private int _infoFontSizePx = 15;
        private Color _itemForeColorHeader = Color.Orange;
        private Color _itemForeColorHeaderHover = Color.Orange;
        private Color _itemForeColorInfo = Color.FromArgb(100, 100, 100);
        private Color _itemForeColorInfoHover = Color.FromArgb(64, 64, 64);
        private Color _itemBackColorHover = Color.FromArgb(220, 220, 220);
        private Color _itemBackColor = Color.White;
        private Point _itemHeaderLocation = new Point(70, 0);
        private Point _itemInfoLocation = new Point(72, 35);
        private Point _itemImageLocation = new Point(1, 1);
        private int _itemSteps = 20;
        private int _itemAnimationInterval = 10;
        #endregion

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        
        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        #region EventHandlers
        /// <summary>
        /// Occurs when a LunaItem is clicked. Should be used instead of LunaItem.Click
        /// </summary>
        public event EventHandler LunaItemClicked;
        #endregion

        public LunaItemList()
        {
            InitializeComponent();
            DoubleBuffered = true;
            if (DesignMode)
            {
                Size = new Size(200, 100);
            }
            Controls_Changed();
        }

        #region Getters / Setters
        #region Scrollbar
        public Color LunaScrollBarForeColor
        {
            get { return lunaScrollBar1.ForeColor; }
            set { lunaScrollBar1.ForeColor = value; }
        }

        public Color LunaScrollBarForeColorHover
        {
            get { return lunaScrollBar1.ForeColorHover; }
            set { lunaScrollBar1.ForeColorHover = value; }
        }

        public Color LunaScrollBarBackColorScrollBar
        {
            get { return lunaScrollBar1.BackColorScrollBar; }
            set { lunaScrollBar1.BackColorScrollBar = value; }
        }

        #endregion
        #region LunaItem
        /// <summary>
        /// The main text to be displayed.
        /// </summary>
        public string LunaItemHeader
        {
            get { return _itemHeader; }
            set
            {
                _itemHeader = value;
            }
        }

        /// <summary>
        /// Additional information to be displayed.
        /// </summary>
        [DefaultValue("Info")]
        public string LunaItemInfo
        {
            get { return _itemInfo; }
            set
            {
                _itemInfo = value;
            }
        }

        /// <summary>
        /// The font top be used to draw both Header and Info.
        /// </summary>
        public Font LunaItemFont
        {
            get { return _itemFont; }
            set
            {
                _itemFont = value;
            }
        }

        /// <summary>
        /// The font size in pixels of the info text.
        /// </summary>
        [DefaultValue(15)]
        public int LunaItemInfoFontSizePx
        {
            get { return _infoFontSizePx; }
            set { _infoFontSizePx = value; }
        }

        /// <summary>
        /// The foreground color of the header text.
        /// </summary>
        public Color LunaItemForeColorHeader
        {
            get { return _itemForeColorHeader; }
            set { _itemForeColorHeader = value; }
        }

        /// <summary>
        /// The foreground color of the header text when the user hovers over it.
        /// </summary>
        public Color LunaItemForeColorHeaderHover
        {
            get { return _itemForeColorHeaderHover; }
            set { _itemForeColorHeaderHover = value; }
        }

        /// <summary>
        /// The foreground color of the info text when the user hovers over it.
        /// </summary>
        public Color LunaItemForeColorInfoHover
        {
            get { return _itemForeColorInfoHover; }
            set { _itemForeColorInfoHover = value; }
        }

        /// <summary>
        /// The foreground color of the info text.
        /// </summary>
        public Color LunaItemForeColorInfo
        {
            get { return _itemForeColorInfo; }
            set
            {
                _itemForeColorInfo = value;
            }
        }

        /// <summary>
        /// The background color of the LunaItem.
        /// </summary>
        public Color LunaItemBackColorNormal
        {
            get { return _itemBackColor; }
            set { _itemBackColor = value; }
        }

        /// <summary>
        /// The background color of the LunaItem when the user hovers over it.
        /// </summary>
        public Color LunaItemBackColorHover
        {
            get { return _itemBackColorHover; }
            set { _itemBackColorHover = value; }
        }

        /// <summary>
        /// The location of the top left corner of the image.
        /// </summary>
        public Point ImageLocation
        {
            get { return _itemImageLocation; }
            set { _itemImageLocation = value; }
        }

        /// <summary>
        /// The location of the top left corner of the header text.
        /// </summary>
        public Point LunaItemHeaderLocation
        {
            get { return _itemHeaderLocation; }
            set { _itemHeaderLocation = value; }
        }

        /// <summary>
        /// The location of the top left corner of the info text.
        /// </summary>
        public Point LunaItemInfoLocation
        {
            get { return _itemInfoLocation; }
            set
            {
                _itemInfoLocation = value;
            }
        }

        /// <summary>
        /// Controls whether the info text is drawn or not.
        /// </summary>
        [DefaultValue(true)]
        public bool LunaItemShowInfo
        {
            get { return _itemShowInfo; }
            set
            {
                _itemShowInfo = value;
                if (_itemShowInfo)
                {
                    _itemHeaderLocation = new Point(70, 0);
                    _itemInfoLocation = new Point(72, 35);
                }
                else
                {
                    _itemHeaderLocation = new Point(70, 10);
                }
            }
        }

        /// <summary>
        /// Controls how smooth the color transition is from the BackColor to BackColorHover. More _steps means smoother transition.
        /// </summary>
        [DefaultValue(20)]
        public int LunaItemAnimationSteps
        {
            get { return _itemSteps; }
            set { _itemSteps = value; }
        }

        /// <summary>
        /// The interval in ms in which the Animations goes through it's animation steps. The total animation duration is AnimationInterval * AnimationSteps.
        /// </summary>
        [DefaultValue(10)]
        public int LunaItemAnimationInterval
        {
            get { return _itemAnimationInterval; }
            set { _itemAnimationInterval = value; }
        }
        #endregion
        #region LunaItemList

        public bool ShowBorderOnScrollBarShown
        {
            get { return _showBorderOnScrollBarShown; }
            set
            {
                _showBorderOnScrollBarShown = value;
                Controls_Changed();
            }
        }

        public int ScrollBarMargin
        {
            get { return _scrollBarMargin; }
            set
            {
                _scrollBarMargin = value;
                Controls_Changed();
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            LunaItemList_SizeChanged(this, null);
            Controls_Changed();
        }

        public bool IsScrollable
        {
            get { return _isScrollable; }
            set { _isScrollable = value; }
        }

        /// <summary>
        /// Gets the list of LunaSmallCards.
        /// </summary>
        public List<LunaItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// The total vertical padding to be applied inbetween seperate LunaSmallCards.
        /// </summary>
        public int SeperatorVerticalPadding
        {
            get { return _seperatorVerticalPadding; }
            set { _seperatorVerticalPadding = value; }
        }


        public override Color BackColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
                BackColor = value;
                panel1.BackColor = value;
                lunaScrollBar1.BackColor = value;
                flowLayoutPanel1.BackColor = value;
            }
        }
        /// <summary>
        /// Checks if List has capacity for another entry without having to display the scrollbar.
        /// </summary>
        public bool CheckCapacity
        {
            get { return checkCapacity(); }
        }
        #endregion
        #endregion

        #region Functionality
        /// <summary>
        /// Adds a LunaItem to the end of the List.
        /// </summary>
        /// <param name="header">The Header of the LunaItem.</param>
        /// <param name="image">The Image of the LunaItem.</param>
        /// <param name="id">The ID of the LunaItem.</param>
        public void Add(string header, Image image, string id, int index)
        {
            if (!checkCapacity())
            {
                if (!_isScrollable)
                {
                    return;
                }
            }
            if (_items.Count > 0)
            {
                LunaSeperator seperator = new LunaSeperator
                {
                    Location = new Point(0, 66),
                    Padding = new Padding(5, 0, 5, 0),
                    Size = new Size(Width, 7),
                    TabIndex = 1
                };
                flowLayoutPanel1.Controls.Add(seperator);
                controls.Add(seperator);
                seperator.Refresh();
            }
            LunaItem item = new LunaItem
            {
                ShowInfo = false,
                AnimationInterval = _itemAnimationInterval,
                AnimationSteps = _itemSteps,
                BackColorHover = _itemBackColorHover,
                BackColorImage = Color.White,
                BackColorNormal = _backColor,
                ForeColorHeader = _itemForeColorHeader,
                ForeColorInfo = _itemForeColorInfo,
                Font = _itemFont,
                InfoFontSizePx = _infoFontSizePx,
                Header = header,
                HeaderLocation = _itemHeaderLocation,
                Image = image,
                InfoLocation = _itemInfoLocation,
                ShowBorder = false,
                Size = new Size(showScrollBar ? flowLayoutPanel1.Width - _scrollBarMargin : flowLayoutPanel1.Width, 60),
                Id = id,
                VerticalPadding = _seperatorVerticalPadding,
                ImageLocation = _itemImageLocation,
                ForeColorHeaderHover = _itemForeColorHeaderHover,
                ForeColorInfoHover = _itemForeColorInfoHover,
                Index = index
            };
            item.OnClickEvent += Control_clicked;
            item.MouseEnter += LunaItem_MouseEnter;
            flowLayoutPanel1.Controls.Add(item);
            controls.Add(item);
            _items.Add(item);
            item.Refresh();
        }

        /// <summary>
        /// Adds a LunaItem to the end of the List.
        /// </summary>
        /// <param name="header">The Header of the LunaItem.</param>
        /// <param name="image">The Image of the LunaItem.</param>
        /// <param name="id">The ID of the LunaItem.</param>
        public void Add(string header, Image image, string id, int index, int index2)
        {
            if (!checkCapacity())
            {
                if (!_isScrollable)
                {
                    return;
                }
            }
            if (_items.Count > 0)
            {
                LunaSeperator seperator = new LunaSeperator
                {
                    Location = new Point(0, 66),
                    Padding = new Padding(5, 0, 5, 0),
                    Size = new Size(Width, 7),
                    TabIndex = 1
                };
                flowLayoutPanel1.Controls.Add(seperator);
                controls.Add(seperator);
                seperator.Refresh();
            }
            LunaItem item = new LunaItem
            {
                ShowInfo = false,
                AnimationInterval = _itemAnimationInterval,
                AnimationSteps = _itemSteps,
                BackColorHover = _itemBackColorHover,
                BackColorImage = Color.White,
                BackColorNormal = _backColor,
                ForeColorHeader = _itemForeColorHeader,
                ForeColorInfo = _itemForeColorInfo,
                Font = _itemFont,
                InfoFontSizePx = _infoFontSizePx,
                Header = header,
                HeaderLocation = _itemHeaderLocation,
                Image = image,
                InfoLocation = _itemInfoLocation,
                ShowBorder = false,
                Size = new Size(showScrollBar ? flowLayoutPanel1.Width - _scrollBarMargin : flowLayoutPanel1.Width, 60),
                Id = id,
                VerticalPadding = _seperatorVerticalPadding,
                ImageLocation = _itemImageLocation,
                ForeColorHeaderHover = _itemForeColorHeaderHover,
                ForeColorInfoHover = _itemForeColorInfoHover,
                Index = index,
                Index2 = index2
            };
            item.OnClickEvent += Control_clicked;
            item.MouseEnter += LunaItem_MouseEnter;
            flowLayoutPanel1.Controls.Add(item);
            controls.Add(item);
            _items.Add(item);
            item.Refresh();
        }

        /// <summary>
        /// Adds a LunaItem to the end of the List.
        /// </summary>
        /// <param name="header">The Header of the LunaItem.</param>
        /// <param name="image">The Image of the LunaItem.</param>
        /// <param name="id">The ID of the LunaItem.</param>
        public void Add(string header, Image image, string id, int index, int index2, int index3)
        {
            if (!checkCapacity())
            {
                if (!_isScrollable)
                {
                    return;
                }
            }
            if (_items.Count > 0)
            {
                LunaSeperator seperator = new LunaSeperator
                {
                    Location = new Point(0, 66),
                    Padding = new Padding(5, 0, 5, 0),
                    Size = new Size(Width, 7),
                    TabIndex = 1
                };
                flowLayoutPanel1.Controls.Add(seperator);
                controls.Add(seperator);
                seperator.Refresh();
            }
            LunaItem item = new LunaItem
            {
                ShowInfo = false,
                AnimationInterval = _itemAnimationInterval,
                AnimationSteps = _itemSteps,
                BackColorHover = _itemBackColorHover,
                BackColorImage = Color.White,
                BackColorNormal = _backColor,
                ForeColorHeader = _itemForeColorHeader,
                ForeColorInfo = _itemForeColorInfo,
                Font = _itemFont,
                InfoFontSizePx = _infoFontSizePx,
                Header = header,
                HeaderLocation = _itemHeaderLocation,
                Image = image,
                InfoLocation = _itemInfoLocation,
                ShowBorder = false,
                Size = new Size(showScrollBar ? flowLayoutPanel1.Width - _scrollBarMargin : flowLayoutPanel1.Width, 60),
                Id = id,
                VerticalPadding = _seperatorVerticalPadding,
                ImageLocation = _itemImageLocation,
                ForeColorHeaderHover = _itemForeColorHeaderHover,
                ForeColorInfoHover = _itemForeColorInfoHover,
                Index = index,
                Index2 = index2,
                Index3 = index3
            };
            item.OnClickEvent += Control_clicked;
            item.MouseEnter += LunaItem_MouseEnter;
            flowLayoutPanel1.Controls.Add(item);
            controls.Add(item);
            _items.Add(item);
            item.Refresh();
        }

        /// <summary>
        /// Adds a LunaItem to the end of the List.
        /// </summary>
        /// <param name="header">The Header of the LunaItem.</param>
        /// <param name="image">The Image of the LunaItem.</param>
        /// <param name="info">The Info of the LunaItem.</param>
        /// <param name="id">The ID of the LunaItem.</param>
        public void Add(string header, Image image, string info, string id, int index)
        {
            if (!checkCapacity())
            {
                if (!_isScrollable)
                {
                    return;
                }
            }
            if (_items.Count > 0)
            {
                LunaSeperator seperator = new LunaSeperator
                {
                    Location = new Point(0, 66),
                    Padding = new Padding(5, 0, 5, 0),
                    Size = new Size(Width, 7),
                    TabIndex = 1
                };
                flowLayoutPanel1.Controls.Add(seperator);
                seperator.Refresh();
            }
            LunaItem item = new LunaItem
            {
                ShowInfo = true,
                AnimationInterval = _itemAnimationInterval,
                AnimationSteps = _itemSteps,
                BackColorHover = _itemBackColorHover,
                BackColorImage = Color.White,
                BackColorNormal = _backColor,
                ForeColorHeader = _itemForeColorHeader,
                ForeColorInfo = _itemForeColorInfo,
                Font = _itemFont,
                InfoFontSizePx = _infoFontSizePx,
                Header = header,
                HeaderLocation = _itemHeaderLocation,
                Image = image,
                Info = info,
                InfoLocation = _itemInfoLocation,
                ShowBorder = false,
                Size = new Size(showScrollBar ? flowLayoutPanel1.Width - _scrollBarMargin : flowLayoutPanel1.Width, 60),
                Id = id,
                VerticalPadding = _seperatorVerticalPadding,
                ImageLocation = _itemImageLocation,
                ForeColorHeaderHover = _itemForeColorHeaderHover,
                ForeColorInfoHover = _itemForeColorInfoHover,
                Index = index
            };
            item.ShowInfo = true;
            item.VerticalPadding = 0;
            item.VerticalPadding = _seperatorVerticalPadding;
            item.OnClickEvent += Control_clicked;
            item.MouseEnter += LunaItem_MouseEnter;
            flowLayoutPanel1.Controls.Add(item);
            _items.Add(item);
            item.Refresh();
        }

        /// <summary>
        /// Adds a LunaItem to the end of the List.
        /// </summary>
        /// <param name="header">The Header of the LunaItem.</param>
        /// <param name="image">The Image of the LunaItem.</param>
        /// <param name="info">The Info of the LunaItem.</param>
        /// <param name="id">The ID of the LunaItem.</param>
        public void Add(string header, Image image, string info, string id, int index, int index2)
        {
            if (!checkCapacity())
            {
                if (!_isScrollable)
                {
                    return;
                }
            }
            if (_items.Count > 0)
            {
                LunaSeperator seperator = new LunaSeperator
                {
                    Location = new Point(0, 66),
                    Padding = new Padding(5, 0, 5, 0),
                    Size = new Size(Width, 7),
                    TabIndex = 1
                };
                flowLayoutPanel1.Controls.Add(seperator);
                seperator.Refresh();
            }
            LunaItem item = new LunaItem
            {
                ShowInfo = true,
                AnimationInterval = _itemAnimationInterval,
                AnimationSteps = _itemSteps,
                BackColorHover = _itemBackColorHover,
                BackColorImage = Color.White,
                BackColorNormal = _backColor,
                ForeColorHeader = _itemForeColorHeader,
                ForeColorInfo = _itemForeColorInfo,
                Font = _itemFont,
                InfoFontSizePx = _infoFontSizePx,
                Header = header,
                HeaderLocation = _itemHeaderLocation,
                Image = image,
                Info = info,
                InfoLocation = _itemInfoLocation,
                ShowBorder = false,
                Size = new Size(showScrollBar ? flowLayoutPanel1.Width - _scrollBarMargin : flowLayoutPanel1.Width, 60),
                Id = id,
                VerticalPadding = _seperatorVerticalPadding,
                ImageLocation = _itemImageLocation,
                ForeColorHeaderHover = _itemForeColorHeaderHover,
                ForeColorInfoHover = _itemForeColorInfoHover,
                Index = index,
                Index2 = index2
            };
            item.ShowInfo = true;
            item.VerticalPadding = 0;
            item.VerticalPadding = _seperatorVerticalPadding;
            item.OnClickEvent += Control_clicked;
            item.MouseEnter += LunaItem_MouseEnter;
            flowLayoutPanel1.Controls.Add(item);
            _items.Add(item);
            item.Refresh();
        }

        /// <summary>
        /// Adds a LunaItem to the end of the List.
        /// </summary>
        /// <param name="header">The Header of the LunaItem.</param>
        /// <param name="image">The Image of the LunaItem.</param>
        /// <param name="info">The Info of the LunaItem.</param>
        /// <param name="id">The ID of the LunaItem.</param>
        public void Add(string header, Image image, string info, string id, int index, int index2, int index3)
        {
            if (!checkCapacity())
            {
                if (!_isScrollable)
                {
                    return;
                }
            }
            if (_items.Count > 0)
            {
                LunaSeperator seperator = new LunaSeperator
                {
                    Location = new Point(0, 66),
                    Padding = new Padding(5, 0, 5, 0),
                    Size = new Size(Width, 7),
                    TabIndex = 1
                };
                flowLayoutPanel1.Controls.Add(seperator);
                seperator.Refresh();
            }
            LunaItem item = new LunaItem
            {
                ShowInfo = true,
                AnimationInterval = _itemAnimationInterval,
                AnimationSteps = _itemSteps,
                BackColorHover = _itemBackColorHover,
                BackColorImage = Color.White,
                BackColorNormal = _backColor,
                ForeColorHeader = _itemForeColorHeader,
                ForeColorInfo = _itemForeColorInfo,
                Font = _itemFont,
                InfoFontSizePx = _infoFontSizePx,
                Header = header,
                HeaderLocation = _itemHeaderLocation,
                Image = image,
                Info = info,
                InfoLocation = _itemInfoLocation,
                ShowBorder = false,
                Size = new Size(showScrollBar ? flowLayoutPanel1.Width - _scrollBarMargin : flowLayoutPanel1.Width, 60),
                Id = id,
                VerticalPadding = _seperatorVerticalPadding,
                ImageLocation = _itemImageLocation,
                ForeColorHeaderHover = _itemForeColorHeaderHover,
                ForeColorInfoHover = _itemForeColorInfoHover,
                Index = index,
                Index2 = index2,
                Index3 = index3
            };
            item.ShowInfo = true;
            item.VerticalPadding = 0;
            item.VerticalPadding = _seperatorVerticalPadding;
            item.OnClickEvent += Control_clicked;
            item.MouseEnter += LunaItem_MouseEnter;
            flowLayoutPanel1.Controls.Add(item);
            _items.Add(item);
            item.Refresh();
        }

        /// <summary>
        /// Removes the LunaItem at the specified index.
        /// </summary>
        /// <param name="index">The inted to remove the LunaItem at.</param>
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
            Control control = flowLayoutPanel1.Controls[index];
            // REMOVE EVENTHANDLERS
            if (control is LunaItem item)
            {
                item.MouseEnter -= LunaItem_MouseEnter;
                item.OnClickEvent -= Control_clicked;
            }
            flowLayoutPanel1.Controls.RemoveAt(index);
            control.Dispose();
            if (index < 0)
            {
                control = flowLayoutPanel1.Controls[index - 1];
                if (control is LunaItem)
                {
                    item = (LunaItem)control;
                    item.MouseEnter -= LunaItem_MouseEnter;
                    item.OnClickEvent -= Control_clicked;
                }
                flowLayoutPanel1.Controls.RemoveAt(index - 1);
                control.Dispose();
            }
        }

        /// <summary>
        /// Removes all LunaItems from the List.
        /// </summary>
        public void RemoveAll()
        {
            _items = new List<LunaItem>();
            int count = flowLayoutPanel1.Controls.Count;
            flowLayoutPanel1.Controls.Clear();
            for (int i = count - 1; i >= 0; i--)
            {
                Control control = controls[i];
                if (control is LunaItem item)
                {
                    item.MouseEnter -= LunaItem_MouseEnter;
                    item.OnClickEvent -= Control_clicked;
                }
                control.Dispose();
            }
            controls = new List<Control>();
        }

        /// <summary>
        /// Checks if List has capacity for another entry.
        /// </summary>
        /// <returns>True if there is enough capacity for another entry.</returns>
        private bool checkCapacity()
        {
            if (_items.Count > 0)
            {
                return _items.Last().Location.Y + _seperatorVerticalPadding + 1 + 2 * 60 < flowLayoutPanel1.ClientSize.Height;
            }
            return 60 < flowLayoutPanel1.ClientSize.Height;
        }
        #endregion

        #region EventHandlers

        private void Control_clicked(object sender, EventArgs e)
        {
            ClickEvent(sender, e);
        }

        protected virtual void ClickEvent(object sender, EventArgs e)
        {
            LunaItemClicked?.Invoke(sender, e);
        }

        private void LunaItemList_SizeChanged(object sender, EventArgs e)
        {
            flowLayoutPanel1.Size = Size;
            panel1.Height = Height;
            panel1.Width = showScrollBar ? Width - _scrollBarMargin : Width;
            lunaScrollBar1.Location = new Point(Width - lunaScrollBar1.ScrollbarSize - 1, 0);
            lunaScrollBar1.Height = Height;
            Resize_Items();
        }

        private void Resize_Items()
        {
            if (showScrollBar)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    controls[i].Width = flowLayoutPanel1.Width - _scrollBarMargin;
                }
            }
            else
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    controls[i].Width = flowLayoutPanel1.Width;
                }
            }
        }

        private void lunaScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            flowLayoutPanel1.AutoScrollPosition = new Point(0, lunaScrollBar1.Value);
            lunaScrollBar1.Invalidate();
            Application.DoEvents();
        }

        private void flowLayoutPanel1_Scroll(object sender, ScrollEventArgs e)
        {
            lunaScrollBar1.Value = flowLayoutPanel1.AutoScrollPosition.Y;
        }

        private void flowLayoutPanel1_ControlAdded(object sender, ControlEventArgs e)
        {
            Controls_Changed();
        }

        private void flowLayoutPanel1_ControlRemoved(object sender, ControlEventArgs e)
        {
            Controls_Changed();
        }

        private void LunaItem_MouseEnter(object sender, EventArgs e)
        {
            if (showScrollBar)
            {
                lunaScrollBar1.Focus();
            }
        }

        private void Controls_Changed()
        {
            if (flowLayoutPanel1.VerticalScroll.Visible)
            {
                if (!showScrollBar)
                {
                    showScrollBar = true;
                    LunaItemList_SizeChanged(this, null);
                    if (_showBorderOnScrollBarShown)
                    {
                        BorderStyle = BorderStyle.FixedSingle;
                    }
                }
            }
            else
            {
                if (showScrollBar)
                {
                    showScrollBar = false;
                    LunaItemList_SizeChanged(this, null);
                    if (_showBorderOnScrollBarShown)
                    {
                        BorderStyle = BorderStyle.None;
                    }
                }
            }
            lunaScrollBar1.Maximum = flowLayoutPanel1.VerticalScroll.Maximum;
            lunaScrollBar1.Value = flowLayoutPanel1.VerticalScroll.Value;
            lunaScrollBar1.Minimum = flowLayoutPanel1.VerticalScroll.Minimum;
            lunaScrollBar1.LargeChange = flowLayoutPanel1.VerticalScroll.LargeChange;
            lunaScrollBar1.SmallChange = flowLayoutPanel1.VerticalScroll.SmallChange;
            if (flowLayoutPanel1.HorizontalScroll.Visible)
            {
                ShowScrollBar(flowLayoutPanel1.Handle, (int)ScrollBarDirection.SB_HORZ, false);
            }
        }
        #endregion
    }
}
