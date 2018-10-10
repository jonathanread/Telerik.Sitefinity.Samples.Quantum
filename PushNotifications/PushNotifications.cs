using FcmSharp;
using FcmSharp.Requests;
using FcmSharp.Settings;
using System;
using System.Threading;
using System.Web.Hosting;
using Telerik.Sitefinity.Abstractions;

namespace SitefinityWebApp.PushNotifications
{
	/// <summary>
	/// Static class for sending push notifications
	/// </summary>
	public static class PushNotifications
	{
		/// <summary>
		/// Send push notifications
		/// </summary>
		/// <returns>Results from API</returns>
		public static string SendNotificationFromFirebaseCloud(FcmMessage message)
		{
			string result = string.Empty;
			string serviceKeyPath = HostingEnvironment.MapPath("~/App_Data/Sitefinity/quantum-app-dea2a-firebase-adminsdk-flpe9-5a5652bbb2.json");
			serviceKeyPath.ToString();
			var settings = FileBasedFcmClientSettings.CreateFromFile("quantum-app-dea2a", serviceKeyPath);

			// Construct the Client:
			using (var client = new FcmClient(settings))
			{
				// Finally send the Message and wait for the Result:
				try
				{
					CancellationTokenSource cts = new CancellationTokenSource();

					// Send the Message and wait synchronously:
					var response = client.SendAsync(message, cts.Token).GetAwaiter().GetResult();
					result = response.Name;
				}
				catch (Exception ex)
				{
					Log.Write(ex, System.Diagnostics.TraceEventType.Error);
				}

			}
			return result;
		}
	}
}