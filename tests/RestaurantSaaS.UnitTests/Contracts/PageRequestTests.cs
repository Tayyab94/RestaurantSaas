using RestaurantSaaS.Contracts.Paging;

namespace RestaurantSaaS.UnitTests.Contracts;

public class PageRequestTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(3, 3)]
    public void Page_is_never_below_one(int input, int expected) =>
        Assert.Equal(expected, new PageRequest(Page: input).NormalizedPage);

    [Theory]
    [InlineData(0, 25)]
    [InlineData(500, 100)]
    [InlineData(40, 40)]
    public void PageSize_is_defaulted_and_capped_at_100(int input, int expected) =>
        Assert.Equal(expected, new PageRequest(PageSize: input).NormalizedPageSize);

    [Fact]
    public void Skip_is_calculated_from_page_and_size() =>
        Assert.Equal(50, new PageRequest(3, 25).Skip);

    [Fact]
    public void CursorRequest_limit_is_capped_at_100() =>
        Assert.Equal(100, new CursorRequest(Limit: 1000).NormalizedLimit);
}
