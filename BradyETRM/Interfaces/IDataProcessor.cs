using System;
using System.Xml.Linq;
using System.Collections.Generic;


namespace BradyETRM
{
    interface IDataProcessor
    {
        Dictionary<string, double> CalculateTotals(XDocument inputXml);
        Dictionary<DateTime, (string Name, double Emission)> CalculateMaxEmissions(XDocument inputXml);
        Dictionary<string, double> CalculateHeatRates(XDocument inputXml);
    }

}
