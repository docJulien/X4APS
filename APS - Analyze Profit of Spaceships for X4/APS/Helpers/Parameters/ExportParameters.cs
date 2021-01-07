using System.Collections.Generic;

namespace APS.Helpers.Parameters
{
    public class ExportParameters
    {
        public string FileName { get; set; }
        public string Logo { get; set; }
        public IEnumerable<string> Headers { get; set; }
        public IEnumerable<IEnumerable<string>> Data { get; set; }
    }
}
