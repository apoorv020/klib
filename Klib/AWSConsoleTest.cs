/* Using the AWS Product Advertising API, Main()
 * looks up a given book + author combination.
 * Returns a list that can be injected into a database
*/

using System;
using System.Text;
using System.Net;
using System.ServiceModel;
using Klib.AWS.WSDL;

namespace AmazonPAapi
{
    class AWSlookup
    {
        // TODO: Move these fields out to a global configuration file
        private const string MY_AWS_ACCESS_KEY_ID = "AKIAI5X6IPJSICDCZB3Q";
        private const string MY_AWS_SECRET_KEY = "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";
        
        // Currently, this application uses SOAP because WCF only supports SOAP
        private const string ENDPOINT = "https://webservices.amazon.com/onca/soap?Service=AWSECommerceService";
        private const string NAMESPACE = "http://security.amazonaws.com/doc/2007-01-01/";

        // Convert this application to REST after the feature is integrated into WCF
        // private const string ENDPOINT = "https://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService";
        // private const string NAMESPACE = "http://webservices.amazon.com/AWSECommerceService/2009-03-31";

        public static void Main()
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = int.MaxValue;
            AWSECommerceServicePortTypeClient client = new AWSECommerceServicePortTypeClient(binding,
                                                                                             new EndpointAddress(ENDPOINT));

            client.ChannelFactory.Endpoint.Behaviors.Add(new AWSHelper.AmazonSigningEndpointBehavior(MY_AWS_ACCESS_KEY_ID, MY_AWS_SECRET_KEY, NAMESPACE));

            ItemSearchRequest request = new ItemSearchRequest();
            request.SearchIndex = "Books";
            request.Title = "Harry Potter and the Chamber of Secrets";
            request.ResponseGroup = new string[] { "Medium" };

            ItemSearch itemSearch = new ItemSearch();
            itemSearch.Request = new ItemSearchRequest[] { request };
            itemSearch.AWSAccessKeyId = MY_AWS_ACCESS_KEY_ID;

            // issue the ItemSearch request
            ItemSearchResponse response	= client.ItemSearch(itemSearch);

            foreach (var item in response.Items[0].Item)
            {
                Console.WriteLine("{0}  {1}", item.ItemAttributes.Title, item.ItemAttributes.ISBN);
                /* Other useful fields
                 * item.ItemAttributes.Author;
                 * item.DetailPageURL;
                 * item.SmallImage;
                 * item.SalesRank;
                */
            }
            Console.ReadKey();
        }
    }
}