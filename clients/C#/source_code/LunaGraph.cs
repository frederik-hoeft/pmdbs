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
        private List<GraphData> _graphData = new List<GraphData>() { new GraphData(4, ""), new GraphData(12, ""), new GraphData(99, ""), new GraphData(44, ""), new GraphData(11, ""), new GraphData(303, ""), new GraphData(42, ""), new GraphData(9, ""), new GraphData(17, "") };
        private Font _font = new Font("Segoe UI", 7f, GraphicsUnit.Point);
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
                
                while (space - (minSeperation * 2f) - 1.5f * entryHeight > 0)
                {
                    locations.Insert(locations.Count / 2, new PointF(minSeperation, minSeperation + shownDataPoints * minSeperation + shownDataPoints * entryHeight));
                    locations.Add(new PointF(minSeperation, Height - (entryHeight + minSeperation + shownDataPoints * minSeperation + shownDataPoints * entryHeight)));
                    shownDataPoints++;
                    space -= (minSeperation + entryHeight);
                }
                graphics.FillEllipse(new SolidBrush(Color.DarkRed), new RectangleF(new PointF(minSeperation, (Height - entryHeight) / 2f), new SizeF(entryHeight, entryHeight)));
                isReduced = true;
            }
            List<GraphData> shownGraphData = new List<GraphData>();
            shownGraphData.AddRange(_graphData.GetRange(0, shownDataPoints));
            shownGraphData.AddRange(_graphData.GetRange(_graphData.Count - shownDataPoints, shownDataPoints).AsEnumerable().Reverse());
            for (int i = 0; i < locations.Count; i++)
            {
                GraphData graphData;
                if (isReduced)
                {
                    graphData = shownGraphData[i];
                }
                else
                {
                    graphData = _graphData[i];
                } 
                graphics.FillEllipse(new SolidBrush(graphData.Color), new RectangleF(locations[i], new SizeF(entryHeight, entryHeight)));
                string label = graphData.X > 99 ? "99+" : graphData.X.ToString();
                SizeF labelSize = graphics.MeasureString(label, _font);
                float x = locations[i].X + ((entryHeight - labelSize.Width) / 2f);
                float y = locations[i].Y + ((entryHeight - labelSize.Height) / 2f);
                graphics.DrawString(label, _font, new SolidBrush(Color.White), new PointF(x, y));
            }
        }

        private void LunaGraph_SizeChanged(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
