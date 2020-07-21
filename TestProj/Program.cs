using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQS.Metadata;

namespace TestProj
{
    class Program
    {
        [STAThread()]
        static void Main(string[] args)
        {
            List<double> bitola = new List<double>();
            bitola.Add(4.0);
            bitola.Add(6.3);
            bitola.Add(8.0);
            bitola.Add(10);

            Calculator calculator = new Calculator() { bw = 14, h = 60, bitolas = bitola };
            MetadataApi.ShowWindow(calculator);
        }
    }
}
