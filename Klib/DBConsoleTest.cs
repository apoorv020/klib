using System;
using System.Data.SqlClient;
using Klib;

class DBConsoleTest
{
    public static void Main1()
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        /* TODO: Move this out into a configuration file */
        builder.Password = "klib";
        builder.UserID = "klib";
        builder.DataSource = ".\\SQLEXPRESS";

        string connectionPath = builder.ConnectionString;
        try
        {
            var db = new KDbDataContext(connectionPath);
            db.Log = Console.Out;

            Book b2 = new Book { Author = "Orwell", Title = "Animal Farm2" };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        Console.ReadKey();
    }
}
