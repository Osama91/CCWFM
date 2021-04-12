// This is an auto-generated file to enable WCF faults to reach Silverlight clients.

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace CCWFM.Web.Service
{
    public class SilverlightFaultBehavior : Attribute, IServiceBehavior
    {
        private class SilverlightFaultEndpointBehavior : IEndpointBehavior
        {
            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new SilverlightFaultMessageInspector());
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }

            private class SilverlightFaultMessageInspector : IDispatchMessageInspector
            {
                Stopwatch sw = new Stopwatch(); DateTime start;
                public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
                {
                    if (Global.LogTransactionsStatus)
                    {
                        start = DateTime.Now;
                        sw = new Stopwatch();
                        sw.Start();
                    }
                    return null;
                }


                public void BeforeSendReply(ref Message reply, object correlationState)
                {
                    if ((reply != null) && reply.IsFault)
                    {
                        HttpResponseMessageProperty property = new HttpResponseMessageProperty();
                        property.StatusCode = HttpStatusCode.OK;
                        reply.Properties[HttpResponseMessageProperty.Name] = property;
                    }
                    if (Global.LogTransactionsStatus)
                    {
                        sw.Stop();
                        var elapsedMilliseconds = sw.ElapsedMilliseconds;
                        if (elapsedMilliseconds > 1000 || reply.IsFault) //||OperationContext.Current.IncomingMessageHeaders.Action.Contains("BankDeposit"))
                        {
                            Logger logger = LogManager.GetLogger(Global.TransactionsLoggerName);
                            string message = string.Empty;
                            try
                            {
                                message = JsonConvert.SerializeObject(new
                                {
                                    // Context = HttpContext.Current,
                                    OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri.AbsoluteUri,

                                }, new JsonSerializerSettings()
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                                    ContractResolver = new CustomResolver(),
                                });
                            }
                            catch (Exception ex) { }
                            LogEventInfo theEvent = new LogEventInfo(
                                LogLevel.Info, Global.TransactionsLoggerName, message);
                            theEvent.Properties["Milliseconds"] = elapsedMilliseconds;
                            theEvent.Properties["Method"] = OperationContext.Current.IncomingMessageHeaders.Action;
                            theEvent.Properties["IsFault"] = reply.IsFault;

                            logger.Log(theEvent);
                        }
                    }
                }

            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints)
            {
                endpoint.Behaviors.Add(new SilverlightFaultEndpointBehavior());
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
    #region Faults
    class CustomResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = instance =>
            {
                try
                {
                    PropertyInfo prop = (PropertyInfo)member;
                    if (prop.CanRead)
                    {
                        try
                        {
                            prop.GetValue(instance, null);
                            return true;
                        }
                        catch (Exception) { return false; }
                    }
                }
                catch { }
                return false;
            };
            return property;
        }
    }
    #endregion
}