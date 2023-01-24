# Merseyrail.dll

## Merseyrail

### ActivityResultCodes.cs
An enum representing some in-app actions a user could do.

### AppPreferencesActivity.cs
App user preference manager (name, postcode, usual routes etc.).

### CalamityAlertActivity.cs
Merseyrail service alert JSON deserialiser.

### CompassMapActivity.cs
Map compass manager (sensor listener -> map rotation constructor).

### Feedback.cs
In-app feedback sender.

### HelpDisplay.cs
Displays `HelpDisplayItem`s and `HelpOverlayFragment`s.

### HelpDisplayItem.cs
An single help message.

### HelpOverlayFragment.cs
Displays zero or more `HelpPageIndicator`s.

### HelpPageIndicator.cs
A help page indicator with a background image.

### JourneyPlannerDetailFareListAd.cs
Full class name is `JourneyPlannerDetailFareListAdapter`. Displays info about a fare including a description, amount, and saving.

### JourneyPlannerDetailListAdapte.cs
Full class name is `JourneyPlannerDetailListAdapter`. Displays info about a journey including start and end stations and platforms, journey time, and operator.

### JourneyPlannerListAdapter.cs
Displays info about a user's planned journey.

### LiveMapInfoWindowAdapter.cs
Resizes the live map upon user interaction.

### LiveTrain.cs
Empty view.

### Loader.cs
Fetches resources from databases such as Firebase.

### Main.cs
Enables analytics and registers main callbacks and views.

### MenuItem.cs
A single class containing properties such as schedule times and destination name.

### MenuItemAdapter.cs
Displays `MenuItem`s.

### MtogoActivity.cs
Displays offers.

### MyApplication.cs
The application's entrypoint.

### NotificationType.cs
Enum describing notification purpose.

### NotificationViewer.cs
Displays Merseyrail notifications.

### PreferencesActivity.cs
Registers callbacks and delegates.

### PreferencesAlertActivity.cs
Displays controls relating to app alerts

### PreferencesManageAlerts.cs
Loads and saves settings relating to app alerts.

### PreferencesProfileActivity.cs
Displays controls relating to the user's profile and preferences.

### RainbowBoardIncidentsActivity.cs
Displays incidents on the rainbow board.

### RainbowBoardIncidentsListAdapt.cs
Full class name is `RainbowBoardIncidentsListAdapter`.+

### ReportAProblem.cs
### Resource.cs
### RotateView.cs
### ShareTrain.cs
### StationDetailActivity.cs
### StationListAdapter.cs
### StationSelectorActivity.cs
### TestData.cs

## Merseyrail.Activities
### DeveloperActivity.cs
### FaqActivity.cs
### FaqsActivity.cs
### OfferActivity.cs

## Merseyrail.Adapters
### DeveloperItem.cs
### DeveloperListAdapter.cs
### FaqAdapter.cs
### ImageAdapter.cs
### LiveTrainStationsListAdapter.cs
### OfferAdapter.cs
### SettingsV2ListAdapter.cs
### TrainJourneyListAdapter.cs

## Merseyrail.Adapters.LayoutMana
### NestedLayoutManager.cs

## Merseyrail.Adapters.ViewHolder
### OfferViewHolder.cs

## Merseyrail.AppWidgets
### DeparturesWidgetPreferencesAct.cs
### DeparturesWidgetProvider.cs
### DeparturesWidgetProviderExtens.cs
### DeparturesWidgetService.cs
### LinqExtensions.cs
### LocationUpdater.cs
### RemoteViewsFactory.cs
### StackRemoteViewsFactory.cs
### WidgetDataModel.cs
### WidgetDepartureItem.cs

## Merseyrail.BroadcastReceivers
### DeparturesListItemClickReceive.cs
### JourneyPlannerProgressReceiver.cs
### LocationUpdatedReceiver.cs
### ReceiverTypes.cs
### WidgetStationSelectedEventArgs.cs
### WidgetStationSelectedReceiver.cs

## Merseyrail.Domain
### TrainJourneyItem.cs

## Merseyrail.Events
### RedeemTimerFinishEventArgs.cs
### RedeemTimerTickEventArgs.cs

## Merseyrail.Firebase.Messaging
### MerseyrailFirebaseMessagingSer.cs

## Merseyrail.Fragments
### BaseFragment.cs
### CompassFragment.cs
### CustomClient.cs
### DateTimeDialogClosedEventArgs.cs
### DateTimeDialogFragment.cs
### Departures.cs
### Feedback.cs
### JourneyPlanner.cs
### JourneyPlannerDetail.cs
### JourneyPlannerList.cs
### ListHeader.cs
### LiveMap.cs
### LiveTrain.cs
### LiveTrainAlert.cs
### MapFragment.cs
### MenuFragment.cs
### MenuItem.cs
### PagedFragmentBase.cs
### PagedFragmentSection.cs
### PageDirection.cs
### PageHeader.cs
### RainbowBoard.cs
### RainbowBoardItem.cs
### SectionFragmentBase.cs
### SectionFragmentType.cs
### Settings.cs
### SettingsV2.cs
### StationDetail.cs
### StationSelector.cs
### TimePickerFragment.cs
### TrainJourneyList.cs
### WebViewFragment.cs
### WirralTrackReplacementView.cs

## Merseyrail.Helpers
### AppInitHelpers.cs
### BitmapHelpers.cs
### CalamityHelpers.cs
### NotificationHelper.cs
### ObjectTypeHelper.cs
### PermissionHelper.cs
### UIUtils.cs
### UnitHelpers.cs

## Merseyrail.Notifications
### BaseNotification.cs
### CalamityNotification.cs
### IncidentNotification.cs
### INotificationHandler.cs
### ReminderNotification.cs

## Merseyrail.Notifications.Model
### BaseAlert.cs
### CalamityFCMAlert.cs
### IncidentAlert.cs
### ReminderAlert.cs

## Merseyrail.Pager
### NonSwipePager.cs

## Merseyrail.Providers
### DeparturesWidgetContentProvide.cs
### MerseyrailDatabase.cs

## Merseyrail.Rotator
### RotateView.cs

## Merseyrail.Services
### DatabaseFileService.cs
### FragmentService.cs
### LocationService.cs
### SettingsService.cs

## Merseyrail.Shared
### SharedServices.cs
### SharedSettings.cs
### SharedValues.cs

## Merseyrail.Timers
### RedeemTimer.cs

## Merseyrail.Views
### Arrow.cs
### LiveTrainIcon.cs
### AssemblyInfo.cs