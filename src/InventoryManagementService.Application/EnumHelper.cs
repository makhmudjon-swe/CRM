namespace WholesaleCRM.Application;

public static class EnumHelper
{
    public static string GetDealStatusName(int status) => status switch
    {
        0 => "Yangi Lead",
        1 => "Qualified",
        2 => "Taklif",
        3 => "Muzokaralar",
        4 => "Yutildi",
        5 => "Yutqizildi",
        _ => "Noma'lum"
    };

    public static string GetDealStatusBadge(int status) => status switch
    {
        0 => "secondary",
        1 => "info",
        2 => "primary",
        3 => "warning",
        4 => "success",
        5 => "danger",
        _ => "secondary"
    };

    public static string GetActivityTypeName(int type) => type switch
    {
        0 => "Qo'ng'iroq",
        1 => "Email",
        2 => "Uchrashuv",
        3 => "Eslatma",
        4 => "Vazifa",
        _ => "Noma'lum"
    };

    public static string GetActivityTypeIcon(int type) => type switch
    {
        0 => "fa-phone",
        1 => "fa-envelope",
        2 => "fa-handshake",
        3 => "fa-sticky-note",
        4 => "fa-tasks",
        _ => "fa-circle"
    };

    public static string GetCustomerStatusName(int status) => status switch
    {
        0 => "Potensial",
        1 => "Faol",
        2 => "Nofaol",
        _ => "Noma'lum"
    };

    public static string GetCustomerStatusBadge(int status) => status switch
    {
        0 => "warning",
        1 => "success",
        2 => "secondary",
        _ => "secondary"
    };
}
