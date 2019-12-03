using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace jsreport.Shared
{
    public class ReportingBinary : IReportingBinary
    {
        private Func<Stream> _readContent;
        private bool _compresed = false;

        public ReportingBinary(string uniqueId, Func<Stream> readContent)
        {
            UniqueId = uniqueId;
            _readContent = readContent;                        
        }

        public ReportingBinary(string uniqueId, Func<Stream> readContent, bool compressed)
        {
            UniqueId = uniqueId;
            _readContent = readContent;
            _compresed = compressed;
        }

        public Stream ReadContent()
        {
            if (_compresed)
            {
                var zip = new ZipArchive(_readContent());
                return zip.Entries.First().Open();
            }
            return _readContent();
        }                

        public string UniqueId { get; set; }
    }
}
