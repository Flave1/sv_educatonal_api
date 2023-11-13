using BLL.AuthenticationServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class RolesServiceTests
{
   [TestMethod]
    public async Task CreateRoleIfNotCreated_RoleDoesNotExist_CreatesRole()
    {
        // Arrange
        var mockRolesService = new Mock<IRolesService>();

        //// Configure the mock to return a completed Task when CreateRoleIfNotCreated is called.
        //mockRolesService
        //    .Setup(x => x.CreateRoleIfNotCreated("SCHOOL_ADMIN719bb2d9-9a5d-45da-78dd-08db883ae6f0"))
        //    .Returns((Task<string>)Task.CompletedTask);

        // Act
        // Perform some action that calls CreateRoleIfNotCreated.

        // Assert
        // Assert the behavior or result of the action.
    }


}