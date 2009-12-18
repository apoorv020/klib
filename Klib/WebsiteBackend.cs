﻿using System;
using System.Data.SqlClient;

using Public;


namespace ResourceWebsite
{

    class WebsiteBackend
    {
        private DBHelper dbHandle;
        private AWSHelper awsHandle;
        private const string bookSearchType = "Books";

        public WebsiteBackend(DBHelper dbHandle, AWSHelper awsHandle)
        {
            this.awsHandle = awsHandle;
            this.dbHandle = dbHandle;
        }


        /// <summary>
        ///     Will return an array of books in the database based on search parameters
        /// </summary>
        /// <param name="Title">Title of book to search</param>
        /// <param name="Author">Currently Unused, Author of book to search for</param>
        /// <returns>Possible matches, null OW</returns>
        public Public.DBHelper.Book[] searchDBForBook(string Title, string Author)
        {
            var bookReturned = dbHandle.SearchBook(Title);
            if (bookReturned == null)
                return null;
            Public.DBHelper.Book[] retValue = new Public.DBHelper.Book[] { bookReturned };
            return retValue;
        }
        /// <summary>
        ///     Returns an array of Online books that match the current book
        /// </summary>
        /// <param name="book">A book object present in database. Obtain by using searchDBForBook.</param>
        /// <returns> possible online matches, null if already matched</returns>
        public Klib.AWS.WSDL.Item[] searchOnlineMatches(DBHelper.Book book)
        {
            if(book.UniqueMap)
                return null; //Throw exception instead?
            return awsHandle.Search(book.Title, bookSearchType);
        }
        /// <summary>
        ///     Resolves specified book to the givenm search Result
        /// </summary>
        /// <param name="book">Book object to update</param>
        /// <param name="item">Online result whose attributes to use</param>
        /// <remarks>
        ///     Search for a book in DB, then get online results.
        ///     Match one of those results to the book.
        /// </remarks>
        /// <see cref="searchOnlineMatches"/>
        public void resolveBookWithOnlineMatch(DBHelper.Book book, Klib.AWS.WSDL.Item item)
        {
            if (book.UniqueMap)
                throw new InvalidOperationException("Book already mapped!");
            book.Author = "";
            foreach (var author in item.ItemAttributes.Author)
            {
                book.Author += author + ", ";
            }
            book.Title = item.ItemAttributes.Title;
            book.UniqueMap = true;
            if (item.ItemAttributes.ISBN.Length == 10)
                book.ISBN10 = item.ItemAttributes.ISBN;
            else if (item.ItemAttributes.ISBN.Length == 13)
                book.ISBN13 = item.ItemAttributes.ISBN;
            //TODO : add code to update the book in the database.

            //book.
            //Assert.;
        }

    }
}