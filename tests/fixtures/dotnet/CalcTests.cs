using Xunit;

namespace Fixture.Tests;

public static class Calc
{
    public static int Add(int a, int b) => a + b;
}

public class CalcTests
{
    [Fact]
    public void AddsTwoNumbers() => Assert.Equal(5, Calc.Add(2, 3));
}
