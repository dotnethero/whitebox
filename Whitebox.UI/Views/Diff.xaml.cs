using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Whitebox.Types;

namespace Whitebox.UI.Views
{
    public partial class Diff
    {
        public Diff()
        {
            InitializeComponent();
        }

        private void Diff_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = e.NewValue as IEnumerable<Line>;
            if (context == null) return;

            var document = new FlowDocument();
            foreach (var line in context)
            {
                document.Blocks.Add(Convert(line));
            }
            RichDiff.Document = document;
        }

        private static Paragraph Convert(Line line)
        {
            return new Paragraph
            {
                Inlines = {line.Text},
                Background = new SolidColorBrush(Convert(line.Type)),
                Margin = new Thickness()
            };
        }

        private static Color Convert(LineType value)
        {
            switch (value)
            {
                case LineType.Add:
                    return Color.FromRgb(232, 249, 210);
                case LineType.Remove:
                    return Color.FromRgb(241, 218, 221);
                default:
                    return Color.FromArgb(0, 0, 0, 0);
            }
        }
    }
}
