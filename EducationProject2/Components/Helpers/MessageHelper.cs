using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;

namespace EducationProject2.Components.Helpers
{
    public static class MessageHelper
    {
        public static void ShowToastNotification(string message)
        {
            string toastXmlString = $@"
            <toast>
                <visual>
                    <binding template='ToastGeneric'>
                        <text>Notification</text>
                        <text>{message}</text>
                    </binding>
                </visual>
            </toast>";

            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastXmlString);

            ToastNotification toast = new ToastNotification(toastXml);

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
        
        public static ContentDialog GetInfoDialog(string message, string title)
        {
            return new ContentDialog()
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "Ok"
            };
        }
    }
}