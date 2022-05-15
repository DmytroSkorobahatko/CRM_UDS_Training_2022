using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using UDS.Study;

namespace Block2_UDSTraining
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Authenticate
            string connectionString = @"AuthType=OAuth;
        Username=#######@#######.onmicrosoft.com;
        Password=#######; 
        Url=https://carrentingcompany.crm4.dynamics.com;
        AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
        RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;"; // set your Username and Password

            CrmServiceClient service = new CrmServiceClient(connectionString);

            CrmServiceContext context = new CrmServiceContext(service);


            string queryContacts = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
  <entity name='contact'>
    <attribute name='fullname' />
    <attribute name='parentcustomerid' />
    <attribute name='telephone1' />
    <attribute name='emailaddress1' />
    <attribute name='contactid' />
    <order attribute='fullname' descending='false' />
    <filter type='and'>
      <condition attribute='ownerid' operator='eq-userid' />
      <condition attribute='statecode' operator='eq' value='0' />
    </filter>
  </entity>
</fetch>";

            IQueryable<cr4de_carclass> carClasses =
                from a in context.cr4de_carclassSet
                where a.cr4de_name != null
                select a;

            IQueryable<cr4de_car> cars =
                from a in context.cr4de_carSet
                where a.cr4de_vinnumber != null
                select a;

            OptionSetValue stateCode = new OptionSetValue(0); // default = Active | 1 for Inactive
            OptionSetValue statusCode = new OptionSetValue(420660000); // default = Created 
            List<KeyValuePair<OptionSetValue, double>> listStatusCodes = new List<KeyValuePair<OptionSetValue, double>> {
                new KeyValuePair<OptionSetValue, double>(new OptionSetValue(420660000), 0.05), // Created 
                new KeyValuePair<OptionSetValue, double>(new OptionSetValue(420660001), 0.05), // Confirmed 
                new KeyValuePair<OptionSetValue, double>(new OptionSetValue(420660002), 0.05), // Renting
                new KeyValuePair<OptionSetValue, double>(new OptionSetValue(420660005), 0.10), // Canceled 
                new KeyValuePair<OptionSetValue, double>(new OptionSetValue(420660006), 0.75)}; // Returned 

            OptionSetValue[] locations = new OptionSetValue[] {
                new OptionSetValue(420660000), new OptionSetValue(420660001), new OptionSetValue(420660002)};

            Entity newRent = new Entity("cr4de_rent");

            DateTime pickupDate = GeneratePickupDate(ref newRent); // to repeat 40.000
            DateTime returnDate = GenerateHandoverDate(ref pickupDate, ref newRent); // to repeat 40.000

            cr4de_carclass carClass = GenerateCarClassFromList(ref carClasses, ref newRent); // to repeat 40.000

            cr4de_car car = GenerateCarRespectCarClass(ref carClass, ref cars, ref newRent); // to repeat 40.000

            GenerateLocations(ref locations, ref newRent); // to repeat 40.000

            GenerateContact(ref service, ref queryContacts, ref newRent); // to repeat 40.000 

            statusCode = GenerateStatus(ref stateCode, ref statusCode, ref listStatusCodes, ref newRent); // to repeat 40.000

            GenerateReportsRespectStatus(ref service, ref statusCode, ref pickupDate, ref returnDate, ref car, ref newRent);


            CreateRequest rentReq = new CreateRequest();
            rentReq.Target = newRent;

            CreateResponse response = (CreateResponse)service.Execute(rentReq);
            
                     
        }

        private static DateTime GeneratePickupDate(ref Entity newRent)
        {
            Random gen = new Random();
            DateTime start = new DateTime(2019, 1, 1);
            DateTime finish = new DateTime(2020, 12, 31);
            int range = (finish - start).Days;
            
            DateTime pickupDate = start.AddDays(gen.Next(range));
                        
            newRent.Attributes.Add("cr4de_reservedpickup", pickupDate);

            return pickupDate;
        }

        private static DateTime GenerateHandoverDate(ref DateTime pickupDate, ref Entity newRent)
        {
            Random gen = new Random();
            DateTime handoverDate = pickupDate.AddDays(gen.Next(1, 30));

            newRent.Attributes.Add("cr4de_reservedhandover", handoverDate);

            return handoverDate;
        }

        private static cr4de_carclass GenerateCarClassFromList(
            ref IQueryable<cr4de_carclass> carClasses, ref Entity newRent)
        {
            Random randgen = new Random();

            cr4de_carclass carClass = carClasses.AsEnumerable().OrderBy(x => randgen.Next()).FirstOrDefault();

            newRent.Attributes["cr4de_carclass"] = new EntityReference("cr4de_carclass", (Guid)carClass.cr4de_carclassId);

            newRent.Attributes.Add("cr4de_price", carClass.cr4de_Price);

            return carClass;
        }

        private static cr4de_car GenerateCarRespectCarClass(
            ref cr4de_carclass carClass, ref IQueryable<cr4de_car> cars, ref Entity newRent)
        {
            cr4de_car[] carsRespectCarClass = {};

            foreach (var c in cars)
            {
                if (c.cr4de_car_class.Name.ToString() == carClass.cr4de_name.ToString())
                {
                    Array.Resize(ref carsRespectCarClass, carsRespectCarClass.Length + 1);
                    carsRespectCarClass[carsRespectCarClass.Length - 1] = c;
                }
            }
            Random randgen = new Random();

            cr4de_car car = carsRespectCarClass[randgen.Next(carsRespectCarClass.Length)];

            newRent.Attributes["cr4de_car"] = new EntityReference("cr4de_car", (Guid)car.cr4de_carId);

            return car;
        }

        private static void GenerateLocations(ref OptionSetValue[] locations, ref Entity newRent)
        {
            Random randgen = new Random();
            OptionSetValue pickupLocation = locations[randgen.Next(locations.Length)];
            randgen = new Random(); // is it ok here?                                                             ????????? 
            OptionSetValue returnLocation = locations[randgen.Next(locations.Length)];

            newRent.Attributes.Add("cr4de_pickuplocation", pickupLocation);
            newRent.Attributes.Add("cr4de_returnlocation", returnLocation);
        }

        private static void GenerateContact(
            ref CrmServiceClient service, ref string queryContacts, ref Entity newRent)
        {
            EntityCollection contactsCollection = service.RetrieveMultiple(new FetchExpression(queryContacts));
            Random randgen = new Random();

            var contact = contactsCollection.Entities.OrderBy(x => randgen.Next()).First();

            newRent.Attributes["cr4de_customer"] = new EntityReference(contact.LogicalName, (Guid)contact.Attributes["contactid"]);
        }

        private static OptionSetValue GenerateStatus(ref OptionSetValue stateCode, ref OptionSetValue statusCode, 
            ref List<KeyValuePair<OptionSetValue, double>> listStatusCodes, ref Entity newRent)
        {
            Random randStatus = new Random();
            double diceRoll = randStatus.NextDouble();

            double cumulative = 0.0;
            for (int i = 0; i < listStatusCodes.Count; i++)
            {
                cumulative += listStatusCodes[i].Value;
                if (diceRoll < cumulative)
                {
                    statusCode = listStatusCodes[i].Key;
                    if (statusCode.Value < 420660005)
                        stateCode = new OptionSetValue(0);
                    else
                        stateCode = new OptionSetValue(1);
                    break;
                }
            }

            newRent.Attributes.Add("statecode", stateCode);
            newRent.Attributes.Add("statuscode", statusCode);

            return statusCode;
        }

        private static void CreatePickupReport(ref CrmServiceClient service,
            ref DateTime pickupDate, ref cr4de_car car, ref Entity newRent) // Renting
        {
            Entity newPickupReport = new Entity("cr4de_cartransferreport");

            newPickupReport.Attributes.Add("cr4de_type", new OptionSetValue(420660000)); // pickup report
            newPickupReport.Attributes.Add("cr4de_date", pickupDate);
            newPickupReport.Attributes.Add("statecode", new OptionSetValue(1));
            newPickupReport.Attributes.Add("statuscode", new OptionSetValue(1));
            newPickupReport.Attributes["cr4de_car"] = new EntityReference("cr4de_car", (Guid)car.cr4de_carId);

            CreateRequest pickupReq = new CreateRequest();
            pickupReq.Target = newPickupReport;
            CreateResponse pickupRespons = (CreateResponse)service.Execute(pickupReq);

            Random random = new Random();

            if (random.NextDouble() <= 0.05)
            {
                newPickupReport.Attributes.Add("cr4de_damage", true);
                newPickupReport.Attributes.Add("cr4de_damagedescrition", "damage");
            }
            else
                newPickupReport.Attributes.Add("cr4de_damage", false);

            newRent.Attributes["cr4de_pickupreport"] = new EntityReference("cr4de_cartransferreport", pickupRespons.id);
        }
    
        private static void CreateReturnReport(ref CrmServiceClient service,
            ref DateTime returnDate, ref cr4de_car car, ref Entity newRent) // Returned
        {
            Entity newReturnReport = new Entity("cr4de_cartransferreport");

            newReturnReport.Attributes.Add("cr4de_type", new OptionSetValue(420660001)); // return
            newReturnReport.Attributes.Add("cr4de_date", returnDate); // ? what the date (handover?)        ????????????
            newReturnReport.Attributes.Add("statecode", new OptionSetValue(1));
            newReturnReport.Attributes.Add("statuscode", new OptionSetValue(1));
            newReturnReport.Attributes["cr4de_car"] = new EntityReference("cr4de_car", (Guid)car.cr4de_carId);

            CreateRequest returnReq = new CreateRequest();
            returnReq.Target = newReturnReport;
            CreateResponse returnRespons = (CreateResponse)service.Execute(returnReq); // return report created

            Random random = new Random();

            if (random.NextDouble() <= 0.05)
            {
                newReturnReport.Attributes.Add("cr4de_damage", true);
                newReturnReport.Attributes.Add("cr4de_damagedescrition", "damage");
            }
            else
                newReturnReport.Attributes.Add("cr4de_damage", false);

            newRent.Attributes["cr4de_returnreport"] = new EntityReference("cr4de_cartransferreport", returnRespons.id); // report has no field "name"
        }

        private static void CreateReport(ref CrmServiceClient service,
            ref DateTime date, ref cr4de_car car, ref Entity newRent, OptionSetValue reportType) // Returned
        {
            Entity newReport = new Entity("cr4de_cartransferreport");
            
            newReport.Attributes.Add("cr4de_type", reportType); // return
            newReport.Attributes.Add("cr4de_date", date); // ? what the date (handover?)        ????????????
            newReport.Attributes.Add("statecode", new OptionSetValue(1));
            newReport.Attributes.Add("statuscode", new OptionSetValue(1));
            newReport.Attributes["cr4de_car"] = new EntityReference("cr4de_car", (Guid)car.cr4de_carId);

            CreateRequest reportReq = new CreateRequest();
            reportReq.Target = newReport;
            CreateResponse reportRespons = (CreateResponse)service.Execute(reportReq); // return report created

            Random random = new Random();

            if (random.NextDouble() <= 0.05)
            {
                newReport.Attributes.Add("cr4de_damage", true);
                newReport.Attributes.Add("cr4de_damagedescrition", "damage");
            }
            else
                newReport.Attributes.Add("cr4de_damage", false);

            newRent.Attributes["cr4de_returnreport"] = new EntityReference("cr4de_cartransferreport", reportRespons.id); // report has no field "name"
        }
        
        private static void SetPaidORNotByChance(double chanceForPaid, ref Entity newRent)
        {
            Random random = new Random();

            if (random.NextDouble() <= chanceForPaid)
                newRent.Attributes.Add("cr4de_paid", true);
            else
                newRent.Attributes.Add("cr4de_paid", false);
        }

        private static void GenerateReportsRespectStatus(ref CrmServiceClient service, ref OptionSetValue statusCode,
            ref DateTime pickupDate, ref DateTime returnDate, ref cr4de_car car, ref Entity newRent)
        {
            //OptionSetValue[] reportTypes = new OptionSetValue[] { new OptionSetValue(420660000), new OptionSetValue(420660001) };     ?

            switch (statusCode.Value)
            {
                // Status == Renting
                case 420660002:
                    //CreateReport(ref service, ref pickupDate, ref car, ref newRent, ref reportTypes[0]);          // pickup           ?
                    CreateReport(ref service, ref pickupDate, ref car, ref newRent, new OptionSetValue(420660000)); // pickup
                    SetPaidORNotByChance(0.999, ref newRent);
                    break;

                // Status == Returned
                case 420660006: 
                    CreateReport(ref service, ref pickupDate, ref car, ref newRent, new OptionSetValue(420660000)); // pickup
                    CreateReport(ref service, ref returnDate, ref car, ref newRent, new OptionSetValue(420660001)); // return
                    //CreateReport(ref service, ref pickupDate, ref car, ref newRent, ref reportTypes[0]);          // pickup           ?
                    //CreateReport(ref service, ref returnDate, ref car, ref newRent, ref reportTypes[1]);          // return           ?
                    SetPaidORNotByChance(0.9998, ref newRent);
                    break;

                // Status == Confirmed
                case 420660001: 
                    SetPaidORNotByChance(0.9, ref newRent);
                    break;
            }

        }
    }
}
