﻿using CashFlow.Domain.Entities;

namespace WebApi.Test.Resources;
public class UserIdentityManager
{
    private User _user;
    private string _password;
    private string _token;

    public UserIdentityManager(User user, string password, string token)
    {
        _user = user;
        _password = password;
        _token = token;
    }

    public string GetName() => _user.Name;

    public string GetEmail() => _user.Email;

    public string GetPassword() => _password;

    public string GetToken() => _token;
}
