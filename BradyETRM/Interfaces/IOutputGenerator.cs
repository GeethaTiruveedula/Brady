using System;
using System.Xml.Linq;
using System.Collections.Generic;


namespace BradyETRM
{
    interface IOutputGenerator
    {
        XDocument GenerateOutputXml(Dictionary<string, double> totals, Dictionary<DateTime, (string Name, double Emission)> maxEmissions, Dictionary<string, double> heatRates);
    }
}
