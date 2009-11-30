using System.Web.Mvc;

namespace Maxfire.Web.Mvc
{
	public abstract class OpinionatedResourceSparkView<TEditModel, TInputModel, TId> : OpinionatedSparkView<TEditModel>
		where TEditModel : EditModelFor<TInputModel>
		where TInputModel : class, IEntityViewModel<TId>
	{
		public string ActionFor<TRestfulController>()
			where TRestfulController : Controller, IRestfulController<TInputModel, TId>
		{
			return ViewModel.Input.IsTransient ?
			                                   	this.UrlFor<TRestfulController>(x => x.Create(null)).ToString() :
			                                   	                                                                	this.UrlFor<TRestfulController>(x => x.Update(null)).Id(ViewModel.Input.Id).ToString();
		}
	}
}