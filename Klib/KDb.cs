/* KDb.cs -- Custom functionality and overrides for
 * auto-generated LINQ to SQL mappers
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


namespace Klib
{
    public partial class Person
    {
        public override string ToString()
        {
            return string.Format("Employee (Id:{0},Name:{1},Location:{2})",
                                 this._UID,
                                 this._FirstName,
                                 this._Location);
        }
    }
    public partial class Book
    {
        public override string ToString()
        {
            return string.Format("Book (Id:{0},Title:{1} by Author:{2},Owner:{3})",
                                 this._UID,
                                 this.Title,
                                 this.Author,
                                 this.Owner);
        }
    }
}