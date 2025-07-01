using DesktopTool.App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TokenStorage.cs
namespace DesktopTool.App.Service;

public static class TokenStorage
{
    private static string _jwtToken;
    private static string _refreshToken;

    public static void SaveToken(string token)
    {
        _jwtToken = token;
    }
    public static void SaveRefreshToken(string token)
    {
        _refreshToken = token;
    }

    public static string GetToken()
    {
        return _jwtToken;
    }

    public static string GetRefreshToken()
    {
        return _refreshToken;
    }

    public static bool IsTokenAvailable => !string.IsNullOrWhiteSpace(_jwtToken);
}


