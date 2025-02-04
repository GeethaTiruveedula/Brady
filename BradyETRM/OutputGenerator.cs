using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;


namespace BradyETRM
{
    // Concrete Output Generator
    class XmlGenerator : IOutputGenerator
    {
        //Output Geeration
        public XDocument GenerateOutputXml(Dictionary<string, double> totals, Dictionary<DateTime, (string Name, double Emission)> maxEmissions, Dictionary<string, double> heatRates)
        {
            return new XDocument(
                new XElement("GenerationOutput",
                    new XElement("Totals",
                        totals.Select(t => new XElement("Generator", new XElement("Name", t.Key), new XElement("Total", t.Value)))),
                    new XElement("MaxEmissionGenerators",
                        maxEmissions.Select(e =>
                            new XElement("Day",
                                new XElement("Name", e.Value.Name),
                                new XElement("Date", e.Key.ToString("o")),
                                new XElement("Emission", e.Value.Emission))
                        )
                    ),
                    new XElement("ActualHeatRates",
                        heatRates.Select(h =>
                            new XElement("ActualHeatRate",
                                new XElement("Name", h.Key),
                                new XElement("HeatRate", h.Value))
                        )
                    )
                )
            );
        }
    }
}
