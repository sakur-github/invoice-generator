using InvoiceGenerator.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceGenerator.Models.Data
{
    public class TimeExport
    {
        public List<Time>? Times { get; set; }

        public static TimeExport FromCsv(string csv)
        {
            TimeExport exportedTimes = new TimeExport();
            List<Time> times = new List<Time>();
            string[] lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines.Skip(1))
            {
                string[] values = line.Split(',');

                Time time = new Time
                {
                    Name = values[0].GetUnescapedValue(),
                    Project = values[1].GetUnescapedValue(),
                    Client = values[2].GetUnescapedValue(),
                    Amount = (int)Math.Round(decimal.Parse(values[4].GetUnescapedValue(), CultureInfo.InvariantCulture))
                };

                if(time.Client == "(Without client)") // Clockify exports null as "(Without client)"
                    time.Client = null;

                times.Add(time);
            }

            exportedTimes.Times = times;

            return exportedTimes;
        }
    }
}
