using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jsreport.Shared
{
    public interface IReportingBinary
    {
        Stream ReadContent();
        string UniqueId { get; }        
    }
}
