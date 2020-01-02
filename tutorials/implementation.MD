## Creating a Graphic Primitive
- Create a new C# class library (.net Framework) project. Chose the .net framework version depending on your 800xA version. You can also use higher Versions, but in this case they have to be installed on the target system.
    - 800xA V5.1 uses .net 3.5
    - 800xA 6.0/6.1 uses .net 4.5
- Get the the public strong name signature key. You can get it from the example project, or extract it from some other graphic primitive assembly. For extraction we are using the dotnet strong name tool. More informations here: https://msdn.microsoft.com/en-us/library/k5b5tt23%28VS.80%29.aspx
This is the complete command:
'sn -e "c:\Program Files (x86)\ABB Industrial IT\Operate IT\Process Portal A\bin\GraphicPrimitives\ABB.xA.ProcessGraphics.AspectViewPrimitives.dll" "PG2Key.snk"
- Add the signature key to your project.
    - go to the project properties... signing
    - check "Sign the assembly", browse for the PG2Key.snk, check "Delay Sign only" too.
- Add a reference to the **GraphicsKernel.dll** and **AfwExpressionHandling.dll**, you can usually find this files in **c:\Program Files (x86)\ABB Industrial IT\Operate IT\Process Portal A\bin\**.
- Implement the Class
At first some required imports
'using AfwDynamicGraphics;
'using AfwDynamicGraphics.Media;
'using AfwExpressionHandling;
'using System.Globalization;
'using System.Windows;
'using System.Windows.Media;
'
'namespace ElementTutorial
'{
'    [PrimitiveItemAttribute("{F2280FE4-4DFF-4c77-92E9-2AB593C4ED07}", "ABB:FirstElement", "FirstElement", "Tutorial Elements", "My first graphic primitive")]
This defines where you can find your Graphic Primitive in Graphic Builder
1. Is a random GUID. You should generate a new one.
2. Is the internal name.
3. Is the visible name
4. Is the category name
5. Is the description
you can also add an icon resource name as parameter, but I have not get this working until now.

'    public class MyfirstElement : FrameItem, ITimerUpdateable
Our example primary inherits from **FrameItem**. This is the Template for visible rectangular items.
You can also use a different one, just take a look into the GraphicsKernel.dll interface.
The example also implements the **ITimerUpdateable** interface for the animation effect.
'    {
'        // additional properties
'        private static readonly PropertyDesc[] myprops = new PropertyDesc[2]
'        {
'            new PropertyDesc("Text", StringType.Singleton, 17, "Description Text", "Appearance"),
'            new PropertyDesc("Speed", IntegerType.Singleton, 18, "Animation speed", "Appearence")
'        };
This is the definition of PG2 properties, additional to the inherited FrameItem ones.
1. Is the name of the property
2. Is the datatype of the property. You can take a look into the AfwExpressionHandling.dll interface for valid types.
3. Is the index number of this property. FrameItem contains 16 properties, so the first additional is 17. Take care of this index numbers!
4. Is the description
5. Is the property category
'        // all properties copyed together
'        private static readonly PropertyDesc[] allprops = new PropertyDesc[MyfirstElement.myprops.Length + FrameItem.FiGetNumberOfProps(true, true, true, true, false)];
'
'        // instance variables
'        private IElementView view;
'        private CBrush textBrush = new CSolidBrush(0, 0, 0);
'        private LogicalFont font = new LogicalFont("Tahoma", 13.0, AfwDynamicGraphics.Media.FontStyle.Regular);
'        private DrawingVisual visual;
'        private string text = "FirstElement";
'        private long speed = 1;
'        private long pos = 0;
'        private bool dir = false;
'
'        // static constructor
'        static MyfirstElement()
'        {
'            int n = 0;
'            FrameItem.FiFillInPropertyDescriptions(allprops, ref n, true, true, true, true, false);
'            foreach (PropertyDesc propertyDesc in myprops)
'            {
'                allprops[n++] = propertyDesc;
'            }
'        }
This static constructor just copyes the inherited and the additional property definition together

'        // another one constructor
'        public MyfirstElement()
'            : base(true, true, true)
'        {
'        }
This constructor is required for satisfying the FrameItem templating

'        // constructor for runtime instance
'        public MyfirstElement(MyfirstElement other, GraphicItemVisual otherVisual)
'            : base(other, otherVisual)
'        {
'            textBrush = other.textBrush;
'            text = other.text;
'            font = other.font;
'            speed = other.speed;
'        }
This constructor clones an other item instance. This is the most used one. Take care of copying all required variables!

'        // returns a new instance
'        public override GraphicItem GetRunTimeInstance(IElementView elementView, GraphicItemVisual visual)
'        {
'            return new MyfirstElement(this, visual);
'        }
This is the interface function used for cloning

'        protected override Size GetDefaultSize()
'        {
'            return new Size(200.0, 50.0);
'        }
This function defines the default size

'        protected override PropertyDesc[] GetPropertyDescriptions()
'        {
'            return allprops;
'        }
This function returns the property definitions we copied together in the static constructor

'        // called if property is changed
'        protected override void TransferValue(IDataAccess accessor, int accessIndex, int propertyIndex, bool writeOperation)
'        {
'            switch (propertyIndex)
'            {
'                case 17:    // property "Text"
'                    accessor.TransferString(writeOperation, accessIndex, ref text);
'                    break;
'                case 18:    // property "Speed"
'                    accessor.TransferInteger(writeOperation, accessIndex, ref speed);
'                    break;
'                default:    // anything inherited
'                    base.TransferValue(accessor, accessIndex, propertyIndex, writeOperation);
'                    break;
'            }
'        }
This one is called if a property is changed dynamically, or a property is accessed by Graphics Builder. Constant properties are set by the cloning constructor.

'        protected override void InitVisual(IElementView elementView, ulong noValueEffects)
'        {
'            this.view = elementView;
'            visual = new GraphicItemVisual(this);
'            ItemDrawingVisual.Children.Add(visual);
'            DrawItem();
'        }
This is called for graphic initialisation

'        protected override void UpdateVisual(IElementView elementView, ulong updateReason, ulong noValueEffects)
'        {
'            DrawItem();
'        }
This is called if the control needs redraw

'        public void OnTimerUpdate(IElementView elementView)
'        {
'            pos = (dir ? pos + speed : pos - speed);
'            if (pos < 0)
'            {
'                pos = 0;
'                dir = true;
'            }
'            if (pos > 300)
'            {
'                pos = 300;
'                dir = false;
'            }
'            DrawItem();
'        }
This is the implementation of ITimerUpdateable. It is called about 60 times per second. Doing a full redraw such often results in high CPU load, so this is not a good example.

'        private void DrawItem()
'        {
'            using (DrawingContext drawingContext = visual.RenderOpen())
'            {
'                System.Windows.Media.Pen pen = new System.Windows.Media.Pen(Brushes.Black, 1);
'                drawingContext.DrawRectangle(Brushes.Green, pen, ClientArea);
'                double xd = ClientArea.Width * pos / 1000;
'                double yd = ClientArea.Height * pos / 1000;
'                drawingContext.DrawRectangle(Brushes.Gray, pen, new Rect(ClientArea.X+xd,ClientArea.Y+yd,ClientArea.Width-(xd*2),ClientArea.Height-(yd*2)));
'
'                Brush brush = textBrush.GetBrush(view);
'                WPFFont font = this.font.GetFont(view);
'                FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, font.Typeface, font.Size, brush);
'                drawingContext.DrawText(formattedText, new Point(ClientArea.Left+10, ClientArea.Top+10));
'            }
'        }
'    }
'}
Finally the drawing of the item. 
