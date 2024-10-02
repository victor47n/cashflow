using FluentAssertions;
using System.Net;
using System.Net.Mime;

namespace WebApi.Test.Expenses.Reports;
public class GenerateExpensesReportTest : CashFlowClassFixure
{
    private const string METHOD = "api/report";

    private readonly string _adminToken;
    private readonly string _teamMemberToken;
    private readonly DateTime _expenseDate;

    public GenerateExpensesReportTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _adminToken = webApplicationFactory.UserAdmin.GetToken();
        _teamMemberToken = webApplicationFactory.UserTeamMember.GetToken();
        _expenseDate = webApplicationFactory.ExpenseAdmin.GetDate();
    }

    [Fact]
    public async Task SuccessPdf()
    {
        var result = await DoGet(requestUri: $"{METHOD}/pdf?month={_expenseDate:yyyy/MM}", token: _adminToken);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Content.Headers.ContentType.Should().NotBeNull();
        result.Content.Headers.ContentType!.MediaType.Should().Be(MediaTypeNames.Application.Pdf);
    }

    [Fact]
    public async Task SuccessExcel()
    {
        var result = await DoGet(requestUri: $"{METHOD}/excel?month={_expenseDate:yyyy/MM}", token: _adminToken);

        result.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Content.Headers.ContentType.Should().NotBeNull();
        result.Content.Headers.ContentType!.MediaType.Should().Be(MediaTypeNames.Application.Octet);
    }

    [Fact]
    public async Task ErrorForbiddenUserNotAllowedPdf()
    {
        var result = await DoGet(requestUri: $"{METHOD}/pdf?month={_expenseDate:yyyy/MM}", token: _teamMemberToken);

        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ErrorForbiddenUserNotAllowedExcel()
    {
        var result = await DoGet(requestUri: $"{METHOD}/excel?month={_expenseDate:yyyy/MM}", token: _teamMemberToken);

        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
