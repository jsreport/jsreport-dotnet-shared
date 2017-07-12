using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;

namespace jsreport.Shared.Test
{
    [TestFixture]
    public class SerializerHelperTest
    {
        [Test]
        public void TestParseReportMetaFromHeaders()
        {
            var report = SerializerHelper.ParseReportMetaFromHeaders(new Dictionary<string, string>()
            {
                { "Content-Type", "text/html" } 
            });

            report.ContentType.ShouldBe("text/html");
        }

        [Test]
        public void TestParseReportMeta()
        {
            var report = SerializerHelper.ParseReportMeta(new Dictionary<string, string>()
            {
                { "contentType", "text/html" }
            });

            report.ContentType.ShouldBe("text/html");            
        }
    }
}
