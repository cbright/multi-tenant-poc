namespace Contracts;
public class SubmitApplication
{
    public SubmitApplication()
    {
     Tenant = string.Empty;
    }

    public string Tenant {get; set;} 
    public Guid Id => Guid.NewGuid();
}
