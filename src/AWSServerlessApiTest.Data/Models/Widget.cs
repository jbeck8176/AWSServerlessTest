using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AWSServerlessApiTest.Data.Models
{
	[Table("Custom_Widgets")]
    public class Widget
    {
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
	    public int Id { get; set; }
	    [Column("WidgetName")]
	    public string Name { get; set; }
    }
}
