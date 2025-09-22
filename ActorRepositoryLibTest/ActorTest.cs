using ActorRepositoryLib;

namespace ActorRepositoryLibTest;

[TestClass]
public class ActorTest
{
    [TestMethod]
    public void Actor_Validation_Works()
    {
        var a = new Actor(1, "John Doe", 1990, "USA");
        Assert.AreEqual("John Doe", a.Name);
        Assert.AreEqual(1990, a.Birthyear);
        Assert.AreEqual("USA", a.Country);
    }

    [DataTestMethod]
    [DataRow(-1)]
    [DataRow(-100)]
    public void Actor_Negative_Id_Throws(int negativeId)
    {
       var a = new Actor(0, "John Doe", 1990, "USA");
       var ex = Assert.ThrowsException<ArgumentException>(() => a.Id = negativeId);
       StringAssert.Contains(ex.Message, "Id must be non-negative");
    }

    [TestMethod]
    public void Actor_Positive_Set_Id_Works()
    {
        var a = new Actor(0, "John Doe", 1990, "USA");
        a.Id = 5;
        Assert.AreEqual(5, a.Id);
    }
    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Actor_NullOrEmpty_Name_Throws(string invalidName)
    {
        var a = new Actor(1, "John Doe", 1990, "USA");
        var ex = Assert.ThrowsException<ArgumentException>(() => a.Name = invalidName);
        StringAssert.Contains(ex.Message, "Name cannot be null or empty");
    }

    [TestMethod]
    public void Actor_Name_Exactly_4_Chars_Works()
    {
        var a = new Actor(2, "Thomas", 1990, "USA");
        a.Name = "John";
        Assert.AreEqual("John", a.Name);
    }

    [DataTestMethod]
    [DataRow(1819)]
    [DataRow(1500)]
    public void BirthYear_Below1820_Throws(int invalidYear)
    {
        var a = new Actor(1, "John Doe", 1990, "USA");
        var ex = Assert.ThrowsException<ArgumentException>(() => a.Birthyear = invalidYear);
        StringAssert.Contains(ex.Message, "Birthyear must be between 1820 and the current year");
    }

    [TestMethod]
    public void BirthYear_AboveCurrentYear_Throws()
    {
        var a = new Actor(1, "John Doe", 1990, "USA");
        var nextYear = DateTime.Now.Year + 1;
        var ex = Assert.ThrowsException<ArgumentException>(() => a.Birthyear = nextYear);
        StringAssert.Contains(ex.Message, "Birthyear must be between 1820 and the current year");
    }

    [TestMethod]
    public void BirthYear_BorderValues_Work()
    {
        var a = new Actor(1, "John Doe", 1990, "USA");
        a.Birthyear = 1820;
        Assert.AreEqual(1820, a.Birthyear);
        var currentYear = DateTime.Now.Year;
        a.Birthyear = currentYear;
        Assert.AreEqual(currentYear, a.Birthyear);
    }

    [TestMethod]
    public void ToString_Formats_Correctly()
    {
        var a = new Actor(1, "John Doe", 1990, "USA");
        var expected = "Id: 1, Name: John Doe, Birthyear: (1990), Country: USA";
        Assert.AreEqual(expected, a.ToString());
    }

}
