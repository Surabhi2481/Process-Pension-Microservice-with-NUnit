using NUnit.Framework;
using ProcessPension;
using ProcessPension.Controllers;
using System;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProcessPensionTesting
{
    public class ProcessPensionTests
    {
        public static IConfiguration configuration;
        ProcessPensionController controller;

        ProcessPensionInput client = new ProcessPensionInput();


        [SetUp]
        public void Setup()
        {
            configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .Build();
            controller = new ProcessPensionController(configuration);

            client.Name = "Surabhi";
            client.DateOfBirth = Convert.ToDateTime("1998-08-01");
            client.Pan = "BCDVN1234F";
            client.AadharNumber = "511122223331";
            client.PensionType = PensionType.Self;


        }

        [Test]
        public void ProcessPension_PensionStatus_IsNotNull()
        {
            var result = controller.ProcessPension(client);
            var type1 = result;
            Assert.IsNotNull(type1);
        }

        [Test]
        public void ProcessPension_PensionStatus_IsPositive()
        {
            var result = controller.ProcessPension(client);
            var type1 = result.status;
            Assert.Positive(type1);
        }

        [Test]
        public void ProcessPension_Person_Valid()
        {
            var result = controller.ProcessPension(client);
            int type = result.status;
            int statusCode = 21;
            Assert.AreEqual(type, statusCode);
        }

        [Test]
        public void ProcessPension_Person_Invalid()
        {
            var result = controller.ProcessPension(client);
            var type = result.status;
            var statusCode = 10;
            Assert.AreNotEqual(type, statusCode);
        }
    }
}