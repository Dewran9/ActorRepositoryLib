using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using ActorRepositoryLib;
using System.Collections.Generic;

namespace ActorRepositoryLibTest;

[TestClass]
public class ActorRepositoryTest
{
    private static ActorRepository SeedRepo()
    {
        var repo = new ActorRepository();
        repo.Add(new Actor(0, "Tom Hanks", 1956, "USA"));
        repo.Add(new Actor(0, "Meryl Streep", 1949, "USA"));
        repo.Add(new Actor(0, "Idris Elba", 1972, "UK"));
        repo.Add(new Actor(0, "Cate Blanchett", 1969, "Australia"));
        repo.Add(new Actor(0, "Joaquin Phoenix", 1974, "USA"));
        return repo;
    }

    [TestMethod]
    public void Add_Assigns_Id_Sequentially()
    {
        var repo = new ActorRepository();
        var a1 = repo.Add(new Actor(0, "John Doe", 1990, "USA"));
        var a2 = repo.Add(new Actor(0, "John Doe", 1991, "USA"));
        Assert.AreEqual(1, a1.Id);
        Assert.AreEqual(2, a2.Id);
    }

    [TestMethod]
    public void Add_Null_Throws()
    {
        var repo = new ActorRepository();
        var ex = Assert.ThrowsException<ArgumentNullException>(() => repo.Add(null!));
        StringAssert.Contains(ex.Message, "Actor cannot be null");
    }

    [TestMethod]
    public void Get_Returns_All()
    {
        var repo = SeedRepo();
        var allActors = repo.Get().ToList();
        Assert.AreEqual(5, allActors.Count);
        CollectionAssert.AreEquivalent(
            new[] { "Tom Hanks", "Meryl Streep", "Idris Elba", "Cate Blanchett", "Joaquin Phoenix" },
            allActors.Select(a => a.Name).ToArray());
    }

