using InvoiceGenerator.Helpers;
using InvoiceGenerator.Models.Configuration;
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

        private Dictionary<string, int> headerIndexes;

        public TimeExport(Dictionary<string, int> headerIndexes)
        {
            this.headerIndexes = headerIndexes;
        }

        public static TimeExport FromCsv(string csv)
        {
            List<Time> times = new List<Time>();
            string[] lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            TimeExport exportedTimes = new TimeExport(GetHeaderIndexes(lines[0]));

            foreach (string line in lines.Skip(1))
            {
                string[] values = line.Split(',');

                string? name = values[exportedTimes.GetHeaderIndex("User")].GetUnescapedValue();
                string? project = values[exportedTimes.GetHeaderIndex("Project")].GetUnescapedValue();
                string? client = values[exportedTimes.GetHeaderIndex("Client")].GetUnescapedValue();
                decimal amount = decimal.Parse(values[exportedTimes.GetHeaderIndex("Duration (decimal)", "Time (decimal)")].GetUnescapedValue(), CultureInfo.InvariantCulture);

                name = string.IsNullOrEmpty(name) ? null : name; // if the string is empty we make it null instead
                project = string.IsNullOrEmpty(project) ? null : project;
                client = string.IsNullOrEmpty(client) ? null : client;

                Time time = new Time(name, project, client, amount);

                if (time.Client == "(Without client)") // Clockify exports null as "(Without client)"
                    time.Client = null;

                times.Add(time);
            }

            exportedTimes.Times = Time.CollapseTimes(times);

            return exportedTimes;
        }

        private int GetHeaderIndex(string header, params string[] additionalHeaders)
        {
            if (headerIndexes.ContainsKey(header))
                return headerIndexes[header];

            foreach (string additionalHeader in additionalHeaders)
            {
                if (headerIndexes.ContainsKey(additionalHeader))
                    return headerIndexes[additionalHeader];
            }

            throw new GenerationException($"Could not find header {header} in the exported times");
        }

        private static Dictionary<string, int> GetHeaderIndexes(string firstLine)
        {
            Dictionary<string, int> indexes = new Dictionary<string, int>();

            string[] headers = firstLine.Split(',');

            for (int i = 0; i < headers.Length; i++)
            {
                indexes.Add(headers[i].Substring(1, headers[i].Length - 2).Trim(), i);
            }

            return indexes;
        }

        public int GetTotalCost(InvoiceConfiguration configuration)
        {
            if (Times == null)
                throw new GenerationException("Trying to get total cost but there is no time data");

            if (Times.Any(time => time.Name == null))
                throw new GenerationException("Trying to get total cost but there is a time entry without a name");

            return Times.Sum(time => time.Amount * configuration.GetUnitPrice(time.Name!));
        }
    }
}
