using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpTest
{
    [TestClass]
    public class WorkDayCalculatorTests
    {

        [TestMethod]
        public void TestNoWeekEnd()
        {
            DateTime startDate = new DateTime(2021, 12, 1);
            int count = 10;

            DateTime result = new WorkDayCalculator().Calculate(startDate, count, null);

            Assert.AreEqual(startDate.AddDays(count-1), result);
        }

        [TestMethod]
        public void TestNormalPath()
        {
            DateTime startDate = new DateTime(2021, 4, 21);
            int count = 5;
            WeekEnd[] weekends = new WeekEnd[1]
            {
                new WeekEnd(new DateTime(2021, 4, 23), new DateTime(2021, 4, 25))
            }; 

            DateTime result = new WorkDayCalculator().Calculate(startDate, count, weekends);

            Assert.IsTrue(result.Equals(new DateTime(2021, 4, 28)));
        }

        [TestMethod]
        public void TestWeekendAfterEnd()
        {
            DateTime startDate = new DateTime(2021, 4, 21);
            int count = 5;
            WeekEnd[] weekends = new WeekEnd[2]
            {
                new WeekEnd(new DateTime(2021, 4, 23), new DateTime(2021, 4, 25)),
                new WeekEnd(new DateTime(2021, 4, 29), new DateTime(2021, 4, 29))
            };
            
            DateTime result = new WorkDayCalculator().Calculate(startDate, count, weekends);

            Assert.IsTrue(result.Equals(new DateTime(2021, 4, 28)));
        }

        [TestMethod]
        public void MyTestWeekendsAfter()
        {
            DateTime startDate = new DateTime(2021, 4, 21);
            int count = 3;
            WeekEnd[] weekends = new WeekEnd[3]
            {
                new WeekEnd(new DateTime(2021, 4, 24), new DateTime(2021, 4, 24)),
                new WeekEnd(new DateTime(2021, 4, 26), new DateTime(2021, 4, 26)),
                new WeekEnd(new DateTime(2021, 4, 29), new DateTime(2021, 4, 29))
            };
            
            DateTime result = new WorkDayCalculator().Calculate(startDate, count, weekends);

            Assert.IsTrue(result.Equals(new DateTime(2021, 4, 23)));
        }

        [TestMethod]
        public void MyTestWeekendsAfterNormal()
        {
            DateTime startDate = new DateTime(2021, 4, 21);
            int count = 5;
            WeekEnd[] weekends = new WeekEnd[4]
            {
                new WeekEnd(new DateTime(2021, 4, 22), new DateTime(2021, 4, 23)),
                new WeekEnd(new DateTime(2021, 4, 25), new DateTime(2021, 4, 25)),
                new WeekEnd(new DateTime(2021, 4, 29), new DateTime(2021, 4, 29)),
                new WeekEnd(new DateTime(2021, 4, 30), new DateTime(2021, 4, 30))
            };
            
            DateTime result = new WorkDayCalculator().Calculate(startDate, count, weekends);

            Assert.IsTrue(result.Equals(new DateTime(2021, 4, 28)));
        }

        [TestMethod]
        public void MyTestWeekendsBefor()
        {
            DateTime startDate = new DateTime(2021, 4, 21);
            int count = 5;
            WeekEnd[] weekends = new WeekEnd[3]
            {
                new WeekEnd(new DateTime(2021, 4, 10), new DateTime(2021, 4, 10)),
                new WeekEnd(new DateTime(2021, 4, 13), new DateTime(2021, 4, 18)),
                new WeekEnd(new DateTime(2021, 4, 20), new DateTime(2021, 4, 20))
            };
            
            DateTime result = new WorkDayCalculator().Calculate(startDate, count, weekends);

            Assert.IsTrue(result.Equals(new DateTime(2021, 4, 25)));
        }

        [TestMethod]
        public void MyTestWeekendAroundStart()
        {
            DateTime startDate = new DateTime(2021, 4, 21);
            int count = 5;
            WeekEnd[] weekends = new WeekEnd[3]
            {
                new WeekEnd(new DateTime(2021, 4, 10), new DateTime(2021, 4, 15)),
                new WeekEnd(new DateTime(2021, 4, 19), new DateTime(2021, 4, 22)),
                new WeekEnd(new DateTime(2021, 4, 25), new DateTime(2021, 4, 25))
            };

            DateTime result = new WorkDayCalculator().Calculate(startDate, count, weekends);

            Assert.IsTrue(result.Equals(new DateTime(2021, 4, 28)));
        }

        [TestMethod]
        public void MyTestWeekendsBeforNormal()
        {
            DateTime startDate = new DateTime(2021, 4, 21);
            int count = 5;
            WeekEnd[] weekends = new WeekEnd[4]
            {
                new WeekEnd(new DateTime(2021, 4, 10), new DateTime(2021, 4, 15)),
                new WeekEnd(new DateTime(2021, 4, 20), new DateTime(2021, 4, 20)),
                new WeekEnd(new DateTime(2021, 4, 23), new DateTime(2021, 4, 24)),
                new WeekEnd(new DateTime(2021, 4, 26), new DateTime(2021, 4, 26))
            };

            DateTime result = new WorkDayCalculator().Calculate(startDate, count, weekends);

            Assert.IsTrue(result.Equals(new DateTime(2021, 4, 28)));
        }

        [TestMethod]
        public void MyTestAllWeekends()
        {
            DateTime startDate = new DateTime(2021, 4, 21);
            int count = 5;
            WeekEnd[] weekends = new WeekEnd[5]
            {
                new WeekEnd(new DateTime(2021, 4, 10), new DateTime(2021, 4, 15)),
                new WeekEnd(new DateTime(2021, 4, 20), new DateTime(2021, 4, 22)),
                new WeekEnd(new DateTime(2021, 4, 25), new DateTime(2021, 4, 25)),
                new WeekEnd(new DateTime(2021, 4, 28), new DateTime(2021, 4, 28)),
                new WeekEnd(new DateTime(2021, 4, 30), new DateTime(2021, 4, 30))
            };

            DateTime result = new WorkDayCalculator().Calculate(startDate, count, weekends);

            Assert.IsTrue(result.Equals(new DateTime(2021, 4, 29)));
        }

        [TestMethod]
        public void MyTestOneDay()
        {
            DateTime startDate = new DateTime(2021, 4, 21);
            int count = 1;
            WeekEnd[] weekends = new WeekEnd[3]
            {
                new WeekEnd(new DateTime(2021, 4, 19), new DateTime(2021, 4, 20)),
                new WeekEnd(new DateTime(2021, 4, 21), new DateTime(2021, 4, 21)),
                new WeekEnd(new DateTime(2021, 4, 26), new DateTime(2021, 4, 27))
            };

            DateTime result = new WorkDayCalculator().Calculate(startDate, count, weekends);

            Assert.IsTrue(result.Equals(new DateTime(2021, 4, 22)));
        }
    }
}