    [TestMethod]
    public void GetById_Found_And_NotFound()
    {
        var repo = SeedRepo();
        var first = repo.Get().First();
        var found = repo.GetById(first.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual(first.Name, found!.Name);
        
        var none = repo.GetById(999);
        Assert.IsNull(none);
    }

    [TestMethod]
    public void Delete_Removes_And_Returns_Deleted_Actor()
    {
        var repo = SeedRepo();
        var any = repo.Get().First();
        var deleted = repo.Delete(any.Id);

        Assert.IsNotNull(deleted);
        Assert.AreEqual(any.Id, deleted!.Id);

        var again = repo.Delete(any.Id);
        Assert.IsNull(again);

        Assert.AreEqual(4, repo.Get().Count());
    }

    [TestMethod]
    public void Update_Updates_DataFields()
    {
        var repo = SeedRepo();
        var target = repo.Get().First();
        var updatedData = new Actor(0, "Mowgli", 2000, "Denmark");

        Assert.IsNotNull(repo.Update(target.Id, updatedData));
        Assert.AreEqual("Mowgli", updatedData!.Name);
        Assert.AreEqual(2000, updatedData.Birthyear);
        Assert.AreEqual("Denmark", updatedData.Country);
    }

    [TestMethod]
    public void Update_NotFound_Returns_Null()
    {
        var repo = SeedRepo();
        var updatedData = new Actor(0, "Mowgli", 2000, "Denmark");
        var result = repo.Update(999, updatedData);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Get_BirthYear_Before_Filters()
    {
        var repo = SeedRepo();
        var before1970 = repo.Get(1970).ToList();

        Assert.IsTrue(before1970.All(a => a.Birthyear < 1970));
        Assert.IsTrue(before1970.Select(a => a.Name).Contains("Meryl Streep")); //1949
        Assert.IsFalse(before1970.Select(a => a.Name).Contains("Joaquin Phoenix")); //1974
    }

    [TestMethod]
    public void Get_BirthYear_Before_And_After_Filters()
    {
        var repo = SeedRepo();
        var between1950And1973 = repo.Get(1973, 1950).ToList();
        
        Assert.IsTrue(between1950And1973.All(a => a.Birthyear < 1973 && a.Birthyear > 1950));
        CollectionAssert.AreEquivalent(
            new[] { "Tom Hanks", "Cate Blanchett", "Idris Elba" }, // 1956, 1969, 1972
            between1950And1973.Select(a => a.Name).ToArray());
    }

    [TestMethod]
    public void GetByName_NullOrWhiteSpace_Returns_All()
    {
        var repo = SeedRepo();
        var res1 = repo.GetByName(null).ToList();
        var res2 = repo.GetByName("").ToList();
        var res3 = repo.GetByName("   ").ToList();

        Assert.AreEqual(5, res1.Count);
        Assert.AreEqual(5, res2.Count);
        Assert.AreEqual(5, res3.Count);
    }

    [TestMethod]
    public void GetByName_Is_CaseInsensitive_And_Trims()
    {
        var repo = SeedRepo();

        var res1 = repo.GetByName("  tom ").ToList();
        Assert.AreEqual(1, res1.Count);
        Assert.AreEqual("Tom Hanks", res1[0].Name);

        var res2 = repo.GetByName("PHO").Select(a => a.Name).ToList();
        CollectionAssert.Contains(res2, "Joaquin Phoenix");

    }

    [TestMethod]
    public void Get_With_Sorting_By_Name_Asc_And_Desc()
    {
        var repo = SeedRepo();
        var asc = repo.Get(null, null, null, "name", true).Select(a => a.Name).ToList();
        var desc = repo.Get(null, null, null, "name", false).Select(a => a.Name).ToList();

        var ascExpected = asc.OrderBy(x => x).ToList();
        var descExpected = desc.OrderByDescending(x => x).ToList();

        CollectionAssert.AreEqual(ascExpected, asc);
        CollectionAssert.AreEqual(descExpected, desc);
    }

    [TestMethod]
    public void Get_Default_Sorting_Is_By_Id()
    {
        var repo = SeedRepo();

        // Brug en ukendt sortBy -> skal falde tilbage på Id
        var asc = repo.Get(null, null, null, "unknown", true).Select(a => a.Id).ToList();
        var desc = repo.Get(null, null, null, "unknown", false).Select(a => a.Id).ToList();

        var ascExpected = asc.OrderBy(x => x).ToList();
        var descExpected = desc.OrderByDescending(x => x).ToList();

        CollectionAssert.AreEqual(ascExpected, asc);
        CollectionAssert.AreEqual(descExpected, desc);
    }

    [TestMethod]
    public void Add_Delete_Do_Not_Reuse_Ids()
    {
        var repo = new ActorRepository(); // ny repo, ingen seed, da vi skal teste Id tildeling fra 1 - hvis vi ik' tilføjer et nyt repo, så vil id'et starter fra 6, da vi allerede har 5 i seed repo.
        var a1 = repo.Add(new Actor(0, "New Actor1", 1990, "USA"));
        var a2 = repo.Add(new Actor(0, "New Actor2", 1991, "USA"));
        repo.Delete(a1.Id);
        var a3 = repo.Add(new Actor(0, "New Actor3", 1992, "USA"));

        Assert.AreEqual(1, a1.Id);
        Assert.AreEqual(2, a2.Id);
        Assert.AreEqual(3,a3.Id);
    }

    [TestMethod]
    public void Get_Combined_Filters_And_Sort()
    {
        var repo = SeedRepo();

        // Find navne der indeholder 'a', født før 1975, efter 1940, sorteret efter navn stigende

        var res = repo.Get(1975, 1940, "a", "name", true).ToList();

        // Forvent mindst "Cate Blanchett" og "Joaquin Phoenix"
        Assert.IsTrue(res.Any(a => a.Name == "Cate Blanchett"));
        Assert.IsTrue(res.Any(a => a.Name == "Joaquin Phoenix"));

        // Tjek sortering (navn stigende)
        var names = res.Select(a => a.Name).ToList();
        var expected = names.OrderBy(n => n).ToList();
        CollectionAssert.AreEqual(expected, names);

    }
}
