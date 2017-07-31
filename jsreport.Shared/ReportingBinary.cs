using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jsreport.Shared
{
    public class ReportingBinary : IReportingBinary
    {
        private Func<Stream> _readContent;

        public ReportingBinary(string uniqueId, Func<Stream> readContent)
        {
            UniqueId = uniqueId;
            _readContent = readContent;                        
        }

        public Stream ReadContent()
        {
            return _readContent();
        }        

        public string UniqueId { get; set; }
    }
}
