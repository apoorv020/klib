/* The binder for the complete project */

using System;
using System.Data.SqlClient;

using Public;
// Do NOT import Klib;
// It contains too much auto-generated plumbing code

namespace Program
{
    class Program
    {
        // TODO: Move these fields out to a global configuration file
        private const string MY_AWS_ACCESS_KEY_ID = "AKIAI5X6IPJSICDCZB3Q";
        private const string MY_AWS_SECRET_KEY = "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";
        private const string MY_SQL_USERNAME = "klib";
        private const string MY_SQL_PASSWORD = "klib";
        private const string MY_SQL_DATASOURCE = ".\\SQLEXPRESS";
        
        // Currently, this application uses SOAP because WCF only supports SOAP
        private const string ENDPOINT = "https://webservices.amazon.com/onca/soap?Service=AWSECommerceService";
        private const string NAMESPACE = "http://security.amazonaws.com/doc/2007-01-01/";

        // Convert this application to REST after the feature is integrated into WCF
        // private const string ENDPOINT = "https://ecs.amazonaws.com/onca/xml?Service=AWSECommerceService";
        // private const string NAMESPACE = "http://webservices.amazon.com/AWSECommerceService/2009-03-31";

        public static void Main()
        {
            // Instantiate the two helpers
            var dbHandle = new DBHelper(MY_SQL_USERNAME, MY_SQL_PASSWORD, MY_SQL_DATASOURCE);
            var awsHandle = new AWSHelper(ENDPOINT, MY_AWS_ACCESS_KEY_ID, MY_AWS_SECRET_KEY, NAMESPACE);
            var results = awsHandle.Search("Harry Potter and the Chamber of Secrets");
            foreach (var result in results)
            {
                Console.Out.WriteLine(result.ItemAttributes.ISBN);
            }
            Console.ReadKey();
        }
    }
}