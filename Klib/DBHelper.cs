/* DBHelper.cs -- DB helper functions and classes
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
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;
using System.Data.SqlClient;
// Do NOT import Klib

namespace Public
{
    public class DBHelper
    {
        private static Klib.KDbDataContext db;

        public class AWSInfo
        {
            // Klib.AWSInfo is not exposed; it contains auto-generated LINQ
            // This Public.AWSInfo automatically maps to Klib.AWSInfo
            public int UID;
            public string Title;
            public string Author;
            public string ISBN10;
            public string ISBN13;
            public string URL;

            public AWSInfo(Klib.AWSInfo awsInfo)
            {
                // Constructs Public.AWSawsInfo from Klib.AWSawsInfo
                this.UID = awsInfo.UID;
                this.Title = awsInfo.Title;
                this.Author = awsInfo.Author;
                this.ISBN10 = awsInfo.ISBN10;
                this.ISBN13 = awsInfo.ISBN13;
                this.URL = awsInfo.URL;
            }
        }
        public class Book
        {
            public int UID;
            public string Title;
            public string Author;
            public string ISBN10;
            public string ISBN13;
            public int Owner;           // This field is not in Klib.Book
            public bool UniqueMap;

            public Book(Klib.Book book)
            {
                // Constructs Public.Book from Klib.Book
                this.UID = book.UID;
                this.Title = book.Title;
                this.Author = book.Author;
                this.ISBN10 = book.ISBN10;
                this.ISBN13 = book.ISBN13;
                this.Owner = book.Owner;
                this.UniqueMap = book.UniqueMap;
            }

            // BEGIN top level helpers
            // TODO: Interface with RFID
            public bool Borrow(Person person)
            {
                // TODO: Throw and catch error objects instead of a non-descriptive bool
                try
                {
                    Write(this, person, true);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            public bool Return(Person person)
            {
                // TODO: Throw and catch error objects instead of a non-descriptive bool
                try
                {
                    Write(this, person, false);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            // END top level helpers

        }
        public class Person
        {
            // Klib.Person is not exposed; it contains auto-generated LINQ
            // This Public.Person automatically maps to Klib.Person
            public int UID;
            public string FirstName;
            public string LastName;
            public string Location;

            public Person(Klib.Person person)
            {
                // Constructs Public.Person from Klib.Person
                this.UID = person.UID;
                this.FirstName = person.FirstName;
                this.LastName = person.LastName;
                this.Location = person.Location;
            }
        }
        public DBHelper(string MY_SQL_USERNAME, string MY_SQL_PASSWORD, string MY_SQL_DATASOURCE)
        {
            // Constructor for DBHelper
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.UserID = MY_SQL_USERNAME;
            builder.Password = MY_SQL_PASSWORD;
            builder.DataSource = MY_SQL_DATASOURCE;
            db = new Klib.KDbDataContext(builder.ConnectionString);
        }

        // BEGIN Static Builders
        private static Klib.Book Build(Book book)
        {
            var builtBook = new Klib.Book();
            builtBook.UID = book.UID;
            builtBook.Title = book.Title;
            builtBook.Author = book.Author;
            builtBook.ISBN10 = book.ISBN10;
            builtBook.ISBN13 = book.ISBN13;
            builtBook.UniqueMap = book.UniqueMap;
            return builtBook;
        }
        private static Klib.Person Build(Person person)
        {
            var builtPerson = new Klib.Person();
            builtPerson.UID = person.UID;
            builtPerson.FirstName = person.FirstName;
            builtPerson.LastName = person.LastName;
            builtPerson.Location = person.Location;
            return builtPerson;
        }
        private static Klib.AWSInfo Build(AWSInfo awsInfo)
        {
            var builtInfo = new Klib.AWSInfo();
            builtInfo.UID = awsInfo.UID;
            builtInfo.Title = awsInfo.Title;
            builtInfo.Author = awsInfo.Author;
            builtInfo.ISBN10 = awsInfo.ISBN10;
            builtInfo.ISBN13 = awsInfo.ISBN13;
            builtInfo.URL = awsInfo.URL;
            return builtInfo;
        }
        // END Static Builders

        // BEGIN Static Writers
        private static void Write(Book book)
        {
            var newBook = Build(book);
            var newResource = new Klib.Resource();
            newResource.Owner = book.Owner;
            db.Resources.InsertOnSubmit(newResource);
            db.SubmitChanges();
            newBook.UID = newResource.UID;
            db.Books.InsertOnSubmit(newBook);
            db.SubmitChanges();
        }
        private static void Write(Person person)
        {
            var newPerson = Build(person);
            db.Persons.InsertOnSubmit(newPerson);
            db.SubmitChanges();
        }
        private static void Write(Book book, AWSInfo awsInfo)
        {
            // For AWSInfo and associated Book
            var newAWSInfo = Build(awsInfo);
            var newMapper = new Klib.BookMapper();
            newMapper.AWSInfo = awsInfo.UID;
            newMapper.Book = book.UID;
            db.AWSInfos.InsertOnSubmit(newAWSInfo);
            db.BookMappers.InsertOnSubmit(newMapper);
            db.SubmitChanges();
        }
        private static void Write(Book book, Person person, bool borrowFlag)
        {
            // For borrow and return
            var newResource = new Klib.Resource();
            var newBook = Build(book);
            var newPerson = Build(person);
            newResource.Owner = newPerson.UID;
            db.Resources.InsertOnSubmit(newResource);
            db.SubmitChanges();
            newBook.UID = newResource.UID;

            var newMapper = new Klib.RelationshipMapper { Person = newPerson.UID, Resource =  newResource.UID};
            if (borrowFlag)
                db.RelationshipMappers.InsertOnSubmit(newMapper);
            else
                db.RelationshipMappers.DeleteOnSubmit(newMapper);
            db.SubmitChanges();
        }
        // END Static Writers

        // BEGIN Utility functions
        public Person SearchPerson(string FirstName, string LastName)
        {
            try
            {
                var thisPerson = db.Persons
                    .Where(person => person.FirstName.Equals(FirstName))
                    .Where(person => person.LastName.Equals(LastName))
                    .First();
                return new Person(thisPerson);
            }
            catch
            {
                return null;
            }
        }
        public Book SearchBook(string searchString)
        {
            try
            {
                var thisBook = db.Books
                    .Where(book => book.Title.Equals(searchString))
                    .First();
                return new Book(thisBook);
            }
            catch
            {
                return null;
            }
        }
        // END Utility functions

    }
}