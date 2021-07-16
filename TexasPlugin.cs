using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace TexasPlugin
{
    public class TexasPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            /* throw new NotImplementedException();*/


            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    /// when creating a new account with State != "Texas" or "tx" (empty or non-tx state) -- block it!
                    //need to address empty state field
                    //check in entity is account
                    //check is state is null, throw new InvalidPluginExecutionException("You must include a state to save an account")

                    tracingService.Trace("beginning Try- {0}", DateTime.Now.ToLongTimeString());

                    if(context.MessageName == "Create")
                    {
                        tracingService.Trace("context.MessageName = Create - {0}", DateTime.Now.ToLongTimeString());
                        string state = entity["address1_stateorprovince"].ToString().ToLower();

                        if (state == "texas" || state == "tx")
                        {
                            tracingService.Trace("state is texas - {0}", DateTime.Now.ToLongTimeString());
                            return;
                            } else
                            {
                                throw new InvalidPluginExecutionException("You can only save Accounts with the state of Texas or TX");
                            }
                         }

                    }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in TexasPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("TexasPlugin: {0} - {1}", ex.ToString(), DateTime.Now.ToLongTimeString());
                    throw;
                }
            }

        }


    }
}
