
namespace IMCApp.Utilidades
{
    public static class ConexionDB
    {
        public static string devolverRuta(string nombreBD)
        {
            string rutaBD = string.Empty;
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                rutaBD = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                rutaBD = Path.Combine(rutaBD, nombreBD);
            } 
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                rutaBD = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                rutaBD = Path.Combine(rutaBD, "..", "Library", nombreBD);
            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                rutaBD = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                rutaBD = Path.Combine(rutaBD, nombreBD);
            }

            return rutaBD;
        }
    }
}
