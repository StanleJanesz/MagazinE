using MagazinEAPI.utils;
using SharedLibrary.Base_Classes___Database;
using SharedLibrary.DTO_Classes;
using MagazinEAPI.Models;
using MagazinEAPI.Models.Users.Journalists;
using MagazinEAPI.Models.Users.Admins;
using MagazinEAPI.Models.Users.Editors;
using MagazinEAPI.Models.Users.Readers;
using MagazinEAPI.Models.Users;


namespace xUnitTests
{
    public class ClassFinderTests
    {
        [Fact]
        public void Test1()
        {
            RoleFinder roleFinder = new RoleFinder();
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Admin = new Admin()
            };
            Assert.Equal("Admin", roleFinder.FindRole(applicationUser))
            ;

        }
        [Fact]
        public void Test2()
        {
            RoleFinder roleFinder = new RoleFinder();
            ApplicationUser applicationUser = new ApplicationUser()
            {
                HeadEditor = new HeadEditor()
            };
            Assert.Equal("Head Editor", roleFinder.FindRole(applicationUser))
            ;

        }
        [Fact]
        public void Test3()
        {
            RoleFinder roleFinder = new RoleFinder();
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Editor = new Editor()
            };
            Assert.Equal("Editor", roleFinder.FindRole(applicationUser))
            ;

        }
        [Fact]
        public void Test4()
        {
            RoleFinder roleFinder = new RoleFinder();
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Journalist = new Journalist()
            };
            Assert.Equal("Journalist", roleFinder.FindRole(applicationUser))
            ;

        }
        [Fact]
        public void Test5()
        {
            RoleFinder roleFinder = new RoleFinder();
            ApplicationUser applicationUser = new ApplicationUser()
            {
                User = new User()
            };
            Assert.Equal("User", roleFinder.FindRole(applicationUser))
            ;

        }
        [Fact]
        public void Test6()
        {
            RoleFinder roleFinder = new RoleFinder();
            ApplicationUser applicationUser = new ApplicationUser();
            Assert.Throws<Exception>(() => roleFinder.FindRole(applicationUser))
            ;

        }

    }
}
