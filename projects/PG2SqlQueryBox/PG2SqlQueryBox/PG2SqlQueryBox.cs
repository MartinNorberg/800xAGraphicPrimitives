namespace PG2SqlQueryBox
{
    using System;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using AfwDynamicGraphics;
    using AfwDynamicGraphics.Media;
    using AfwExpressionHandling;

    [PrimitiveItemAttribute("{1DBBCE9D-8030-4D10-A323-95ED488DBB90}", "Custom:FirstElement", "SqlQueryBox", "Custom Controls", "Displays result of sql query and writes to property")]
    public class PG2SqlQueryBox : FrameItem, ITimerUpdateable
    {
        private protected VariantValue source;
        private protected DataEntityIndex target;
        private static readonly PropertyDesc[] Properties = new PropertyDesc[6]
        {
            new PropertyDesc("ConnectionString", StringType.Singleton, 17, "Connectionstring to database", "Appearance"),
            new PropertyDesc("Query", StringType.Singleton, 18, "Sql query", "Appearance"),
            new PropertyDesc("TimeStamp", BooleanType.Singleton, 19, "Show timestamp", "Appearance"),
            new PropertyDesc("updatetime", IntegerType.Singleton, 20, "in seconds, 0 if not used", "Appearance"),
            new PropertyDesc("Target", PropertyRefType.Singleton, 21, "To be connected to plcTag", "Appearance"),
            new PropertyDesc("Trigger", BooleanType.Singleton, 22, "Trigg", "Appearance"),
        };

        private static readonly PropertyDesc[] Allprops = new PropertyDesc[PG2SqlQueryBox.Properties.Length + FrameItem.FiGetNumberOfProps(true, true, true, true, false)];
        private ICommand triggCommand;
        private IElementView elementView;
        private CBrush textBrush = new CSolidBrush(0, 0, 0);
        private LogicalFont font = new LogicalFont("Tahoma", 13.0, AfwDynamicGraphics.Media.FontStyle.Regular);
        private DrawingVisual visual;
        private string connectionString = string.Empty;
        private string query = string.Empty;
        private bool showTimestamp = false;
        private long updateTime = 10;
        private bool oldTriggerValue = false;
        private bool trigger = false;
        private bool hasMouseFocus = false;
        private string response = string.Empty;

        private DateTime lastUpdate = DateTime.Now.AddHours(-1);

        static PG2SqlQueryBox()
        {
            var n = 0;
            FrameItem.FiFillInPropertyDescriptions(Allprops, ref n, true, true, true, true, false);
            foreach (PropertyDesc propertyDesc in Properties)
            {
                Allprops[n++] = propertyDesc;
            }
        }

        public PG2SqlQueryBox(PG2SqlQueryBox ai, GraphicItemVisual visual)
            : base(ai, visual)
        {
            this.triggCommand = new RelayCommand(_ => this.ExecuteQuery());

            this.textBrush = ai.textBrush;
            this.connectionString = ai.connectionString;
            this.query = ai.query;
            this.showTimestamp = ai.showTimestamp;
            this.target = ai.target;
            this.trigger = ai.trigger;
            this.updateTime = ai.updateTime;
        }

        public PG2SqlQueryBox()
            : base(true, true, true)
        {
        }

        public override void Enter(MouseEventContext mouseEventContext, IElementView elementView)
        {
            this.hasMouseFocus = true;
            base.Enter(mouseEventContext, elementView);
        }

        public override void Leave(MouseEventContext mouseEventContext, IElementView elementView)
        {
            this.hasMouseFocus = false;
            base.Leave(mouseEventContext, elementView);
        }

        public override GraphicItem GetRunTimeInstance(IElementView elementView, GraphicItemVisual visual)
        {
            return new PG2SqlQueryBox(this, visual);
        }

        public void OnTimerUpdate(IElementView elementView)
        {
            if (this.updateTime != 0)
            {
                this.ExecuteCyclic();
            }

            this.DrawItem(this.response);
        }

        protected override Size GetDefaultSize()
        {
            return new Size(200.0, 50.0);
        }

        protected override PropertyDesc[] GetPropertyDescriptions()
        {
            return Allprops;
        }

        protected override void OnFiStateVarChanged()
        {
            if (this.elementView == null)
            {
                return;
            }

            this.StateVariablesChanged(this.elementView);
        }

        protected override void TransferValue(IDataAccess accessor, int accessIndex, int propertyIndex, bool writeOperation)
        {
            switch (propertyIndex)
            {
                case 17: // property "connectionString"
                    accessor.TransferString(writeOperation, accessIndex, ref this.connectionString);
                    break;
                case 18: // property "Query"
                    accessor.TransferString(writeOperation, accessIndex, ref this.query);
                    break;
                case 19: // property "showTimeStamp"
                    accessor.TransferBoolean(writeOperation, accessIndex, ref this.showTimestamp);
                    break;
                case 20:
                    accessor.TransferInteger(writeOperation, accessIndex, ref this.updateTime);
                    break;
                case 21:
                    accessor.TransferDataEntityIndex(writeOperation, accessIndex, ref this.target);
                    break;
                case 22:
                    accessor.TransferBoolean(writeOperation, accessIndex, ref this.trigger);
                    if (this.trigger)
                    {
                        if (this.oldTriggerValue)
                        {
                            break;
                        }
                        else
                        {
                            this.oldTriggerValue = this.trigger;
                            this.triggCommand.Execute(null);
                        }
                    }
                    else
                    {
                        this.oldTriggerValue = this.trigger;
                    }

                    break;
                default: // anything inherited
                    base.TransferValue(accessor, accessIndex, propertyIndex, writeOperation);
                    break;
            }
        }

        protected override void InitVisual(IElementView elementView, ulong noValueEffects)
        {
            this.elementView = elementView;
            this.visual = new GraphicItemVisual(this);
            this.ItemDrawingVisual.Children.Add(this.visual);
            if (this.updateTime != 0)
            {
                this.ExecuteCyclic();
            }

            this.DrawItem(this.response);
        }

        protected override void UpdateVisual(IElementView elementView, ulong updateReason, ulong noValueEffects)
        {
            if (this.updateTime != 0)
            {
                this.ExecuteCyclic();
            }

            this.DrawItem(this.response);
        }

        private void DrawItem(string response)
        {
            using (DrawingContext drawingContext = this.visual.RenderOpen())
            {
                if (this.hasMouseFocus)
                {
                    this.DrawFrame(drawingContext, this.elementView, true);
                }

                System.Windows.Media.Pen pen = new System.Windows.Media.Pen(Brushes.Black, 1);
                drawingContext.DrawRectangle(this.fillColor.GetBrush(this.elementView), new System.Windows.Media.Pen(Brushes.Black, 1), this.ClientArea);
                Brush brush = this.textBrush.GetBrush(this.elementView);
                WPFFont font = this.font.GetFont(this.elementView);
                FormattedText formattedText = new FormattedText(response, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, font.Typeface, font.Size, brush);
                drawingContext.DrawText(formattedText, new Point(this.ClientArea.Left + 10, this.ClientArea.Top + 10));
            }
        }

        private void ExecuteCyclic()
        {
            var diffSeconds = (DateTime.Now - this.lastUpdate).TotalSeconds;
            if (diffSeconds >= this.updateTime)
            {
                this.ExecuteQuery();
            }
        }

        private void ExecuteQuery()
        {
            if (this.elementView.Viewer is EditPanel)
            {
                return;
            }
            else
            {
                try
                {
                    using (var con = new SqlConnection(this.connectionString))
                    {
                        using (var cmd = new SqlCommand(this.query, con))
                        {
                            con.Open();
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.CommandText = this.query;
                            this.response = cmd.ExecuteScalar().ToString();
                            this.elementView.WriteProperty(VariantValue.FromObject(this.response), this.target);

                            if (this.showTimestamp)
                            {
                                this.response += $" {DateTime.Now}";
                            }

                            this.lastUpdate = DateTime.Now;
                        }
                    }
                }
                catch (Exception e)
                {
                    this.response = e.Message;
                }
            }
        }
    }
}
