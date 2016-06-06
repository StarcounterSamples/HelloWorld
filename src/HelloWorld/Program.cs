﻿using Starcounter;

namespace HelloWorld
{
    [Database]
    public class Person
    {
        public string FirstName;
        public string LastName;
    }

    class Program
    {
        static void Main()
        {
            var anyone = Db.SQL<Person>("SELECT p FROM Person p").First;
            if (anyone == null)
                Db.Transact(() =>
                {
                    new Person
                    {
                        FirstName = "John",
                        LastName = "Doe"
                    };
                });

            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/HelloWorld", () =>
            {
                return Db.Scope(() =>
                {
                    var person = Db.SQL<Person>("SELECT p FROM Person p").First;
                    var json = new PersonJson()
                    {
                        Data = person
                    };
                    json.Session = new Session(SessionOptions.PatchVersioning);
                    return json;
                });
            });
        }
    }
}