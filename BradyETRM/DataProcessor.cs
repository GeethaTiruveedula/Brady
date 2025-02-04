using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;


namespace BradyETRM
{
    // Concrete Data Processor
    class DataProcessor : IDataProcessor
    {
        private readonly Dictionary<string, double> _valueFactors;
        private readonly Dictionary<string, double> _emissionFactors;

        public DataProcessor(string referenceDataPath)
        {
            XDocument refDataXml = XDocument.Load(referenceDataPath);
            _valueFactors = LoadFactors(refDataXml, "ValueFactor");
            _emissionFactors = LoadFactors(refDataXml, "EmissionsFactor");
        }

        //Reference data file reading to store the factors
        private Dictionary<string, double> LoadFactors(XDocument refDataXml, string factorType)
        {
            return refDataXml.Descendants(factorType).Elements()
                             .ToDictionary(x => x.Name.LocalName, x => double.Parse(x.Value));
        }

        //Calculation of totals
        public Dictionary<string, double> CalculateTotals(XDocument inputXml)
        {
            var totals = new Dictionary<string, double>();
            foreach (var generator in inputXml.Descendants("WindGenerator").Concat(inputXml.Descendants("GasGenerator")).Concat(inputXml.Descendants("CoalGenerator")))
            {
                string name = generator.Element("Name").Value;
                string type = string.Empty;
                if (name.Equals("Wind[Offshore]"))
                {
                    type = "Low";
                }
                else if (name.Equals("Wind[Onshore]"))
                {
                    type = "High";
                }
                else if (name.Contains("Gas"))
                {
                    type = "Medium";
                }
                else if (name.Contains("Coal"))
                {
                    type = "Medium";
                }
                double factor = _valueFactors.ContainsKey(type) ? _valueFactors[type] : 1.0;

                double total = generator.Descendants("Day").Sum(day =>
                    double.Parse(day.Element("Energy").Value) *
                    double.Parse(day.Element("Price").Value) * factor);

                totals[name] = total;
            }
            return totals;
        }

        //Calculation of maximum emission
        public Dictionary<DateTime, (string Name, double Emission)> CalculateMaxEmissions(XDocument inputXml)
        {
            var emissionsByDay = new Dictionary<DateTime, (string Name, double Emission)>();
            foreach (var generator in inputXml.Descendants("GasGenerator").Concat(inputXml.Descendants("CoalGenerator")))
            {
                string name = generator.Element("Name").Value;
                double emissionRating = double.Parse(generator.Element("EmissionsRating").Value);
                string type = string.Empty;
                if (name.Contains("Gas"))
                {
                    type = "Medium";
                }
                else if (name.Contains("Coal"))
                {
                    type = "High";
                }
                //string type = generator.Name.LocalName.Replace("Generator", "");
                double factor = _emissionFactors.ContainsKey(type) ? _emissionFactors[type] : 1.0;

                foreach (var day in generator.Descendants("Day"))
                {
                    DateTime date = DateTime.Parse(day.Element("Date").Value);
                    double energy = double.Parse(day.Element("Energy").Value);
                    double emission = energy * emissionRating * factor;

                    if (!emissionsByDay.ContainsKey(date) || emissionsByDay[date].Emission < emission)
                    {
                        emissionsByDay[date] = (name, emission);
                    }
                }
            }
            return emissionsByDay;
        }


        //Calculation of Heat Rates
        public Dictionary<string, double> CalculateHeatRates(XDocument inputXml)
        {
            return inputXml.Descendants("CoalGenerator").ToDictionary(
                g => g.Element("Name").Value,
                g => double.Parse(g.Element("TotalHeatInput").Value) /
                     double.Parse(g.Element("ActualNetGeneration").Value)
            );
        }
    }

}
