using FluentValidation.TestHelper;

using Application.DTOs.Common.Pagination;
using Application.Validators.Common;

namespace UnitTests.Application.Validators.Common;

public sealed class PaginationRequestValidatorTests
{
    private readonly PaginationRequestValidator _validator = new();
    private sealed record TestPaginationRequest(
        int Page = 1,
        int PageSize = 10
    ) : PaginationRequest(Page, PageSize);

    [Fact]
    public void NaoDeve_GerarErro_Quando_RequisicaoValida()
    {
        var request = new TestPaginationRequest(1, 10);
        
        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(10)] 
    public void NaoDeve_GerarErro_Quando_PageEhValido(int page)
    {
        var request = new TestPaginationRequest(page, 10);

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Deve_GerarErro_Quando_PageInvalido(int page)
    {
        var request = new TestPaginationRequest(page, 10);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage(ValidationMessages.InvalidPage);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void NaoDeve_GerarErro_Quando_PageSizeEhValido(int pageSize)
    {
        var request = new TestPaginationRequest(1, pageSize);

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_GerarErro_Quando_PageSizeInvalido(int pageSize)
    {
        var request = new TestPaginationRequest(1, pageSize);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage(ValidationMessages.InvalidPageSize);
    }
}