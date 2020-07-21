using System;
using WpfApp3;

namespace TQS.Metadata
{
    public class MetadataApi
    {
        public static void ShowWindow(object data)
        {
            MainWindow w = new MainWindow(data);
            w.ShowDialog();
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class HeaderAttribute : Attribute
    {
        public string Header { get; set; }
        public HeaderAttribute(string Header)
        {
            this.Header = Header;
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class HelpAttribute : Attribute
    {
        public string Help { get; set; }
        public HelpAttribute(string Help)
        {
            this.Help = Help;
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class ReadOnlyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class GroupAttribute : Attribute
    {
        public string Group { get; set; }

        public GroupAttribute(string group)
        {
            Group = group;
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class ConverterAttribute : Attribute
    {
        public string Category { get; set; }

        public ConverterAttribute(string category)
        {
            Category = category;
        }
    }
}
