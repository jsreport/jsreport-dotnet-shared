using jsreport.Types;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

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

        [Test]
        public void TestStandardPdfMetaSerialized()
        {
            var renderRequest = new RenderRequest
            {
                Template = new Template
                {
                    Name = "foo",
                    PdfMeta = new PdfMeta
                    {
                        Author = "An Author",
                        Creator = "A Creator",
                        Keywords = "Some Keywords",
                        Producer = "A Producer",
                        Subject = "A Subject",
                        Title = "A Great Title",
                    }
                }
            };
            var serialized = SerializerHelper.SerializeRenderRequest(renderRequest, new CamelCasePropertyNamesContractResolver());

            serialized.ShouldContain("\"pdfMeta\": {", Case.Sensitive);
            serialized.ShouldContain("\"author\": \"An Author\"", Case.Sensitive);
            serialized.ShouldContain("\"creator\": \"A Creator\"", Case.Sensitive);
            serialized.ShouldContain("\"keywords\": \"Some Keywords\"", Case.Sensitive);
            serialized.ShouldContain("\"producer\": \"A Producer\"", Case.Sensitive);
            serialized.ShouldContain("\"subject\": \"A Subject\"", Case.Sensitive);
            serialized.ShouldContain("\"title\": \"A Great Title\"", Case.Sensitive);
        }

        [Test]
        public void TestCustomPdfMetaPropertyPropertyCasingPreserved()
        {
            var renderRequest = new RenderRequest
            {
                Template = new Template
                {
                    Name = "foo",
                    PdfMeta = new PdfMeta
                    {
                        Custom = new Dictionary<string, string>
                        {
                            ["A Custom Property"] = "with a value",
                            ["another_Value_holder"] = "more value"
                        }
                    }
                }
            };
            var serialized = SerializerHelper.SerializeRenderRequest(renderRequest, new CamelCasePropertyNamesContractResolver());

            serialized.ShouldContain("\"custom\": {", Case.Sensitive);
            serialized.ShouldContain("\"A Custom Property\": \"with a value\"", Case.Sensitive);
            serialized.ShouldContain("\"another_Value_holder\": \"more value\"", Case.Sensitive);
        }

        [Test]
        public void TestCustomPdfMetaPropertyOnlyHasValuesInDictionary()
        {
            var renderRequest = new RenderRequest
            {
                Template = new Template
                {
                    Name = "foo",
                    PdfMeta = new PdfMeta
                    {
                        Custom = new Dictionary<string, string>
                        {
                            ["some property"] = "with awesome value"
                        }
                    }
                }
            };

            var serialized = SerializerHelper.SerializeRenderRequest(renderRequest, new CamelCasePropertyNamesContractResolver());

            var result = JObject.Parse(serialized);
            var customPdfMetaResult = result["template"]?["pdfMeta"]?["custom"] as JObject;

            customPdfMetaResult.ShouldNotBeNull();
            customPdfMetaResult.Properties().ShouldContain(property => property.Name == "some property");
            customPdfMetaResult.Properties().Count().ShouldBe(1);
        }
    }
}
