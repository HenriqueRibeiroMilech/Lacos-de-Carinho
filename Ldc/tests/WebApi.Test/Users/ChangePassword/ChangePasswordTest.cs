using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using Ldc.Communication.Requests;
using Ldc.Exception;
using FluentAssertions;
using WebApi.Test.InlineData;

namespace WebApi.Test.Users.ChangePassword;

public class ChangePasswordTest : TestClassFixture
{
    private const string METHOD = "api/User/change-password";

    private readonly string _token;
    private readonly string _password;
    private readonly string _email;
    
    public ChangePasswordTest(CustomWebApplicationFactory webApplicationFactory) : base(webApplicationFactory)
    {
        _token = webApplicationFactory.User_Team_Member.GetToken();
        _password = webApplicationFactory.User_Team_Member.GetPassword();
        _email = webApplicationFactory.User_Team_Member.GetEmail();
    }
    
    [Fact]
    public async Task Success()
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = _password;
        
        var response = await DoPut(METHOD, request, token: _token);
        
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var loginRequest = new RequestLoginJson
        {
            Email = _email,
            Password = _password,
        };

        response = await DoPost("api/Login", loginRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        
        loginRequest.Password = request.NewPassword;
        
        response = await DoPost("api/Login", loginRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Password_Different_Current_Password(string cultureInfo)
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        
        var response = await DoPut(METHOD, request, token: _token, culture: cultureInfo);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync();
        
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();
        
        var expectedMessage = ResourceErrorMessages.ResourceManager.GetString("PASSWORD_DIFFERENT_CURRENT_PASSWORD", new CultureInfo(cultureInfo));
        
        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));
    }
}