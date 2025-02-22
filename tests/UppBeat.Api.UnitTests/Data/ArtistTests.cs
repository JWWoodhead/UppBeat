using Uppbeat.Api.Data;

namespace Uppbeat.Api.UnitTests.Data;

public class ArtistTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var artist = new Artist("New Artist");

        Assert.Equal("New Artist", artist.Name);
    }
}
