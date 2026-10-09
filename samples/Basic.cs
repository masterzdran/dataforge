#:project ../src/DataForger
using System;
using DataForger;
var person = Forge.ForCountry(Country.PT)
    .WithSeed(1234)
    .Create<Person>();
Console.WriteLine(person.FullName);
Console.WriteLine(person.Email);
