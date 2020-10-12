using jsreport.Types;
using Newtonsoft.Json.Serialization;
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

        [Test]
        public void TestShouldntChangeDataPropsCasing()
        {
            var serialized = SerializerHelper.SerializeRenderRequest(new RenderRequest
            {
                Template = new Template
                {
                    Name = "foo"
                },
                Data = new
                {
                    aA = 1,
                    Bb = 2
                }
            });

            serialized.ShouldContain("aA", Case.Sensitive);
            serialized.ShouldContain("Bb", Case.Sensitive);
            serialized.ShouldContain("\"data\": {", Case.Sensitive);
        }

        [Test]
        public void TestShouldntFailWhenSerializingRenderRequestWithNullData()
        {
            var serialized = SerializerHelper.SerializeRenderRequest(new RenderRequest
            {
                Template = new Template
                {
                    Name = "foo"
                }                
            });

            serialized.ShouldContain("template", Case.Sensitive);           
        }

        [Test]
        public void TestShouldntChangeDataPropsCasingForAnonymousObject()
        {
            var serialized = SerializerHelper.SerializeRenderRequest(new  {
                Template = new 
                {
                    name = "foo"
                },
                data = new
                {
                    aA = 1,
                    Bb = 2
                }
            });

            serialized.ShouldContain("aA", Case.Sensitive);
            serialized.ShouldContain("Bb", Case.Sensitive);
            serialized.ShouldContain("\"data\": {", Case.Sensitive);

            serialized.ShouldContain("template", Case.Sensitive);
        }

        [Test]
        public void TestShouldntFailWhenSerializingAnonymousRenderRequestWithNullData()
        {
            var serialized = SerializerHelper.SerializeRenderRequest(new  {
                template = new  {
                    Name = "foo"
                }
            });

            serialized.ShouldContain("template", Case.Sensitive);
        }

        [Test]
        public void TestShouldApplyCustomContractResolver()
        {
            var serialized = SerializerHelper.SerializeRenderRequest(new RenderRequest
            {
                Template = new Template
                {
                    Name = "foo"
                },
                Data = new
                {
                    HelloWorld = 1
                }
            }, new CamelCasePropertyNamesContractResolver());

            serialized.ShouldContain("helloWorld", Case.Sensitive);            
            serialized.ShouldContain("\"data\": {", Case.Sensitive);
        }

        [Test]
        public void TestSerializeConfigToDictionaryWithEnum()
        {
            var dicitonary = SerializerHelper.SerializeConfigToDictionary(new Configuration
            {
                Chrome = new ChromeConfiguration
                {
                    Strategy = ChromeStrategy.ChromePool
                }
            });


            dicitonary["chrome_strategy"].ShouldBe("chrome-pool");
        }
    }
}
