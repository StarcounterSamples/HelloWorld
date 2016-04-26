﻿using Starcounter;
using System.Linq;

namespace HelloWorld
{
    [Database]
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public QueryResultRows<Expense> Expenses => 
            Db.SQL<Expense>("SELECT e FROM Expense e WHERE e.Spender = ?", this);
        public decimal CurrentBalance =>
            Db.SQL<decimal>("SELECT SUM(e.Amount) FROM Expense e WHERE e.Spender = ?", this).First;
    }

    [Database]
    public class Expense
    {
        public Person Spender { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    class Program
    {
        static void Main()
        {
            Db.Transact(() =>
            {
                var person = Db.SQL<Person>("SELECT p FROM Person p").FirstOrDefault();
                if (person == null)
                {
                    new Person
                    {
                        FirstName = "John",
                        LastName = "Doe"
                    };
                }
            });

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/HelloWorld", () =>
            {
                return Db.Scope(() =>
                {
                    Session.Ensure();

                    var person = Db.SQL<Person>("SELECT p FROM Person p").FirstOrDefault();
                    return new PersonJson { Data = person };
                });
            });
        }
    }
}