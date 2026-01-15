namespace EasyDoc.Domain.Constants;

// making some of these dynamic per each doctor might be a good choice
public static class AppointmentConstants
{
    public const long MinAppointmentTimeInMinutes = 10;
    public const long MaxAppointmentTimeInMinutes = 120;
    public const long DefaultAppointmentTimeInMinutes = 30;
    public const long MaxAppointmentLeadTimeInDays = 30;
}
