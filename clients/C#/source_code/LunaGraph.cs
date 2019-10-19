using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pmdbs
{
    public partial class LunaGraph : UserControl
    {
        private List<GraphData> _graphData = new List<GraphData>() { new GraphData(4, "New Devices", Color.FromArgb(153, 0, 0)), new GraphData(11, "Logins", Color.FromArgb(27, 30, 214)), new GraphData(303, "Syncs", Color.FromArgb(38, 128, 235)), new GraphData(42, "Added Accounts", Color.FromArgb(53, 189, 151)), new GraphData(25, "Deleted Accounts", Color.FromArgb(214, 65, 101)) };
        private Font _font = new Font("Segoe UI", 8f, FontStyle.Bold, GraphicsUnit.Point);
        private List<GraphData> shownGraphData = new List<GraphData>();
        private Color _backColor = Color.FromArgb(220, 220, 220);
        public LunaGraph()
        {
            InitializeComponent();
        }

        private void LunaGraph_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            int sections = _graphData.Count;
            SizeF fontSize = graphics.MeasureString("99+", _font);
            float entryHeight = Math.Max(fontSize.Height, fontSize.Width);
            float minSeperation = (entryHeight / 4f) * 1.1f;
            List<PointF> locations = new List<PointF>();
            bool isReduced = false;
            int shownDataPoints = 0;
            if ((sections * entryHeight) + ((sections + 1) * minSeperation) < Height)
            {
                float seperation = (float)(Height - sections * entryHeight) / (float)(sections + 1f);
                for (int i = 0; i < sections; i++)
                {
                    locations.Add(new PointF(minSeperation, seperation + i * seperation + i * entryHeight));
                }
            }
            else
            {
                float space = Height / 2f;
                
                while (space - (minSeperation * 2f) - (1.5f * entryHeight) > 0)
                {
                    locations.Insert(locations.Count / 2, new PointF(minSeperation, minSeperation + (shownDataPoints * minSeperation) + (shownDataPoints * entryHeight)));
                    locations.Add(new PointF(minSeperation, Height - (entryHeight + minSeperation + (shownDataPoints * minSeperation) + (shownDataPoints * entryHeight))));
                    shownDataPoints++;
                    space -= (minSeperation + entryHeight);
                }
                graphics.FillEllipse(new SolidBrush(Color.DarkRed), new RectangleF(new PointF(minSeperation, (Height - entryHeight) / 2f), new SizeF(entryHeight, entryHeight)));
                isReduced = true;
            }
            shownGraphData.Clear();
            if (isReduced)
            {
                shownGraphData.AddRange(_graphData.GetRange(0, shownDataPoints));
                shownGraphData.AddRange(_graphData.GetRange(_graphData.Count - shownDataPoints, shownDataPoints).AsEnumerable().Reverse());
            }
            else
            {
                shownGraphData = _graphData.ConvertAll(graphData => graphData.Copy());
            }
            float maxNameWidth = 0;
            for (int i = 0; i < locations.Count; i++)
            {
                GraphData graphData = GetGraphData(isReduced, i);
                Brush brush = new SolidBrush(graphData.Color);
                graphics.FillEllipse(brush, new RectangleF(locations[i], new SizeF(entryHeight, entryHeight)));
                string label = graphData.X > 99 ? "99+" : graphData.X.ToString();
                SizeF labelSize = graphics.MeasureString(label, _font);
                float labelX = locations[i].X + ((entryHeight - labelSize.Width) / 2f);
                float labelY = locations[i].Y + ((entryHeight - labelSize.Height) / 2f);
                graphics.DrawString(label, _font, new SolidBrush(Color.White), new PointF(labelX, labelY));
                float nameX = locations[i].X + entryHeight + minSeperation;
                string name = graphData.Name;
                float nameWidth = graphics.MeasureString(name, _font).Width;
                if (nameWidth > maxNameWidth)
                {
                    maxNameWidth = nameWidth;
                }
                graphics.DrawString(name, _font, brush, new PointF(nameX, labelY));
            }
            int maxBarValue = shownGraphData.Max(entry => entry.X);
            Brush backgroundBrush = new SolidBrush(_backColor);
            for (int i = 0; i < locations.Count; i++)
            {
                PointF location = locations[i];
                GraphData graphData = GetGraphData(isReduced, i);
                Brush brush = new SolidBrush(graphData.Color);
                float leftBarX = location.X + entryHeight + (2f * minSeperation) + maxNameWidth;
                float rightBarX = Width - minSeperation - entryHeight;
                float barX = leftBarX + (entryHeight / 2f);
                float barWidth = rightBarX - leftBarX;
                float percentage = graphData.X / (float)maxBarValue;
                float percentageWidth = percentage * barWidth;
                float percentageRight = leftBarX + percentageWidth;
                graphics.FillEllipse(backgroundBrush, leftBarX, location.Y, entryHeight, entryHeight);
                graphics.FillRectangle(backgroundBrush, barX, location.Y, barWidth, entryHeight);
                graphics.FillEllipse(backgroundBrush, rightBarX, location.Y, entryHeight, entryHeight);
                if (percentage != 0)
                {
                    graphics.FillEllipse(brush, leftBarX, location.Y, entryHeight, entryHeight);
                    graphics.FillRectangle(brush, barX, location.Y, percentageWidth, entryHeight);
                    graphics.FillEllipse(brush, percentageRight, location.Y, entryHeight, entryHeight);
                }
            }
        }

        private GraphData GetGraphData(bool isReduced, int index)
        {
            return isReduced ? shownGraphData[index] : _graphData[index];
        }

        private void LunaGraph_SizeChanged(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
