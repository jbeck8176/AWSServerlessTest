using AutoFixture;
using AWSServerlessApiTest.Data.Models;
using AWSServerlessApiTest.Data.Repositories;
using AWSServerlessApiTest.SlowTests.Tools;
using Xunit;

namespace AWSServerlessApiTest.SlowTests.Data.Repositories
{
	[Collection("SlowTestContext")]
    public class WidgetRepositoryTest
    {
	    private readonly SlowTestContextMaster _slowTestContext;
	    private readonly IFixture _fixture;

	    private readonly IWidgetRepository _widgetRepository;

	    public WidgetRepositoryTest(SlowTestContextFixture slowTestContextFixture)
	    {
		    _fixture = new Fixture();
		    _slowTestContext = slowTestContextFixture.MasterContext;
		    _slowTestContext.ResetData();

			_widgetRepository = new WidgetRepository(SlowTestContextMaster.SlowTestSqlConnectionSettings);
		}

	    [Fact]
	    public void GetWidgetName_should_get_widget_name()
	    {
		    var fakeWidget = _fixture.Create<Widget>();
		    _slowTestContext.Widgets.Add(fakeWidget);
		    _slowTestContext.SaveChanges();

		    var result = _widgetRepository.GetWidgetNameById(fakeWidget.Id);

			Assert.Equal(fakeWidget.Name, result);
	    }
    }
}
