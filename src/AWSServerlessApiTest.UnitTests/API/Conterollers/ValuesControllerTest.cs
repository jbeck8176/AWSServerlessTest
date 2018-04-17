using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using AWSServerlessApiTest.Controllers;
using Xunit;

namespace AWSServerlessApiTest.UnitTests.API.Conterollers
{
    public class ValuesControllerTest
    {
	    private readonly IFixture _fixture;

	    private ValuesController _controller;

	    public ValuesControllerTest()
	    {
			_fixture = new Fixture();

			_controller = new ValuesController();
	    }

	    [Fact]
	    public void GetShouldEchoIdInput()
	    {
		    var fakeId = _fixture.Create<int>();

		    var result = _controller.Get(fakeId);

			Assert.Equal(fakeId.ToString(), result);
	    }
    }
}
