using jsreport.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace jsreport.Shared
{
    public interface IRenderService
    {
        /// <summary>
        /// Specify comnpletely the rendering requests, see http://jsreport.net/learn/api for details
        /// </summary>
        /// <param name="request">ram name="request">Description of rendering process <see cref="RenderRequest"/></param>
        /// <exception cref="JsReportException"></exception>
        /// <returns>Report result promise</returns>
        Task<Report> RenderAsync(RenderRequest request, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// The simpliest rendering using template shortid and input data
        /// </summary>
        /// <param name="templateShortid">template shortid can be taken from jsreport studio or from filename in jsreport embedded</param>
        /// <param name="data">any json serializable object</param>
        /// <exception cref="JsReportException"></exception>
        /// <returns>Report result promise</returns>
        Task<Report> RenderAsync(string templateShortid, object data, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// The simpliest rendering using template name and input data
        /// </summary>
        /// <param name="templateName">template shortid can be taken from jsreport studio or from filename in jsreport embedded</param>
        /// <param name="jsonData">any json string</param>
        /// <exception cref="JsReportException"></exception>
        /// <returns>Report result promise</returns>
        Task<Report> RenderByNameAsync(string templateName, string jsonData, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// The simpliest rendering using template name and input data
        /// </summary>
        /// <param name="templateName">template name</param>
        /// <param name="data">any json serializable object</param>
        /// <exception cref="JsReportException"></exception>
        /// <returns>Report result promise</returns>
        Task<Report> RenderByNameAsync(string templateName, object data, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// The simpliest rendering using template shortid and input data
        /// </summary>
        /// <param name="templateShortid">template shortid can be taken from jsreport studio or from filename in jsreport embedded</param>
        /// <param name="jsonData">any json string</param>
        /// <exception cref="JsReportException"></exception>
        /// <returns>Report result promise</returns>
        Task<Report> RenderAsync(string templateShortid, string jsonData, CancellationToken ct = default(CancellationToken));        

        /// <summary>
        /// Specify comnpletely the rendering requests, see http://jsreport.net/learn/api for details
        /// </summary>
        /// <param name="request">ram name="request">Description of rendering process</param>
        /// <exception cref="JsReportException"></exception>
        /// <returns>Report result promise</returns>
        Task<Report> RenderAsync(object request, CancellationToken ct = default(CancellationToken));
    }    
}
