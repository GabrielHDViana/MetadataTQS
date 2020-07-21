using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using TQS.Drawing2D;
using TQS.Drawing2D.Creation;
using TQS.Metadata;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        public object data;
        Grid g;
        int rowindex;
        PropertyInfo[] PropInfo;
        public MainWindow(object data)
        {
            this.data = data;
            InitializeComponent();
            Update();
        }
        private void WindowAttribute(Grid g)
        {
            root.Children.Add(g);
            g.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 300, Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            //Title
            var titleheader = data.GetType().GetCustomAttribute<HeaderAttribute>();
            if (titleheader != null) { Title = titleheader.Header; }
        }
        private void clear()
        {
            root.Children.Clear();
        }
        private void GetProperties(object data)
        {
            PropInfo = data.GetType().GetProperties();
        }
        private void BuildTQSDrawable(int rowindex, object propvalue)
        {
            var drawingComponent = DrawableComponentCreator.CreateDrawableComponent(DrawableComponentTypes.TQSJan);
            WindowsFormsHost host = new WindowsFormsHost();
            host.Height = 300;
            host.Child = drawingComponent;
            Grid.SetRow(host, rowindex++);
            Grid.SetColumnSpan(host, 2);
            g.Children.Add(host);

            DrawableBasic drawableBasic = propvalue as DrawableBasic;
            drawingComponent.IDrawable.Add(drawableBasic.GetDrawingElements().ToArray());
            host.Loaded += (o, e) => { drawingComponent.ZoomExtents(); drawingComponent.ZoomOut(); };
        }
        private bool IsGroup(int _rowindex)
        {
            return PropInfo[_rowindex].GetCustomAttribute<GroupAttribute>() == null ? false : true;
        }
        private static bool VerifyList(object o)
        {
            if (o == null)
            {
                return false;
            }
            else
            {
                return o is IList &&
                       o.GetType().IsGenericType &&
                       o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
            }
        }
        private TextBlock MakeTextBlock(PropertyInfo property, int GridRowIndex = 0)
        {
            var headerattribute = property.GetCustomAttribute<HeaderAttribute>();
            string varname = headerattribute == null ? property.Name : headerattribute.Header;
            var helpattibute = property.GetCustomAttribute<HelpAttribute>();
            TextBlock tb = new TextBlock() { Text = varname };
            if (helpattibute != null) { tb.ToolTip = helpattibute.Help; }
            return tb;
        }
        private TextBox MakeTextBox(PropertyInfo property)
        {
            string varvalue = property.GetValue(data).ToString();
            TextBox tb = new TextBox() { Text = varvalue };
            tb.IsEnabled = property.GetCustomAttribute<ReadOnlyAttribute>() == null;
            return tb;
        }
        private void GridAddRowAuto(Grid g)
        {
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
        }
        private void BuildGroup()
        {
            //Creating groupboxes
            Grid TmpGrid = new Grid();
            TmpGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            TmpGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
            TmpGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            int GridRowIndex = 0;

            while (IsGroup(rowindex))
            {
                TmpGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto)});
                var property = PropInfo[rowindex];
                object propvalue = property.GetValue(data);
                {
                    TextBlock tb = MakeTextBlock(property);
                    Grid.SetRow(tb, rowindex);
                    TmpGrid.Children.Add(tb);
                }
                {
                    TextBox tb = MakeTextBox(property);
                    Grid.SetColumn(tb, 1);
                    Grid.SetRow(tb, rowindex++);
                    tb.TextChanged += (o, e) =>
                    {
                        property.SetValue(data, Convert.ChangeType(tb.Text, property.GetValue(data).GetType()));
                        Update();
                    };
                    TmpGrid.Children.Add(tb);
                }
            }

            GroupBox GrpBox = new GroupBox()
            {
                Header = "Header",
                Content = TmpGrid
            };

            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.SetColumnSpan(GrpBox, 2);
            g.Children.Add(GrpBox);

        }
        private void Update()
        {
            clear();
            g = new Grid();
            WindowAttribute(g);
            GetProperties(data);

            rowindex = 0;

            for (int i = 0; i < PropInfo.Length; i++)
            {
                GridAddRowAuto(g);
                var property = PropInfo[i];
                object propvalue = property.GetValue(data);
                bool isList = VerifyList(propvalue);

                if (propvalue is DrawableBasic)
                    BuildTQSDrawable(rowindex, propvalue);
                else if (isList == true)
                {
                    var headerattribute = property.GetCustomAttribute<HeaderAttribute>();
                    var helpattibute = property.GetCustomAttribute<HelpAttribute>();
                    var readOnlyattribute = property.GetCustomAttribute<ReadOnlyAttribute>();
                    string varname = headerattribute == null ? property.Name : headerattribute.Header;

                    {
                        TextBlock tb = new TextBlock() { Text = varname };
                        if (helpattibute != null) { tb.ToolTip = helpattibute.Help; }
                        Grid.SetRow(tb, rowindex);
                        g.Children.Add(tb);
                    }

                    {
                        ComboBox cb = new ComboBox();
                        cb.ItemsSource = (IList)propvalue;
                        cb.SelectedIndex = 0; // VERIFICAR
                        Grid.SetColumn(cb, 1);
                        Grid.SetRow(cb, rowindex++);
                        g.Children.Add(cb);
                    }
                }
                else
                {
                    if (IsGroup(rowindex))
                    {
                        BuildGroup();
                    }
                }
            }

            var methods = data.GetType().GetMethods();
            for (int i = 0; i < methods.Length; i++)
            {
                GridAddRowAuto(g);
                var method = methods[i];
                var headerattribute = method.GetCustomAttribute<HeaderAttribute>();
                var helpattibute = method.GetCustomAttribute<HelpAttribute>();
                if (headerattribute == null) { continue; }
                Button button = new Button() { Content = headerattribute.Header };
                if (helpattibute != null) { button.ToolTip = helpattibute.Help; }

                button.Click += (o, e) => { method.Invoke(data, null); Update(); };

                Grid.SetRow(button, rowindex++);
                Grid.SetColumnSpan(button, 2);
                g.Children.Add(button);
            }

        }
    }


}
