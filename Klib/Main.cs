/* Main.cs -- The binder for the complete project
 * 
 * This file is part of Klib (http://github.com/artagnon/klib)
 * Copyright (C) 2009 Ramkumar Ramachandra <artagnon@gmail.com>
 * Copyright (C) 2009 Aproorv Gupta <apoorv020@gmail.com>
 * 
 * This work is licensed Public Domain.
 * To view a copy of the public domain certification,
 * visit http://creativecommons.org/licenses/publicdomain/ or
 * send a letter to Creative Commons, 171 Second Street,
 * Suite 300, San Francisco, California, 94105, USA.
 */

using System;
/* The main interface to the program
 * Import Public and you're good to go.
 * Do NOT import Klib
 */

using Public;

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

        private const string BOOK = "Books";
        
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
            var results = awsHandle.Search("Harry Potter and the Chamber of Secrets", BOOK);
            foreach (var result in results)
            {
                Console.Out.WriteLine(result.ItemAttributes.ISBN);
            }
            Console.ReadKey();
        }
    }
}