using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class CustomGrid : CustomLayout
    {

        protected override ILayoutManager CreateLayoutManager()
        {
            return new CustomLayoutManager(this);
        }

        double scrollX;

        public double TotalWidth= 3600;

        public double TotalHeight = 500;

        public double ScrollX
        {
            get { return scrollX; }
            set
            {
                if (scrollX != value)
                {
                    scrollX = value;
                    Invalidate();
                }
            }
        }

        internal void Invalidate()
        {
            foreach (var item in this.Children)
                (item as IView).InvalidateMeasure();
        }

        internal override Size MeasureChildren(double widthConstraint, double heightConstraint)
        {
            if (this.Children.Count == 0)
                for (int i = 0; i < 20; i++)
                    this.Children.Add(new Row() { RowIndex = i, customGrid = this });

            foreach (var row in this.Children)
                (row as IView)!.Measure(TotalWidth, 50);

            return new SizeRequest(new Size(TotalWidth, TotalHeight));
        }

        internal override Size ArrangeChildren(Rect bounds)
        {
            foreach (var row in this.Children)
                (row as IView).Arrange(new Rect(0, (row as Row)!.RowIndex * 50, TotalWidth, 50));

            return new Size(TotalWidth, TotalHeight);
        }
    }

    internal class Cell : ContentView
    {
        internal int CellIndex;
        internal Rect ClipRect;
        public Cell()
        {
            this.Content = new Label() ;
        }

        protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
        {
            (this.Content as IView).Measure(widthConstraint, heightConstraint);
            return base.MeasureOverride(widthConstraint, heightConstraint);
        }

        protected override Size ArrangeOverride(Rect bounds)
        {
            (this.Content as IView).Arrange(bounds);
            return base.ArrangeOverride(bounds);
        }

    }

    internal class Row : CustomLayout
    {
        protected override ILayoutManager CreateLayoutManager()
        {
            return new CustomLayoutManager(this);
        }
        internal int RowIndex;
        public CustomGrid? customGrid;


        internal override Size MeasureChildren(double widthConstraint, double heightConstraint)
        {
            if (this.Children.Count == 0)
                for (int i = 0; i < 100; i++)
                    this.Children.Add(new Cell() { CellIndex = i, BackgroundColor = i % 2 == 0 ? Colors.Red : Colors.Blue });
            foreach (var column in this.Children)
                column.Measure(120, 50);

            return new Size(widthConstraint, heightConstraint);
        }

        internal override Size ArrangeChildren(Rect bounds)
        {
            foreach (var column in this.Children)
            {
                // Commenting this clipping logic makes scrolling smoother.
                SetCellClip((column as Cell)!);
                (column as IView)!.Arrange(new Rect((column as Cell)!.CellIndex * 120, 0, 120, 50));

            }

            return new SizeRequest(new Size(bounds.Width, bounds.Height));
        }

        internal void SetCellClip(Cell cell)
        {
            if (cell.CellIndex < 0)
                return;

            double xPosition = this.customGrid!.ScrollX;
            int viewStartIndex = (int)(xPosition / 120);
            int viewEndIndex = (int)((xPosition + 500) / 120);

            if (cell.CellIndex == viewStartIndex)
            {
                var xPos = (xPosition % 120);
                cell.ClipRect = new Rect(xPos, 0, 120-xPos, 50);
            }
            else if (cell.CellIndex == viewEndIndex)
            {
                var width = ((xPosition + 500) % 120);
                cell.ClipRect = new Rect(0, 0, width, 50);
            }
            else
            {
                cell.ClipRect = Rect.Zero;
            }

            if (!cell.ClipRect.IsEmpty)
                cell.Clip = new RectangleGeometry(cell.ClipRect);
            else
                cell.Clip = null;
        }
    }

    public abstract class CustomLayout : Layout
    {
        internal abstract Size ArrangeChildren(Rect bounds);

        internal abstract Size MeasureChildren(double widthConstraint, double heightConstraint);
    }

    internal class CustomLayoutManager : LayoutManager
    {
        CustomLayout layout;
        internal CustomLayoutManager(CustomLayout layout) : base(layout)
        {
            this.layout = layout;
        }

        public override Size ArrangeChildren(Rect bounds) => this.layout.ArrangeChildren(bounds);

        public override Size Measure(double widthConstraint, double heightConstraint) => this.layout.MeasureChildren(widthConstraint, heightConstraint);
    }

}
