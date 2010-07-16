//////////////////////////////////////////////////////////////////////
// The SimplyRestful routes are contributed by Adam Tybor, see
// http://abombss.com/blog/2007/12/10/ms-mvc-simply-restful-routing/
//////////////////////////////////////////////////////////////////////
namespace Maxfire.Web.Mvc.SimplyRestful
{
	/// <summary>
	/// The canonic 7 rails inspired restful actions
	/// </summary>
	public enum RestfulAction
	{
		None    = 0,
		Index   = 1 << 1,  // GET /
		Show    = 1 << 2,  // GET /{id}
		Create  = 1 << 3,  // POST /
		Update  = 1 << 4,  // PUT /{id}    or POST /{id} (_method = PUT)
		Destroy = 1 << 5,  // DELETE /{id} or POST /{id} (_method = DELETE)
		New     = 1 << 6,  // GET /new
		Edit    = 1 << 7   // GET /{id}/edit
	}
}