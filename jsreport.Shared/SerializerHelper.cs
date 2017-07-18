using jsreport.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;
using System.Text.RegularExpressions;

namespace jsreport.Shared
{    
    public static class SerializerHelper
    {
        public static ReportMeta ParseReportMeta(IDictionary<string, string> meta)
        {
            IEnumerable<LogEntry> logs = null;

            if (meta.MetaValue("debugLogs") != null)
            {
                var joLogs = JArray.Parse(meta.MetaValue("debugLogs"));
                logs = joLogs.Select(s => new LogEntry()
                {
                    Message = s["message"].Value<string>(),
                    Level = (LogEntryLevel)Enum.Parse(typeof(LogEntryLevel), s["level"].Value<string>(), true),
                    Timestamp = s["timestamp"].Value<DateTime>()
                });
            }

            return new ReportMeta
            {
                ReportPernamentLink = meta.MetaValue("pernamentLink"),
                AsyncReportLocation = meta.MetaValue("location"),
                ContentType = meta.MetaValue("contentType"),
                ReportId = meta.MetaValue("reportType"),
                NumberOfPages = meta.MetaValueInt("numberOfPages"),
                Logs = logs,
                ContentDisposition = meta.MetaValue("contentDisposition"),
                FileExtension = meta.MetaValue("fileExtension"),
                RawDictionary = meta
            };
        }

        public static ReportMeta ParseReportMetaFromHeaders(IDictionary<string, string> headers)
        {
            var meta = new Dictionary<string, string>();

            foreach (var h in headers)
            {
                var key = new Regex("-.{1}").Replace(h.Key, (match) =>
                {
                    return match.Value[1].ToString().ToUpper();
                });

                key = key[0].ToString().ToLower() + key.Substring(1);
                meta.Add(key, h.Value);                
            }

            return ParseReportMeta(meta);
        }        

        private static int? MetaValueInt(this IDictionary<string, string> meta, string key)
        {
            var str = MetaValue(meta, key);
            return str != null ? int.Parse(str) : (int?)null;

        }

        private static string MetaValue(this IDictionary<string, string> meta, string key)
        {
            if (meta.ContainsKey(key))
            {
                return meta[key];
            }

            var metaKey = key.Replace("-", "");
            metaKey = metaKey.Substring(0, 1).ToLower() + metaKey.Substring(1);

            return meta.ContainsKey(metaKey) ? meta[metaKey] : null;
        }

        public static string SerializeRenderRequest(RenderRequest rr)
        {
            var js = new JsonSerializer()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            var jo = JObject.FromObject(rr, js);
            
            jo["overwrites"]?["template"]?.Values().ToList()
                .ForEach((val => jo["template"][val.Path.Replace("overwrites.template.", "")] = val));

            jo["overwrites"]?["options"]?.Values().ToList()
                .ForEach((val => jo["options"][val.Path.Replace("overwrites.options.", "")] = val));

            jo.Children().FirstOrDefault(c => c.Path.Contains("overwrites"))?.Remove();

            return jo.ToString();            
        }

        public static string SerializeRenderRequest(object r)
        {
            var js = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            return JsonConvert.SerializeObject(r, js);
        }

        public static string SerializeRenderRequest(string templateShortid, object data)
        {
            if (string.IsNullOrEmpty(templateShortid))
                throw new ArgumentNullException("templateShortid cannot be null");

            return SerializeRenderRequest(new RenderRequest()
            {
                Template = new Template()
                {
                    Shortid = templateShortid
                },
                Data = data
            });
        }

        public static string SerializeRenderRequest(string templateShortid, string jsonData)
        {
            if (string.IsNullOrEmpty(templateShortid))
                throw new ArgumentNullException("templateShortid cannot be null");

            return SerializeRenderRequest(new RenderRequest()
            {
                Template = new Template()
                {
                    Shortid = templateShortid
                },
                Data = string.IsNullOrEmpty(jsonData) ? (object)null : JObject.Parse(jsonData)
            });            
        }

        public static string SerializeRenderRequestForName(string templateName, object data)
        {
            if (string.IsNullOrEmpty(templateName))
                throw new ArgumentNullException("templateName cannot be null");

            return SerializeRenderRequest(new RenderRequest()
            {
                Template = new Template()
                {
                    Name = templateName
                },
                Data = data
            });
        }

        public static string SerializeRenderRequestForName(string templateName, string jsonData)
        {
            if (string.IsNullOrEmpty(templateName))
               throw new ArgumentNullException("templateName cannot be null");

            return SerializeRenderRequest(new RenderRequest()
            {
                Template = new Template()
                {
                    Name = templateName
                },
                Data = string.IsNullOrEmpty(jsonData) ? (object)null : JObject.Parse(jsonData),                  
            });          
        }

        public static IDictionary<string, string> SerializeConfigToDictionary(Configuration cfg)
        {
            var cfgDictionary = new Dictionary<string, string>();
            InnerSerializeConfiguration(cfg, cfgDictionary);
            return cfgDictionary;
        }

        private static void InnerSerializeConfiguration(object obj, IDictionary<string, string> config)
        {             
            foreach (PropertyInfo property in obj.GetType().GetRuntimeProperties())
            {
                var value = property.GetValue(obj, null);

                if (value == null)
                {
                    continue;
                }

                if (property.GetCustomAttribute<DataMemberAttribute>() == null)
                {
                    InnerSerializeConfiguration(value, config);
                }
                else
                {
                    var key = property.GetCustomAttribute<DataMemberAttribute>().Name;
                    config[key] = value.ToString();
                }
            }    
        }
    }  
}
