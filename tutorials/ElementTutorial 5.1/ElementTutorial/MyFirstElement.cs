using AfwDynamicGraphics;
using AfwDynamicGraphics.Media;
using AfwExpressionHandling;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace ElementTutorial
{
    [PrimitiveItemAttribute("{F2280FE4-4DFF-4c77-92E9-2AB593C4ED07}", "ABB:FirstElement", "FirstElement", "Tutorial Elements", "My first graphic primitive")]
    public class MyfirstElement : FrameItem, ITimerUpdateable
    {
        // additional properties
        private static readonly PropertyDesc[] myprops = new PropertyDesc[2]
        {
            new PropertyDesc("Text", StringType.Singleton, 17, "Description Text", "Appearance"),
            new PropertyDesc("Speed", IntegerType.Singleton, 18, "Animation speed", "Appearence")
        };
        // all properties
        private static readonly PropertyDesc[] allprops = new PropertyDesc[MyfirstElement.myprops.Length + FrameItem.FiGetNumberOfProps(true, true, true, true, false)];

        // instance variables
        private IElementView view;
        private CBrush textBrush = new CSolidBrush(0, 0, 0);
        private LogicalFont font = new LogicalFont("Tahoma", 13.0, AfwDynamicGraphics.Media.FontStyle.Regular);
        private DrawingVisual visual;
        private string text = "FirstElement";
        private long speed = 1;
        private long pos = 0;
        private bool dir = false;

        // static constructor
        static MyfirstElement()
        {
            int n = 0;
            FrameItem.FiFillInPropertyDescriptions(allprops, ref n, true, true, true, true, false);
            foreach (PropertyDesc propertyDesc in myprops)
            {
                allprops[n++] = propertyDesc;
            }
        }

        // another one constructor
        public MyfirstElement()
            : base(true, true, true)
        {
        }

        // constructor for runtime instance
        public MyfirstElement(MyfirstElement other, GraphicItemVisual otherVisual)
            : base(other, otherVisual)
        {
            textBrush = other.textBrush;
            text = other.text;
            font = other.font;
            speed = other.speed;
        }

        // returns a new instance
        public override GraphicItem GetRunTimeInstance(IElementView elementView, GraphicItemVisual visual)
        {
            return new MyfirstElement(this, visual);
        }

        protected override void OnFiStateVarChanged()
        {
            if (view == null) return;
            StateVariablesChanged(view);
        }

        protected override Size GetDefaultSize()
        {
            return new Size(200.0, 50.0);
        }

        protected override PropertyDesc[] GetPropertyDescriptions()
        {
            return allprops;
        }

        // called if property is changed
        protected override void TransferValue(IDataAccess accessor, int accessIndex, int propertyIndex, bool writeOperation)
        {
            switch (propertyIndex)
            {
                case 17:    // property "Text"
                    accessor.TransferString(writeOperation, accessIndex, ref text);
                    break;
                case 18:    // property "Speed"
                    accessor.TransferInteger(writeOperation, accessIndex, ref speed);
                    break;
                default:    // anything inherited
                    base.TransferValue(accessor, accessIndex, propertyIndex, writeOperation);
                    break;
            }
        }

        protected override void InitVisual(IElementView elementView, ulong noValueEffects)
        {
            this.view = elementView;
            visual = new GraphicItemVisual(this);
            ItemDrawingVisual.Children.Add(visual);
            DrawItem();
        }

        protected override void UpdateVisual(IElementView elementView, ulong updateReason, ulong noValueEffects)
        {
            DrawItem();
        }

        // called cyclically
        public void OnTimerUpdate(IElementView elementView)
        {
            pos = (dir ? pos + speed : pos - speed);
            if (pos < 0)
            {
                pos = 0;
                dir = true;
            }
            if (pos > 300)
            {
                pos = 300;
                dir = false;
            }
            DrawItem();
        }

        // draw all
        private void DrawItem()
        {
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                System.Windows.Media.Pen pen = new System.Windows.Media.Pen(Brushes.Black, 1);
                drawingContext.DrawRectangle(Brushes.Green, pen, ClientArea);
                double xd = ClientArea.Width * pos / 1000;
                double yd = ClientArea.Height * pos / 1000;
                drawingContext.DrawRectangle(Brushes.Gray, pen, new Rect(ClientArea.X+xd,ClientArea.Y+yd,ClientArea.Width-(xd*2),ClientArea.Height-(yd*2)));

                Brush brush = textBrush.GetBrush(view);
                WPFFont font = this.font.GetFont(view);
                FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, font.Typeface, font.Size, brush);
                drawingContext.DrawText(formattedText, new Point(ClientArea.Left+10, ClientArea.Top+10));
            }
        }

    }
}
