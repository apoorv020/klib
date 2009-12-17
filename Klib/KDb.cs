// Custom functionality and overrides for LINQ <-> SQL layer
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