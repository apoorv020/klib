/* WebsiteBackend.cs -- The web interface
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
using System.Data.SqlClient;

using Public;

namespace ResourceWebsite
{

    class WebsiteBackend
    {
        private DBHelper dbHandle;
        private AWSHelper awsHandle;

        public WebsiteBackend(DBHelper dbHandle, AWSHelper awsHandle)
        {
            this.awsHandle = awsHandle;
            this.dbHandle = dbHandle;
        }

        public Public.DBHelper.Book[] searchDBForBook(string Title, string Author)
        {
            var bookReturned = dbHandle.SearchBook(Title);
            Public.DBHelper.Book[] retValue = new Public.DBHelper.Book[] { bookReturned };
            return retValue;
        }

    }
}