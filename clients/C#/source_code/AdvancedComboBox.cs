#region Assembly MetroFramework, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a
// L:\Programming\C#\pmdbs\packages\MetroModernUI.1.4.0.0\lib\net\MetroFramework.dll
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Drawing;
using MetroFramework.Native;
using MetroFramework.Interfaces;

namespace pmdbs
{
    [ToolboxBitmap(typeof(ComboBox))]
    public class AdvancedComboBox : ComboBox
    {
        private string promptText = "";
        private bool drawPrompt;
        private bool isHovered;
        private bool isPressed;
        private bool displayFocusRectangle;
        private bool isFocused;
        public AdvancedComboBox()
        {
            base.SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
            base.DrawMode = DrawMode.OwnerDrawFixed;
            base.DropDownStyle = ComboBoxStyle.DropDownList;
            drawPrompt = (SelectedIndex == -1);
            Font = new Font("Century Gothic", 8F);
            ForeColor = Color.Black;
            BackColor = Color.White;
            NormalForeColor = Color.FromArgb(153, 153, 153);
            HoverForeColor = Color.FromArgb(51, 51, 51);
            PressForeColor = Color.FromArgb(153, 153, 153);
            DisabledForeColor = Color.FromArgb(204, 204, 204);
            NormalBorderColor = Color.FromArgb(153, 153, 153);
            HoverBorderColor = Color.FromArgb(51, 51, 51);
            PressBorderColor = Color.FromArgb(153, 153, 153);
            DisabledBorderColor = Color.FromArgb(204, 204, 204);
            HoverItemColor = Color.FromArgb(255, 96, 49);
            NormalItemColor = Color.White;
            
        }

