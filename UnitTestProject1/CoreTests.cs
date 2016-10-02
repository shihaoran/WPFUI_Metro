using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfMetro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMetro.Tests
{
    [TestClass()]
    public class CoreTests
    {
        [TestMethod()]
        public void ReadDataTest()
        {
            Core core = new Core();
            try
            {
                core.ReadData();
            }
            catch (MapErrorException e)
            {
                System.Console.WriteLine(e.Message);
            }
            int c1 = core.StaCollection.Values.Count;
            int c2 = core.LineCollection.Values.Count;
            System.Console.WriteLine(c1 + " " + c2);
            Assert.AreEqual(276, c1);
            Assert.AreEqual(18, c2);
        }

        [TestMethod()]
        public void BuildGragphTest()
        {
            Core core = new Core();
            try
            {
                core.ReadData();
            }
            catch (MapErrorException e)
            {
                System.Console.WriteLine(e.Message);
            }
            core.BuildGragph("", "", "");
            //     int i = new Int32;
            //        i = graph[1, 2]; 
        }

        [TestMethod()]
        public void FindNearestTransStaTest()
        {
            Core core = new Core();
            try
            {
                core.ReadData();
            }
            catch (MapErrorException e)
            {
                System.Console.WriteLine(e.Message);
            }
            Station s = null;
            core.StaCollection.TryGetValue("知春路", out s);
            Tuple<int, List<string>> a = core.FindNearestTransSta(s);
            Assert.AreEqual(1, a.Item1);
            Assert.AreEqual("知春路", a.Item2.ElementAt(0));

            core.StaCollection.TryGetValue("安贞门", out s);
            a = core.FindNearestTransSta(s);
            Assert.AreEqual(2, a.Item1);
            Assert.AreEqual("北土城", a.Item2.ElementAt(0));
            Assert.AreEqual("惠新西街南口", a.Item2.ElementAt(1));

            core.StaCollection.TryGetValue("十里堡", out s);
            a = core.FindNearestTransSta(s);
            Assert.AreEqual(1, a.Item1);
            Assert.AreEqual("金台路", a.Item2.ElementAt(0));
        }

        [TestMethod()]
        public void DijkstraPathTest()
        {
            Core core = new Core();
            try
            {
                core.ReadData();
            }
            catch (MapErrorException e)
            {
                System.Console.WriteLine(e.Message);
            }

            Tuple<string,int> a =  core.DijkstraPath("沙河", "南锣鼓巷");

            Assert.AreEqual(3,a.Item2);
        }
    }
}