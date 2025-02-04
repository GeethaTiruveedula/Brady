using System;
using System.IO;
using System.Xml.Linq;


namespace BradyETRM
{
    // Concrete File Processor
    class XmlFileProcessor : IFileProcessor
    {
        private readonly IDataProcessor _dataProcessor;
        private readonly IOutputGenerator _outputGenerator;

        public XmlFileProcessor(IDataProcessor dataProcessor, IOutputGenerator outputGenerator)
        {
            _dataProcessor = dataProcessor;
            _outputGenerator = outputGenerator;
        }

        //Processing the input files and setting th path to save the output file
        public void ProcessFile(string filePath, string outputFolder)
        {
            try
            {
                XDocument inputXml = XDocument.Load(filePath);
                var totals = _dataProcessor.CalculateTotals(inputXml);
                var maxEmissions = _dataProcessor.CalculateMaxEmissions(inputXml);
                var heatRates = _dataProcessor.CalculateHeatRates(inputXml);

                XDocument outputXml = _outputGenerator.GenerateOutputXml(totals, maxEmissions, heatRates);
                string outputFilePath = Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(filePath) + "-Result.xml");
                outputXml.Save(outputFilePath);

                Console.WriteLine($"Processed: {filePath}, Output saved to: {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
            }
        }
    }
}
