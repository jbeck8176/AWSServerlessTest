using AWSServerlessApiTest.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AWSServerlessApiTest.Controllers
{
    [Produces("application/json")]
    [Route("api/Widget")]
    public class WidgetController : Controller
    {
	    private readonly IWidgetRepository _widgetRepository;

	    public WidgetController(IWidgetRepository widgetRepository)
	    {
		    _widgetRepository = widgetRepository;
	    }

        // GET: api/Widget/Name/5
        [HttpGet("Name/{id}")]
        public string GetWidgetName(int id)
        {
	        var widgetName = _widgetRepository.GetWidgetNameById(id);
	        return widgetName;
        }
    }
}
