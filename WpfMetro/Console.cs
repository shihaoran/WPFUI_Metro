using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMetro
{
    /// <summary>
    /// 程序入口
    /// </summary>
    static class Console
    {
        [STAThread]
        public static void Main(string[] args)
        {
            //
            if (args.Length == 1 && args[0] == "-g")
            {
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
            return;
        }
    }
}
