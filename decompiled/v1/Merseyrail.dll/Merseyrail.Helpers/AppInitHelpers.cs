using System.Globalization;
using Common.IoC;
using Common.Services;
using Merseyrail.Services;
using Merseyrail.Shared;

namespace Merseyrail.Helpers;

public static class AppInitHelpers
{
	private static bool isInit;

	public static void AppIoCInit()
	{
		if (!isInit)
		{
			CultureInfo.DefaultThreadCurrentUICulture = (CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-gb"));
			IoCUtils.RegisterSingletons();
			SharedServices.IoC.Register<IConnectivityService, ConnectivityService>().AsSingleton();
			SharedServices.IoC.Register<DatabaseFileService>().AsSingleton();
			SharedServices.IoC.Register<ISettingsService, SettingsService>().AsSingleton();
			SharedServices.IoC.Register<FragmentService>().AsSingleton();
			SharedServices.IoC.Register<LocationService>().AsSingleton();
			SharedServices.IoC.Register<BusService>().AsSingleton();
			isInit = true;
		}
	}
}
