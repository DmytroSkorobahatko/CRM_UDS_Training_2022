using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace MyPlugins
{
    public class PreventCreation : IPlugin
    {
        
        public void Execute(IServiceProvider serviceProvider)
        {

            // Extract the tracing service for use in debugging sandboxed plug-ins.  
            // If you are not registering the plug-in in the sandbox, then you do  
            // not have to add any tracing service related code.  
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference which you will need for  
            // web service calls.  
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);



            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];


                
                if (entity.Contains("cr4de_customer") && entity.GetAttributeValue<OptionSetValue>("statuscode").Value == 420660002)
                {

                    var query = new QueryExpression("cr4de_rent")
                    {
                        ColumnSet = new ColumnSet("cr4de_customer", "statuscode"),
                        NoLock = true,
                        Criteria = {
                        Conditions = {
                            new ConditionExpression("cr4de_customer", ConditionOperator.Equal, entity.GetAttributeValue<EntityReference>("cr4de_customer").Id),
                            new ConditionExpression("statuscode", ConditionOperator.Equal, 420660002)
                        }
                        }
                    };

                    var collection = service.RetrieveMultiple(query);

                    if (collection.Entities.Count > 10)
                        throw new InvalidPluginExecutionException("Prevent creation of more than 10 rents in status Renting per one owner");
                }
                
            }
        }
    }
}
