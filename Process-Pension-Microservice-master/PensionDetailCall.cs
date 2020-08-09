using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ProcessPension
{
    public class PensionDetailCall
    {
        /// <summary>
        /// Dependency Injection
        /// </summary>
        private IConfiguration configuration;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(PensionDetailCall));
        public PensionDetailCall(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        /// <summary>
        /// Calling the Pension Detail Microservice
        /// </summary>
        /// <param name="aadhar"></param>
        /// <returns>value for calculations and client input</returns>
        public HttpResponseMessage PensionDetail(string aadhar)
        {
            PensionDetailCall banktype = new PensionDetailCall(configuration);
            ProcessPensionInput res = new ProcessPensionInput();
            HttpResponseMessage response = new HttpResponseMessage();
            string uriConn = configuration.GetValue<string>("MyUriLink:UriLink");
            using (var client = new HttpClient())
            {
                client.BaseAddress =new Uri (uriConn);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    response = client.GetAsync("api/PensionerDetail/" + aadhar).Result;
                }
                catch(Exception e)
                {
                    _log4net.Error("Exception Occured" + e);
                    return null; 
                }
            }
            return response;
        }
        /// <summary>
        /// Getting the Values from Process Management Portal
        /// </summary>
        /// <param name="aadhar"></param>
        /// <returns></returns>
        public ProcessPensionInput GetClientInfo(string aadhar)
        {
            ResultforProcessPensionInput res = new ResultforProcessPensionInput();
            HttpResponseMessage response = PensionDetail(aadhar);
            if (response == null)
            {
                res = null;
                return null;
            }
            string responseValue = response.Content.ReadAsStringAsync().Result;
            res = JsonConvert.DeserializeObject<ResultforProcessPensionInput>(responseValue);
            if(res==null)
            {
                return null;
            }
            return res.result;
        }
        /// <summary>
        /// Getting the values for calculation
        /// </summary>
        /// <param name="aadhar"></param>
        /// <returns>Values required for calculation</returns>
        public ValueforCalCulation GetCalculationValues(string aadhar)
        {
            ResultforValueCalculation res = new ResultforValueCalculation();
            HttpResponseMessage response = PensionDetail(aadhar);
            if (response == null)
            {
                res = null;
                return res.result;
            }
            string responseValue = response.Content.ReadAsStringAsync().Result;
            res = JsonConvert.DeserializeObject<ResultforValueCalculation>(responseValue);

           return res.result;
        }

    }
}
