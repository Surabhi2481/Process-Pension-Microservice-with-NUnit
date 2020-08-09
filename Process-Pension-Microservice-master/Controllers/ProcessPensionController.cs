using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ProcessPension.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessPensionController : ControllerBase
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(ProcessPensionController));
        private IConfiguration configuration;
        /// <summary>
        /// Dependency Injection
        /// </summary>
        /// <param name="iConfig"></param>
        public ProcessPensionController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }

        /// <summary>
        /// 1. This method is taking values given by MVC Client i.e. Pension Management Portal as Parameter
        /// 2. Calling the Pension Detail Microservice and checking all the values
        /// 3. Calling the Pension Disbursement Microservice to get the Status Code
        /// </summary>
        /// <param name="processPensionInput"></param>
        /// <returns>Details to be displayed on the MVC Client</returns>
        [Route("[action]")]
        [HttpPost]
        public PensionDetail ProcessPension(ProcessPensionInput processPensionInput)
        {
            _log4net.Info("Pensioner details invoked from Client Input");
            ProcessPensionInput client = new ProcessPensionInput();
            client.Name = processPensionInput.Name;
            client.AadharNumber = processPensionInput.AadharNumber;
            client.Pan = processPensionInput.Pan;
            client.DateOfBirth = processPensionInput.DateOfBirth;
            client.PensionType = processPensionInput.PensionType;

            PensionDetailCall pension = new PensionDetailCall(configuration);
            ProcessPensionInput pensionDetail = pension.GetClientInfo(client.AadharNumber);

            if (pensionDetail == null)
            {
                PensionDetail mvc = new PensionDetail();
                mvc.name = "";
                mvc.pan = "";
                mvc.pensionAmount = 0;
                mvc.dateOfBirth = new DateTime(2000, 01, 01);
                mvc.bankType = 1;
                mvc.aadharNumber = "***";
                mvc.status = 20;
                return mvc;
            }
           
            int bankType = pension.GetCalculationValues(client.AadharNumber).BankType;
      
            PensionType pensionType = pension.GetCalculationValues(client.AadharNumber).PensionType;
            double pensionAmount;

            double salary = pension.GetCalculationValues(client.AadharNumber).SalaryEarned;
            double allowances = pension.GetCalculationValues(client.AadharNumber).Allowances;

            if (pensionType == PensionType.Self)
                pensionAmount = (0.8 * salary) + allowances;
            else
                pensionAmount = (0.5 * salary) + allowances;

            if (pension.GetCalculationValues(client.AadharNumber).BankType == 1)
                pensionAmount = pensionAmount + 500;
            else
                pensionAmount = pensionAmount + 550;

            int statusCode;

            PensionDetail mvcClientOutput = new PensionDetail();

            if (client.Pan.Equals(pensionDetail.Pan)&&client.Name.Equals(pensionDetail.Name)&& client.PensionType.Equals(pensionDetail.PensionType)&& client.DateOfBirth.Equals(pensionDetail.DateOfBirth))
            {
                mvcClientOutput.name = pensionDetail.Name;
                mvcClientOutput.pan = pensionDetail.Pan;
                mvcClientOutput.pensionAmount = pensionAmount;
                mvcClientOutput.dateOfBirth = pensionDetail.DateOfBirth.Date;
                mvcClientOutput.pensionType = pension.GetCalculationValues(client.AadharNumber).PensionType;
                mvcClientOutput.bankType = bankType;
                mvcClientOutput.aadharNumber = pensionDetail.AadharNumber;
                mvcClientOutput.status = 20;
            }
            else
            {
                mvcClientOutput.name = "";
                mvcClientOutput.pan = "";
                mvcClientOutput.pensionAmount = 0;
                mvcClientOutput.dateOfBirth = new DateTime(2000, 01, 01);
                mvcClientOutput.pensionType = pension.GetCalculationValues(client.AadharNumber).PensionType;
                mvcClientOutput.bankType = 1;
                mvcClientOutput.aadharNumber = "****";
                mvcClientOutput.status = 21;

                return mvcClientOutput;
            }


            string uriConn2 = configuration.GetValue<string>("MyUriLink:PensionDisbursementLink");
            HttpResponseMessage response = new HttpResponseMessage();
            using (var client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri(uriConn2);
                StringContent content = new StringContent(JsonConvert.SerializeObject(mvcClientOutput), Encoding.UTF8, "application/json");
                client1.DefaultRequestHeaders.Clear();
                client1.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    response = client1.PostAsync("api/PensionDisbursement", content).Result;
                }
                catch (Exception e)
                {
                    _log4net.Error("Exception Occured"+e);
                    response = null;
                }
            }
            if (response != null)
            {
                string status = response.Content.ReadAsStringAsync().Result;
                //statusCode = Int32.Parse(status);
                Result result = JsonConvert.DeserializeObject<Result>(status);
                statusCode = result.result;
                mvcClientOutput.status = statusCode;

                return mvcClientOutput;
            }
            return mvcClientOutput;
        }
    }
    public class Result
    {
        public string message { get; set; }
        public int result { get; set; }
    }

}
