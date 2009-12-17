/* A collection of functions to help the user
 * interact with the DB
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text;
using System.Data.SqlClient;

namespace Klib
{
    public class DBHelper
    {
        protected KDbDataContext db;

        public DBHelper(ConnectionString connectionPath)
        {
            // Construct the helper with the connectionString
            db = new KDbDataContext(connectionPath);
        }

        // ===  Get methods   ===
        // ===     BEGIN      ===
        
        public Book getBookByUID(int UID)
        {
            return db.Books
                .Where(book => book.UID == UID)
                .First();
        }
        public Person getPersonByUID(int UID)
        {
            return db.Persons
                .Where(person => person.UID == UID)
                .First();
        }
        public Person getBorrowerByUID(int UID)
        {
            try
            {
                return db.RelationshipMappers
                    .Where(person => person.UID == UID)
                    .First();
            }
            catch
            {
                return null;
            }
        }
        public Person searchPerson(string searchString)
        {
            try
            {
                return db.Persons
                    .Where(person => person.FirstName.Equals(searchString))
                    .First();
            }
            catch
            {
                return null;
            }
        }
        public Book searchBook(string searchString)
        {
            try
            {
                return db.Books
                    .Where(book => book.Title.Equals(searchString))
                    .First();
            }
            catch
            {
                return null;
            }
        }
        public int relationshipCount(int UID)
        {
            // Given a book UID, returns the number of people related to it
            return db.RelationshipMappers
                .Where(mapper => mapper.Resource == UID)
                .Count();
        }
        
        // ===  Get methods   ===
        // ===      END       ===


        // ===  Write methods ===
        // ===     BEGIN      ===

        public bool borrowBook(int bookUID, int personUID)
        {
            // TODO: Throw and catch error objects instead of a non-descriptive bool
            if (relationshipCount(bookUID) >= 1)
                return false;
            var newRelation = new RelationshipMapper { UID = bookUID, Person = personUID };
            db.RelationshipMappers.InsertOnSubmit(newRelation);
            db.SubmitChanges();
            return true;
        }
        public bool returnBook(int bookUID, int personUID)
        {
            // TODO: Throw and catch error objects instead of a non-descriptive bool
            var mapBorrower = db.RelationshipMappers
                .Where(mapper => mapper.Resource == bookUID)
                .First();
            if (mapBorrower.Person != personUID)
                return false;
            db.RelationshipMappers.DeleteOnSubmit(mapBorrower);
            db.SubmitChanges();
            return true;
        }
        public bool writeAWSInfo(int bookUID, string Title, string Author, string ISBN10, string ISBN13, string URL)
        {
            // Given a bookUID and corresponding information from AWS, writes information to DB
            // TODO: Encapsulate this two-step process in a transaction

            // STEP 1: Write the AWSInfo
            var newAWSInfo = new AWSInfo { Title = Title, Author = Author, ISBN10 = ISBN10, ISBN13 = ISBN13, URL = URL };
            db.AWSInfos.InsertOnSubmit(newAWSInfo);
            db.SubmitChanges();

            // STEP 2: Write a BookMapper entry
            // TODO: Catch FKey constraint violation error
            var newMapper = new BookMapper { AWSInfo = newAWSInfo.UID, Book = bookUID };
            db.BookMappers.InsertOnSubmit(newMapper);
            db.SubmitChanges();
            return true;
        }

        // ===  Write methods ===
        // ===      END       ===
    }
}