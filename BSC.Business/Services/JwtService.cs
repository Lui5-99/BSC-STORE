using System;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using BSC.Models;


namespace BSC.Business.Services
{
  public class JwtService(IConfiguration configuration)
  {
    private readonly IConfiguration _configuration = configuration;

    /* public string GenerateToken()
    {
      var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
      var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
    } */
  }
}