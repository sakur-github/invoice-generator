using InvoiceGenerator.Models.Data;
using InvoiceGeneratorTests.Utilities;

namespace InvoiceGeneratorTests
{
    [TestClass]
    public class ExportedTimesTests
    {
        [TestMethod]
        public void LoadFromString()
        {
            string? csv = FileProvider.ReadString(DataFileNames.ClockifyExport);

            Assert.IsNotNull(csv, $"Error when reading the testfile {DataFileNames.ClockifyExport}");

            TimeExport timeExport = TimeExport.FromCsv(csv);

            Assert.IsNotNull(timeExport);
            Assert.IsNotNull(timeExport.CollapsedTimes);
            Assert.AreEqual(3, timeExport.CollapsedTimes.Count);
            
            Time firstTime = timeExport.CollapsedTimes[0];
            Assert.AreEqual("Adam Tovatt", firstTime.Name);
            Assert.AreEqual("Careless", firstTime.Project);
            Assert.IsNull(firstTime.Client);
            Assert.AreEqual(66, firstTime.Amount);

            Time secondTime = timeExport.CollapsedTimes[1];
            Assert.AreEqual("Ludvig Svedberg", secondTime.Name);
            Assert.AreEqual("Careless", secondTime.Project);
            Assert.IsNull(secondTime.Client);
            Assert.AreEqual(10, secondTime.Amount);

            Time thirdTime = timeExport.CollapsedTimes[2];
            Assert.AreEqual("Oliver Levay", thirdTime.Name);
            Assert.AreEqual("Careless", thirdTime.Project);
            Assert.IsNull(thirdTime.Client);
            Assert.AreEqual(31, thirdTime.Amount);
        }

        [TestMethod]
        public void LoadFromString2()
        {
            string? csv = FileProvider.ReadString(DataFileNames.ClockifyExport2);

            Assert.IsNotNull(csv, $"Error when reading the testfile {DataFileNames.ClockifyExport2}");

            TimeExport timeExport = TimeExport.FromCsv(csv);

            Assert.IsNotNull(timeExport);
            Assert.IsNotNull(timeExport.CollapsedTimes);
            Assert.AreEqual(1, timeExport.CollapsedTimes.Count);
            Assert.IsNotNull(timeExport.Times);

            Time firstTime = timeExport.CollapsedTimes[0];
            Assert.AreEqual("Adam Tovatt", firstTime.Name);
            Assert.AreEqual("Retriva", firstTime.Project);
            Assert.IsNull(firstTime.Client);
            Assert.AreEqual(4, firstTime.Amount);

            Assert.IsTrue(timeExport.Times.Count > timeExport.CollapsedTimes.Count);

            Assert.IsTrue(timeExport.Times.First().Description != null);
        }
    }
}