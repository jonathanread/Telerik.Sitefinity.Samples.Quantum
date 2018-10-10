using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using SitefinityWebApp.Mvc.Models;
using SitefinityWebApp.PushNotifications;
using SitefinityWebApp.Tasks;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Data.Events;
using Telerik.Sitefinity.Frontend;
using Telerik.Sitefinity.Frontend.Navigation.Mvc.Models;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Modules.News;
using Telerik.Sitefinity.News.Model;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Workflow;
using FcmSharp.Requests;

namespace SitefinityWebApp
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
			Bootstrapper.Initialized += this.Bootstrapper_Initialized;
			Bootstrapper.Bootstrapped += Bootstrapper_Bootstrapped;
			SystemManager.ApplicationStart += new EventHandler<EventArgs>(ApplicationStartHandler);
		}
		private void ApplicationStartHandler(object sender, EventArgs e)
		{
			EventHub.Subscribe<IDataEvent>(evt => DataEventHandler(evt));
		}

		public void DataEventHandler(IDataEvent eventInfo)
		{
			var action = eventInfo.Action;
			var contentType = eventInfo.ItemType;
			if (action == "New" && contentType == typeof(NewsItem))
			{
				var itemId = eventInfo.ItemId;
				var providerName = eventInfo.ProviderName;

				var manager = ManagerBase.GetMappedManager(contentType, providerName);
				if (manager.GetItemOrDefault(contentType, itemId) is NewsItem item && item.Status == ContentLifecycleStatus.Master)
				{
					// Construct the Data Payload to send:
					var data = new Dictionary<string, string>()
									 {
										{"id", item.Id.ToString()},
										{"test","testing" }
									 };

					// The Message should be sent to the News Topic:
					var message = new FcmMessage()
					{
						ValidateOnly = false,
						Message = new Message
						{
							Topic = "news",
							Data = data,
							Notification = new Notification()
							{
								Body = item.Title,
								Title = "Hot of the presses!"
							}
						}
					};

					var result = PushNotifications.PushNotifications.SendNotificationFromFirebaseCloud(message);
					result.ToString();
				}
			}
		}

		private void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs e)
		{
			if (e.CommandName == "Bootstrapped")
			{
				FrontendModule.Current.DependencyResolver.Rebind<INavigationModel>().To<CustomNavigationModel>();
				//RenewContentTask.CreateTask();

			}
		}

		protected void Bootstrapper_Bootstrapped(object sender, EventArgs e)
		{
			FeatherActionInvokerCustom.Register();
		}
	}
}