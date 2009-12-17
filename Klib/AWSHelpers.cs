/* These endpoint helpers are used by ItemLookup.cs
 * for signing requests. AWS does not allow unsigned requests.
*/

using System;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Klib.AWS.WSDL;

namespace Klib
{
    public class AWSHelper
    {
        private ItemSearchRequest request;
        private AWSECommerceServicePortTypeClient client;

        public AWSHelper(string ENDPOINT, string MY_AWS_ACCESS_KEY_ID, string MY_AWS_SECRET_KEY, string NAMESPACE)
        {
            // Constructor to populate responseItems
            // Accepts various standard AWS parameters and search string
            
            // Basic HTTP binding
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = int.MaxValue;

            // Client
            client = new AWSECommerceServicePortTypeClient(
                binding, new EndpointAddress(ENDPOINT));
            client.ChannelFactory.Endpoint.Behaviors.Add(
                new AmazonSigningEndpointBehavior(MY_AWS_ACCESS_KEY_ID, MY_AWS_SECRET_KEY, NAMESPACE));

            // ItemSearchREquest
            request = new ItemSearchRequest();
            request.SearchIndex = "Books";
            request.ResponseGroup = new string[] { "Medium" };

            ItemSearch itemSearch = new ItemSearch();
            itemSearch.Request = new ItemSearchRequest[] { request };
            itemSearch.AWSAccessKeyId = MY_AWS_ACCESS_KEY_ID;

            // issue the ItemSearch request
        }
        public Item[] Search(string searchTitle)
        {
            // Exposed to programmer
            request.Title = searchTitle;
            var response = client.ItemSearch(itemSearch);
            return response.Items[0].Item;
        }
        private class AmazonSigningEndpointBehavior : IEndpointBehavior
        {
            private string accessKeyId;
            private string secretKey;
            private string awsNamespace;

            public AmazonSigningEndpointBehavior(string accessKeyId, string secretKey, string awsNamespace)
            {
                this.accessKeyId = accessKeyId;
                this.secretKey = secretKey;
                this.awsNamespace = awsNamespace;
            }
            public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MessageInspectors.Add(new AmazonSigningMessageInspector(accessKeyId, secretKey, awsNamespace));
            }

            public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher) { return; }
            public void Validate(ServiceEndpoint serviceEndpoint) { return; }
            public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters) { return; }
        }

        private class AWSheader : MessageHeader
        {
            private string name;
            private string value;
            private string awsNamespace;

            public AWSheader(string name, string value, string awsNamespace)
            {
                this.name = name;
                this.value = value;
                this.awsNamespace = awsNamespace;
            }
            public override string Name { get { return name; } }
            public override string Namespace { get { return awsNamespace; } }

            protected override void OnWriteHeaderContents(XmlDictionaryWriter xmlDictionaryWriter, MessageVersion messageVersion)
            {
                xmlDictionaryWriter.WriteString(value);
            }
        }

        private class AmazonSigningMessageInspector : IClientMessageInspector
        {
            private string accessKeyId;
            private string secretKey;
            private string awsNamespace;

            public AmazonSigningMessageInspector(string accessKeyId, string secretKey, string awsNamespace)
            {
                this.accessKeyId = accessKeyId;
                this.secretKey = secretKey;
                this.awsNamespace = awsNamespace;
            }

            public object BeforeSendRequest(ref Message request, IClientChannel channel)
            {
                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                string operationString = Regex.Match(request.Headers.Action, "[^/]+$").ToString() + timestamp;

                // sign the data
                HMAC secretSigner = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                byte[] hashBytes = secretSigner.ComputeHash(Encoding.UTF8.GetBytes(operationString));
                string signature = Convert.ToBase64String(hashBytes);

                // add the signature information to the request headers
                request.Headers.Add(new AWSheader("AWSAccessKeyId", accessKeyId, awsNamespace));
                request.Headers.Add(new AWSheader("Timestamp", timestamp, awsNamespace));
                request.Headers.Add(new AWSheader("Signature", signature, awsNamespace));
                return null;
            }

            public void AfterReceiveReply(ref Message reply, object correlationState) { }
        }
    }
}
