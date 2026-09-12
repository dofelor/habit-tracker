using Xunit;

namespace habit_tracker.Tests;

public class TestsDateValidate
{
    [Theory]
    [InlineData("12-05-24")] 
    [InlineData("01-01-00")] 
    [InlineData("31-12-99")] 
    public void IsValidDate_ValidFormat_ReturnsTrue(string validDate)
    {
        bool result = DateValidator.IsValidDate(validDate);

        Assert.True(result);
    }

    [Theory]
    [InlineData("31-02-24")] 
    [InlineData("2024-05-12")] 
    [InlineData("invalid")]  
    [InlineData("")]          
    public void IsValidDate_InvalidFormat_ReturnsFalse(string invalidDate)
    {
        bool result = DateValidator.IsValidDate(invalidDate);

        Assert.False(result);
    }
}