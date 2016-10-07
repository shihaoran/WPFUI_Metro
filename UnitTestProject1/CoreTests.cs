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
            catch (Exception e)
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
            Station s = core.StaCollection["2号航站楼"];
            Tuple<int, List<string>> t = core.FindNearestTransSta(s);
            Assert.AreEqual(1, t.Item1);
        }


        [TestMethod()]
        public void BuildGragphTest()
        {
            Core core = new Core();
            try
            {
                core.ReadData();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            try
            {
                core.BuildGragph("", "", "");
            }
            catch (KeyNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
                return;
            }
            int i;
            i = Core.graph[1, 2];
            Assert.AreEqual(2, i);
        }

        [TestMethod()]
        public void FindNearestTransStaTest1()
        {
            Core core = new Core();
            try
            {
                core.ReadData();
            }
            catch (Exception e)
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
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            Tuple<string, int> a;
            try
            {
                a = core.DijkstraPath("沙河", "南锣鼓巷");
            }
            catch (IndexOutOfRangeException e)
            {
                System.Console.WriteLine(e.Message);
                return;
            }

            Assert.AreEqual(3, a.Item2);
        }

        [TestMethod()]
        public void BFSPathTest()
        {
            Core core = new Core();
            core.ReadData();
            Tuple<string, int> a = core.BFSPath("沙河", "土桥");

            Assert.AreEqual(34, a.Item2);
        }

        [TestMethod()]
        public void MakePathSectionTest()
        {
            Core core = new Core();
            core.ReadData();
            Station a = null;
            Station b = null;
            Station c = null;
            PathSection d = null;

            core.StaCollection.TryGetValue("西直门", out a);
            core.StaCollection.TryGetValue("北京站", out b);
            core.StaCollection.TryGetValue("大钟寺", out c);

            d = core.MakePathSection(a, b);
            Assert.AreEqual("2号线", d.LineName);
            Assert.AreEqual(9, d.list.Count);

            d = core.MakePathSection(a, c);
            Assert.AreEqual("13号线", d.LineName);
            Assert.AreEqual(1, d.list.Count);




        }

        [TestMethod()]
        public void FindLinePathTest()
        {
            Core core = new Core();
            core.ReadData();
            Station a = null;
            Station b = null;
            Station c = null;
            Line l = null;
            PathSection d = null;

            core.StaCollection.TryGetValue("西直门", out a);
            core.StaCollection.TryGetValue("北京站", out b);
            core.StaCollection.TryGetValue("大钟寺", out c);
            core.LineCollection.TryGetValue("10号线", out l);

            try
            {
                d = core.FindLinePath(a, b, l);
            }
            catch (KeyNotFoundException e)
            {
                System.Console.WriteLine(e.Message);
            }

        }

        [TestMethod()]
        public void isSameLineTest()
        {
            Core core = new Core();
            core.ReadData();

            Station a = null;
            Station b = null;
            Station c = null;
            Line l = null;

            core.StaCollection.TryGetValue("西直门", out a);
            core.StaCollection.TryGetValue("北京站", out b);
            core.StaCollection.TryGetValue("大钟寺", out c);

            l = core.isSameLine(a, b);
            Assert.AreEqual("2号线", l.LineName);

            l = core.isSameLine(b, c);
            Assert.AreEqual(null, l);

            l = core.isSameLine(a, c);
            Assert.AreEqual("13号线", l.LineName);
        }

        [TestMethod()]
        public void GetLinkedStationsTest()
        {
            Core core = new Core();
            core.ReadData();
            Station a = null;


            core.StaCollection.TryGetValue("3号航站楼", out a);
            List<Station> list = null;
          
            list = core.GetLinkedStations(a);
            Assert.AreEqual(2,list.Count);
        }

        [TestMethod()]
        public void GetShortestLinkedStationsTest()
        {
            Core core = new Core();
            core.ReadData();
            Station a = null;

            core.StaCollection.TryGetValue("五道口", out a);
            List<Station> list = core.GetShortestLinkedStations(a);


            Assert.AreEqual("知春路",list.First().StationName);
        }
    }
}