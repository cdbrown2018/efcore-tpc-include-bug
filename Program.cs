using Microsoft.EntityFrameworkCore;
using EFCoreTpcIncludeBug;

// This program demonstrates the bug when using .Include() on a polymorphic 
// property configured with TPC (Table-Per-Concrete) inheritance

try
{
    using (var context = new AppDbContext())
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Add test data
        var dog = new Dog { Name = "Buddy", Breed = "Golden Retriever" };
        var cat = new Cat { Name = "Whiskers", Color = "Orange" };
        var person = new Person { Name = "John", Pet = dog };

        context.Animals.Add(dog);
        context.Animals.Add(cat);
        context.People.Add(person);
        context.SaveChanges();

        Console.WriteLine("Test data created successfully.");
    }

    using (var context = new AppDbContext())
    {
        // This query should work but throws an "invalid shaper" exception
        // when trying to .Include() the polymorphic Pet navigation property
        Console.WriteLine("\nAttempting query with .Include() on polymorphic property...");
        
        var people = context.People
            .Include(p => p.Pet)  // BUG: This causes an invalid shaper exception
            .ToList();

        Console.WriteLine("Query succeeded!");
        foreach (var person in people)
        {
            Console.WriteLine($"Person: {person.Name}, Pet: {person.Pet?.Name}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.GetType().Name}");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"\nStack Trace:\n{ex.StackTrace}");
}
