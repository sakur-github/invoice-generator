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
            Assert.IsNotNull(timeExport.Times);
            Assert.AreEqual(3, timeExport.Times.Count);
            
            Time firstTime = timeExport.Times[0];
            Assert.AreEqual("Adam Tovatt", firstTime.Name);
            Assert.AreEqual("Careless", firstTime.Project);
            Assert.IsNull(firstTime.Client);
            Assert.AreEqual(66, firstTime.Amount);

            Time secondTime = timeExport.Times[1];
            Assert.AreEqual("Ludvig Svedberg", secondTime.Name);
            Assert.AreEqual("Careless", secondTime.Project);
            Assert.IsNull(secondTime.Client);
            Assert.AreEqual(10, secondTime.Amount);

            Time thirdTime = timeExport.Times[2];
            Assert.AreEqual("Oliver Levay", thirdTime.Name);
            Assert.AreEqual("Careless", thirdTime.Project);
            Assert.IsNull(thirdTime.Client);
            Assert.AreEqual(31, thirdTime.Amount);
        }
    }
}