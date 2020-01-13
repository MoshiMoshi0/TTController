using System.Collections.Generic;
using System.Linq;

namespace TTController.Common
{
    public class LedColorGradient
    {
        private readonly List<LedColorGradientPoint> _points;

        public IReadOnlyList<LedColorGradientPoint> Points => _points;
        public LedColorGradientPoint Start => _points.First();
        public LedColorGradientPoint End => _points.Last();

        public LedColorGradient()
        {
            _points = new List<LedColorGradientPoint>();
        }

        public LedColorGradient(IEnumerable<LedColorGradientPoint> points)
        {
            _points = points.OrderBy(p => p.Location).ToList();
        }

        public LedColorGradient(IEnumerable<LedColor> colors, double locationScale = 1.0) : this()
        {
            var colorList = colors.ToList();
            for (var i = 0; i < colorList.Count; i++)
                _points.Add(new LedColorGradientPoint(locationScale * i / (colorList.Count - 1), colorList[i]));
        }

        public LedColor GetColor(double location)
        {
            var (r, g, b) = GetColorSmooth(location);
            return new LedColor((byte)r, (byte)g, (byte)b);
        }

        public (double, double, double) GetColorSmooth(double location)
        {
            if(_points.Count == 0)
                return (0, 0, 0);

            var i = 0;
            LedColorGradientPoint start, end;
            do
            {
                start = _points[i++];
                if (i >= _points.Count)
                    return (start.Color.R, start.Color.G, start.Color.B);

                end = _points[i];
            } while (!(location >= start.Location && location <= end.Location));

            var correctedLocation = (location - start.Location) / (end.Location - start.Location);
            return LedColor.LerpSmooth(correctedLocation, start.Color, end.Color);
        }
    }

    public class LedColorGradientPoint
    {
        public double Location { get; }
        public LedColor Color { get; }

        public LedColorGradientPoint(double location, LedColor color)
        {
            Location = location;
            Color = color;
        }
    }
}