        [Browsable(false)]
        [DefaultValue(DrawMode.OwnerDrawFixed)]
        public DrawMode DrawMode { get; set; }
        [Category("Metro Appearance")]
        [DefaultValue(MetroThemeStyle.Default)]
        public MetroThemeStyle Theme { get; set; }
        [Browsable(false)]
        public override Font Font { get; set; }
        [Browsable(true)]
        [Category("Metro Appearance")]
        [DefaultValue("")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string PromptText { get; set; }
        [Category("Metro Appearance")]
        [DefaultValue(MetroComboBoxWeight.Regular)]
        public MetroComboBoxWeight FontWeight { get; set; }
        [Category("Metro Appearance")]
        [DefaultValue(MetroComboBoxSize.Medium)]
        public MetroComboBoxSize FontSize { get; set; }
        [Browsable(false)]
        [DefaultValue(ComboBoxStyle.DropDownList)]
        public ComboBoxStyle DropDownStyle { get; set; }
        [Category("Metro Appearance")]
        [DefaultValue(MetroColorStyle.Default)]
        public MetroColorStyle Style { get; set; }
        [Category("Metro Appearance")]
        [DefaultValue(false)]
        public bool DisplayFocus { get; set; }
        [Browsable(false)]
        [Category("Metro Behaviour")]
        [DefaultValue(false)]
        public bool UseSelectable { get; set; }
        [Category("Metro Appearance")]
        [DefaultValue(false)]
        public bool UseStyleColors { get; set; }
        [Category("Metro Appearance")]
        [DefaultValue(false)]
        public bool UseCustomForeColor { get; set; }
        [Category("Metro Appearance")]
        [DefaultValue(false)]
        public bool UseCustomBackColor { get; set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MetroStyleManager StyleManager { get; set; }
        [Browsable(false)]
        public override Color ForeColor { get; set; }
        public Color NormalForeColor { get; set; }
        public Color HoverForeColor { get; set; }
        public Color PressForeColor { get; set; }
        public Color DisabledForeColor { get; set; }
        public Color NormalBorderColor { get; set; }
        public Color HoverBorderColor { get; set; }
        public Color PressBorderColor { get; set; }
        public Color DisabledBorderColor { get; set; }

        [Category("Metro Appearance")]
        public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
        [Category("Metro Appearance")]
        public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;
        [Category("Metro Appearance")]
        public event EventHandler<MetroPaintEventArgs> CustomPaint;

        public override Size GetPreferredSize(Size proposedSize)
        {
            base.GetPreferredSize(proposedSize);
            using (Graphics dc = base.CreateGraphics())
            {
                string text = (Text.Length > 0) ? Text : "MeasureText";
                proposedSize = new Size(2147483647, 2147483647);
                Size result = TextRenderer.MeasureText(dc, text, this.Font, proposedSize, TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding);
                result.Height += 4;
                return result;
            }
        }
        protected virtual void OnCustomPaint(MetroPaintEventArgs e)
        {
            if (base.GetStyle(ControlStyles.UserPaint) && this.CustomPaint != null)
            {
                this.CustomPaint(this, e);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (base.GetStyle(ControlStyles.AllPaintingInWmPaint))
                {
                    OnPaintBackground(e);
                }
                OnCustomPaint(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
                OnPaintForeground(e);
            }
            catch
            {
                base.Invalidate();
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            try
            {
                Color color = BackColor;
                if (color.A == 255 && BackgroundImage == null)
                {
                    e.Graphics.Clear(color);
                }
                else
                {
                    base.OnPaintBackground(e);
                    OnCustomPaintBackground(new MetroPaintEventArgs(color, Color.Empty, e.Graphics));
                }
            }
            catch
            {
                base.Invalidate();
            }
        }
        protected virtual void OnCustomPaintBackground(MetroPaintEventArgs e)
        {
            if (base.GetStyle(ControlStyles.UserPaint) && this.CustomPaintBackground != null)
            {
                this.CustomPaintBackground(this, e);
            }
        }
        protected virtual void OnPaintForeground(PaintEventArgs e)
        {
            base.ItemHeight = GetPreferredSize(Size.Empty).Height;
            Color color;
            Color color2;
            if (isHovered && !isPressed && base.Enabled)
            {
                color = HoverForeColor;
                color2 = HoverBorderColor;
            }
            else if (isHovered && isPressed && base.Enabled)
            {
                color = PressForeColor;
                color2 = PressBorderColor;
            }
            else if (!base.Enabled)
            {
                color = DisabledForeColor;
                color2 = DisabledBorderColor;
            }
            else
            {
                color = NormalForeColor;
                color2 = NormalBorderColor;
            }
            using (Pen pen = new Pen(color2))
            {
                Rectangle rect = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
                e.Graphics.DrawRectangle(pen, rect);
            }
            using (SolidBrush brush = new SolidBrush(color))
            {
                e.Graphics.FillPolygon(brush, new Point[3]
                {
            new Point(base.Width - 20, base.Height / 2 - 2),
            new Point(base.Width - 9, base.Height / 2 - 2),
            new Point(base.Width - 15, base.Height / 2 + 4)
                });
            }
            Rectangle bounds = new Rectangle(2, 2, base.Width - 20, base.Height - 4);
            TextRenderer.DrawText(e.Graphics, Text, this.Font, bounds, color, TextFormatFlags.VerticalCenter);
            OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, color, e.Graphics));
            if (displayFocusRectangle && isFocused)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, base.ClientRectangle);
            }
            if (drawPrompt)
            {
                DrawTextPrompt(e.Graphics);
            }
        }
        protected virtual void OnCustomPaintForeground(MetroPaintEventArgs e)
        {
            if (base.GetStyle(ControlStyles.UserPaint) && this.CustomPaintForeground != null)
            {
                this.CustomPaintForeground(this, e);
            }
        }
        private void DrawTextPrompt(Graphics g)
        {
            Color backColor = BackColor;
            Rectangle bounds = new Rectangle(2, 2, base.Width - 20, base.Height - 4);
            TextRenderer.DrawText(g, promptText, this.Font, bounds, SystemColors.GrayText, backColor, TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            drawPrompt = (SelectedIndex == -1);
            base.Invalidate();
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg != 15 && m.Msg != 8465)
            {
                return;
            }
            if (drawPrompt)
            {
                DrawTextPrompt();
            }
        }
        private void DrawTextPrompt()
        {
            using (Graphics g = base.CreateGraphics())
            {
                DrawTextPrompt(g);
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                Color color = BackColor;
                Color foreColor;
                if (e.State == (DrawItemState.NoAccelerator | DrawItemState.NoFocusRect) || e.State == DrawItemState.None)
                {
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height));
                    }
                    foreColor = Color.White;
                }
                else
                {
                    using (SolidBrush brush2 = new SolidBrush(HoverItemColor)
                    {
                        e.Graphics.FillRectangle(brush2, new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height));
                    }
                    foreColor = ;
                }
                Rectangle bounds = new Rectangle(0, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);
                TextRenderer.DrawText(e.Graphics, base.GetItemText(base.Items[e.Index]), this.Font, bounds, foreColor, TextFormatFlags.VerticalCenter);
            }
            else
            {
                base.OnDrawItem(e);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            isFocused = true;
            isHovered = true;
            base.Invalidate();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            isFocused = false;
            isHovered = false;
            isPressed = false;
            base.Invalidate();
            base.OnLostFocus(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            isFocused = true;
            isHovered = true;
            base.Invalidate();
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            isFocused = false;
            isHovered = false;
            isPressed = false;
            base.Invalidate();
            base.OnLeave(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                isHovered = true;
                isPressed = true;
                base.Invalidate();
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.Invalidate();
            base.OnKeyUp(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            base.Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;
                base.Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isPressed = false;
            base.Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!isFocused)
            {
                isHovered = false;
            }
            base.Invalidate();
            base.OnMouseLeave(e);
        }
    }
}