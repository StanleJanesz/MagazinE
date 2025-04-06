using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagazinEAPI.utils;


using Xunit.Sdk;

namespace xUnitTests
{
    public class CreateJWTTests
    {

        [Fact]
        public void Test1()
        {
            var jwt = new JWTCreator().CreateJWTToken("test", "test");
            Assert.NotNull(jwt);
        }

        [Fact]
        public void Test2()
        {
            var jwt = new JWTCreator().CreateJWTToken("test", "test");
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            Assert.Equal("MagazinEServer", token.Issuer);
            Assert.Equal("MagazinEClient", token.Audiences.FirstOrDefault());
            Assert.Equal("test", token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value);
        }
    }
}
